using CoreWebApi.Models.Account;
using CoreWebApi.Services;
using CoreWebApi.Services.AccountService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ITokenService _tokenService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
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
        /// Replaces a user's email by new email. Sends new confirmation letter and removes existing tokens. 
        /// </summary>
        /// <returns>Status 200 and message</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/account/changeemail
        ///     {
        ///        "ExistingEmail": "test@gmail.com",
        ///        "NewEmail": "newtest@gmail.com",
        ///        "Password": "Password."
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns status 200 and message</response>
        /// <response code="404">If the user not found</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto changeEmailDto)
        {
            var user = await _userManager.FindByEmailAsync(changeEmailDto.ExistingEmail);
            if (user == null) return NotFound("User Not Found.");
            if (!ModelState.IsValid ||
                !(await _signInManager.CheckPasswordSignInAsync(user, changeEmailDto.Password, false)).Succeeded ||
                (await _userManager.FindByEmailAsync(changeEmailDto.NewEmail) != null)) // is new email unique
                return BadRequest("Could not change user's email.");
            await RemoveTokens(user);
            // Update user
            user.Email = changeEmailDto.NewEmail;
            user.UserName = changeEmailDto.NewEmail;
            user.EmailConfirmed = false;
            await _userManager.UpdateAsync(user);
            // Send new confirmation letter at new email
            var code = _tokenService.GenerateRandomToken();
            await _userManager.SetAuthenticationTokenAsync(user, "CoreWebApi", "emailConfirmation", code);
            await _emailSender.SendEmailAsync($"{changeEmailDto.NewEmail}", "Confirmation email link",
                $"Confirmation email link: /Account/ConfirmEmail/?token={code}&email={changeEmailDto.NewEmail}");

            return Ok("Email has been changed successfully.");
        }

        /// <summary>
        /// Replaces a user's password by new password. Sends new letter with new password. 
        /// </summary>
        /// <returns>Status 200 and message</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/account/changeemail
        ///     {
        ///        "Email": "test@gmail.com",
        ///        "OldPassword": "Password.",
        ///        "NewPassword": "NewPassword."
        ///        "ConfirmNewPassword": "NewPassword."
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns status 200 and message</response>
        /// <response code="404">If the user not found</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(changePasswordDto.Email);
            if (user == null) return NotFound("User Not Found.");
            if (!ModelState.IsValid || !(await _signInManager.CheckPasswordSignInAsync(user, changePasswordDto.OldPassword, false)).Succeeded)
                return BadRequest("Could not change user's password.");
            await RemoveTokens(user);
            await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
            await _emailSender.SendEmailAsync($"{changePasswordDto.Email}", "Reset Password",
                $"Your password has been changed, use the new password {changePasswordDto.NewPassword} next login.");

            return Ok("Password has been changed successfully.");
        }

        /// <summary>
        /// Gets all existing user's roles from db. 
        /// </summary>
        /// <returns>Status 200 and list of existing roles</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get /api/account/getallexistingroles
        ///     
        /// </remarks>
        /// <response code="200">Returns status 200 and list of actual roles</response>
        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetAllExistingRoles()
        {
            return Ok(_roleManager.Roles.Select(x => x.Name).ToList());
        }

        /// <summary>
        /// Changes user's roles. 
        /// </summary>
        /// <returns>Status 200 and list of actual user's roles</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/account/changeuserroles
        ///     {
        ///        "UserId": "808f4f76-6dd3-4183-9e26-5ac696b9327a",
        ///        "NeededRoles": ["Admin", "Manager"]
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns status 200 and list of actual user's roles</response>
        /// <response code="404">If the user not found</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeUserRoles([FromBody] ChangeRolesDto changeRolesDto)
        {
            var user = await _userManager.FindByIdAsync(changeRolesDto.UserId);
            if (user == null) return NotFound("User Not Found.");

            foreach (var role in _roleManager.Roles.Select(x => x.Name).ToList())
            {
                if (changeRolesDto.NeededRoles.Contains(role) && !(await _userManager.IsInRoleAsync(user, role)))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
                else if (!changeRolesDto.NeededRoles.Contains(role) && (await _userManager.IsInRoleAsync(user, role)))
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }
                else
                {
                    continue;
                }
            }

            await RemoveTokens(user);

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User Not Found.");

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromBody] EditUserDto editUserDto)
        {
            if (!ModelState.IsValid) return BadRequest("Could not update user's data.");
            var user = await _userManager.FindByEmailAsync(editUserDto.Email);
            if (user == null) return NotFound("User Not Found.");

            user.PhoneNumber = editUserDto.Phone;
            user.AvatarUrl = editUserDto.Avatar;
            await _userManager.UpdateAsync(user);

            return Ok(user);
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
            var user = new ApplicationUser { UserName = registerUserDto.Email, Email = registerUserDto.Email };
            if (!ModelState.IsValid || !(await _userManager.CreateAsync(user, registerUserDto.Password)).Succeeded)
                return BadRequest("Could not register user.");
            await _userManager.AddToRoleAsync(user, nameof(AppRoles.Registered));
            var code = _tokenService.GenerateRandomToken();
            await _userManager.SetAuthenticationTokenAsync(user, "CoreWebApi", "emailConfirmation", code);
            await _emailSender.SendEmailAsync($"{user.Email}", "Confirmation email link",
                $"Confirmation email link: /Account/ConfirmEmail/?token={code}&email={user.Email}");

            return Created("/register", user.Id);
        }

        /// <summary>
        /// Changes User.EmailConfirmed property
        /// </summary>
        /// <param name="code">Code from confirmation email</param>
        /// <param name="email">Users email</param>
        /// <returns>Ok("Email confirmed.")</returns>
        /// <response code="200">Returns the confirmation of success</response>
        /// <response code="404">If the tenant with given email is not found</response>
        /// <response code="400">If the token has expired</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail(string code, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User with given email not found.");
            var result = await _userManager.GetAuthenticationTokenAsync(user, "CoreWebApi", "emailConfirmation");
            if (result == null || result != code) return BadRequest("Could not confirm email.");
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            await _userManager.RemoveAuthenticationTokenAsync(user, "CoreWebApi", "emailConfirmation");

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
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            var userEmail = _tokenService.GetUserEmailFromExpiredToken(tokenModel.AccessToken);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (_tokenService.IsTokenExpired(tokenModel.RefreshToken)) return Forbid(); // refresh token has expired - so user should log in
            if (user == null || (await _userManager.GetAuthenticationTokenAsync(user, "CoreWebApi", "refresh"))
                != tokenModel.RefreshToken) return BadRequest("Could not update tokens.");
            var authModel = await CreateAuthModelAsync(user);
            await SaveTokens(user, authModel.Tokens);

            return Created("/account/refresh", authModel);
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
        /// <response code="400">If the arguments / password are wrong or email is not confirmed</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            var user = await _userManager.FindByEmailAsync(loginUserDto.Email);
            if (user == null) return NotFound("User Not Found.");
            if (!ModelState.IsValid ||
                !(await _signInManager.CheckPasswordSignInAsync(user, loginUserDto.Password, false)).Succeeded ||
                !user.EmailConfirmed)
                return BadRequest("Could not login user.");
            var authModel = await CreateAuthModelAsync(user);
            await SaveTokens(user, authModel.Tokens);

            return Ok(authModel);
        }

        [HttpGet]
        public async Task<IActionResult> Logout([FromRoute] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User Not Found.");
            await RemoveTokens(user);

            return Ok("User logged out.");
        }

        private async Task<AuthModel> CreateAuthModelAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var tokenClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
            foreach (var role in roles) tokenClaims.Add(new Claim("role", role));

            return new AuthModel()
            {
                Email = user.Email,
                Roles = roles,
                Tokens = new TokenModel()
                {
                    AccessToken = _tokenService.GenerateAccessToken(tokenClaims, 15),
                    RefreshToken = _tokenService.GenerateAccessToken(tokenClaims, 60 * 24 * 14)
                }
            };
        }

        private async Task SaveTokens(ApplicationUser user, TokenModel tokenModel)
        {
            await _userManager.SetAuthenticationTokenAsync(user, "CoreWebApi", "access", tokenModel.AccessToken);
            await _userManager.SetAuthenticationTokenAsync(user, "CoreWebApi", "refresh", tokenModel.RefreshToken);
        }

        private async Task RemoveTokens(ApplicationUser user)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, "CoreWebApi", "access");
            await _userManager.RemoveAuthenticationTokenAsync(user, "CoreWebApi", "refresh");
        }
    }
}
