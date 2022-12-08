using CoreWebApi.Data;
using CoreWebApi.Models.Account;
using CoreWebApi.Services;
using CoreWebApi.Services.AccountService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly SeerDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            IConfiguration config,
            SeerDbContext context,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _config = config;
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            var user = new IdentityUser { UserName = registerModel.Email, Email = registerModel.Email };

            user.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(user, registerModel.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                // add user 
                var refreshUser = _context.UserRefreshTokens.SingleOrDefault(u => u.Username == registerModel.Email);
                if (refreshUser != null) return StatusCode(409);

                _context.UserRefreshTokens.Add(new UserRefreshToken
                {
                    Username = registerModel.Email,
                    Password = _passwordHasher.GenerateIdentityV3Hash(registerModel.Password)
                });

                await _context.SaveChangesAsync();
                refreshUser = _context.UserRefreshTokens.SingleOrDefault(u => u.Username == registerModel.Email);
                return Ok(refreshUser);
            }

            return BadRequest("Could not register user.");
        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenModel.AuthenticationToken);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default

            var user = _context.UserRefreshTokens.SingleOrDefault(u => u.Username == username);
            if (user == null || user.RefreshToken != tokenModel.RefreshToken) return BadRequest();

            var newJwtToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _context.SaveChangesAsync();

            return new ObjectResult(new
            {
                authenticationToken = newJwtToken,
                refreshToken = newRefreshToken
            });
        }

        /// <summary>
        /// Login endpoint.
        /// </summary>
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
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginModel.Email);

                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, loginModel.Password, false);
                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Email, user.Email),
                            new Claim(ClaimTypes.Name, user.Email)
                        };

                        var token = _tokenService.GenerateAccessToken(claims);
                        var newRefreshToken = _tokenService.GenerateRefreshToken();

                        var userRefreshToken = _context.UserRefreshTokens.Where(urt => urt.Username == user.Email).FirstOrDefault();
                        userRefreshToken.RefreshToken = newRefreshToken;
                        await _context.SaveChangesAsync();

                        return new ObjectResult(new
                        {
                            authenticationToken = token,
                            refreshToken = newRefreshToken
                        });
                    }
                }
            }

            return BadRequest("Could not create token");
        }
    }
}
