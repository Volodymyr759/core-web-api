using CoreWebApi.Models.Account;
using CoreWebApi.Services;
using CoreWebApi.Services.AccountService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ITokenService _tokenService;

        public AccountController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole(string role)
        {
            var roleToSave = new IdentityRole { Name = role };
            if (!(await _roleManager.CreateAsync(roleToSave)).Succeeded) return BadRequest($"Could not add role {role}.");

            return Ok(roleToSave);
        }

        /// <summary>
        /// Creates a new User.
        /// </summary>
        /// <returns>Status 201 and created User.Id</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/account/register
        ///     {
        ///        "Email": "test@gmail.com",
        ///        "Password": "Password.",
        ///        "ConfirmPassword": "Password."
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created User.Id</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            var user = new AppUser { UserName = registerUserDto.Email, Email = registerUserDto.Email };
            if (!ModelState.IsValid || !(await _userManager.CreateAsync(user, registerUserDto.Password)).Succeeded)
                return BadRequest("Could not register user.");
            await _userManager.AddToRoleAsync(user, nameof(AppRoles.Registered));
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _emailSender.SendEmailAsync($"{user.Email}", "Confirmation email link",
                $"Confirmation email link: /Account/ConfirmEmail/?token={emailConfirmationToken}&email={user.Email}");

            return Created("/register", user.Id);
        }

        /// <summary>
        /// Changes User.EmailConfirmed property
        /// </summary>
        /// <param name="token">Token from confirmation email</param>
        /// <param name="email">Users email</param>
        /// <returns>Ok("Email confirmed.")</returns>
        /// <response code="200">Returns the confirmation of success</response>
        /// <response code="404">If the tenant with given email is not found</response>
        /// <response code="400">If the token has expired</response>
        [HttpGet("{token, email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User with given email not found.");
            if (!(await _userManager.ConfirmEmailAsync(user, token)).Succeeded) return BadRequest("Could not confirm email.");

            return Ok("Email confirmed.");
        }

        /// <summary>
        /// Refreshes access and refresh tokens by previous values.
        /// </summary>
        /// <returns>Status 201 and created access and refresh tokens</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/account/refreshtoken
        ///     {
        ///        "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWI...",
        ///        "refreshToken": "PFYIdnb0vMvz0FcV/jKKgbpT3MEA0FNGAPegBkWXr00=",
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created tokens</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPost]
        [Authorize(Roles = "Admin, Manager, Registered")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            var userEmail = _tokenService.GetUserEmailFromExpiredToken(tokenModel.accessToken);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null || (await _userManager.GetAuthenticationTokenAsync(user, "CoreWebApi", "refresh"))
                != tokenModel.refreshToken) return BadRequest("Could not update tokens.");
            var newTokenModel = await CreateTokenModel(user);
            await SaveTokens(user, newTokenModel);

            return Created("/account/refresh", newTokenModel);
        }

        /// <summary>
        /// Creates new access and refresh tokens.
        /// </summary>
        /// <returns>Status 200 and created access and refresh tokens</returns>
        /// <remarks>
        /// Sample request:
        ///     
        ///     POST /api/account/login
        ///     {
        ///         "email": "vvv@gmail.com",
        ///         "password": "Password1."
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the newly created access / refresh tokens</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="400">If the arguments are wrong</response>
        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            var user = await _userManager.FindByEmailAsync(loginUserDto.Email);
            if (user == null) return NotFound("User not Found.");
            if (!ModelState.IsValid || !(await _signInManager.CheckPasswordSignInAsync(user, loginUserDto.Password, false)).Succeeded)
                return BadRequest("Could not login user.");
            var tokenModel = await CreateTokenModel(user);
            await SaveTokens(user, tokenModel);

            return Ok(tokenModel);
        }

        private async Task<TokenModel> CreateTokenModel(IdentityUser user) => new TokenModel()
        {
            accessToken = _tokenService.GenerateAccessToken(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("role", (await _userManager.GetRolesAsync(user))[0])
                }),
            refreshToken = _tokenService.GenerateRefreshToken()
        };

        private async Task SaveTokens(IdentityUser user, TokenModel tokenModel)
        {
            await _userManager.SetAuthenticationTokenAsync(user, "CoreWebApi", "access", tokenModel.accessToken);
            await _userManager.SetAuthenticationTokenAsync(user, "CoreWebApi", "refresh", tokenModel.refreshToken);
        }
    }
}
