using CoreWebApi.Library.ResponseError;
using CoreWebApi.Models.Account;
using CoreWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class AccountController : ControllerBase
    {
        #region Private members

        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender emailSender;
        private readonly IAccountService accountService;
        private readonly ITokenService tokenService;
        private IResponseError responseBadRequestError;
        private IResponseError responseNotFoundError;

        #endregion

        #region ctor

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ITokenService tokenService,
            IAccountService accountService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.tokenService = tokenService;
            this.accountService = accountService;
            responseBadRequestError = ResponseErrorFactory.getBadRequestError("");
            responseNotFoundError = ResponseErrorFactory.getNotFoundError("User Not Found.");
        }

        #endregion

        /// <summary>
        /// Creates the new IdentityRole in db by given unique parameter.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/account/createRole/{role}
        ///
        /// Returns object like this:
        ///      {
        ///        "id": "2194ad72-81db-410b-9bdd-d172732e3338",
        ///        "name": "Test",
        ///        "normalizedName": "TEST",
        ///        "concurrencyStamp": "f8bf512d-5213-4f0a-a774-4f0f70547a7c"
        ///      }
        ///     
        /// </remarks>
        /// <param name="role">Role name</param>
        /// <returns>Created IdentityRole object</returns>
        /// <response code="201">Returns the created IdentityRole object</response>
        /// <response code="400">If the argument is not unique</response>
        /// <response code="401">If the user is not in Admin-role</response>
        [HttpGet("{role}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRoleAsync([FromRoute] string role)
        {
            var roleToSave = new IdentityRole { Name = role };
            if (!(await roleManager.CreateAsync(roleToSave)).Succeeded)
            {
                responseBadRequestError.Title = $"Unable to add role {role}.";
                return BadRequest(responseBadRequestError);
            }

            return Created("/api/account/createrole/{role}", roleToSave);
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
        ///        "existingEmail": "test@gmail.com",
        ///        "newEmail": "newtest@gmail.com",
        ///        "password": "Password1."
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns status 200 and message</response>
        /// <response code="400">If the arguments are not valid: unique new email and correct password. Also while email server is not running.</response>
        /// <response code="401">If the user is not authorized</response>
        /// <response code="404">If the user not found</response>
        [HttpPost]
        [Authorize(Roles = "Admin, Registered")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeEmailAsync([FromBody] ChangeEmailDto changeEmailDto)
        {
            var user = await userManager.FindByEmailAsync(changeEmailDto.ExistingEmail);
            if (user == null) return NotFound(responseNotFoundError);
            if (await userManager.FindByEmailAsync(changeEmailDto.NewEmail) != null) // is new email unique
            {
                responseBadRequestError.Title = "The email already in use.";
                return BadRequest(responseBadRequestError);
            }
            if (!ModelState.IsValid || !(await signInManager.CheckPasswordSignInAsync(user, changeEmailDto.Password, false)).Succeeded)
            {
                responseBadRequestError.Title = "Invalid password.";
                return BadRequest(responseBadRequestError);
            }
            await RemoveTokens(user);
            var code = tokenService.GenerateRandomToken(30);
            var result = await userManager.SetEmailAsync(user, changeEmailDto.NewEmail);
            if (!result.Succeeded) // UserManager validates other user's fields while changing email
            {
                responseBadRequestError.Title = result.Errors.ToArray()[0].Description;
                return BadRequest(responseBadRequestError);
            }
            await userManager.SetAuthenticationTokenAsync(user, "CoreWebApi", "emailConfirmation", code);
            try
            {
                await emailSender.SendEmailAsync($"{changeEmailDto.NewEmail}", "Confirmation email link",
                    $"Confirmation email link: http://localhost:3000/email-confirm/?code={code}&email={changeEmailDto.NewEmail}");
            }
            catch
            {
                responseBadRequestError.Title = "Email service is temporarily unavailable.";
                return BadRequest(responseBadRequestError);
            }

            return Ok("Email changed.");
        }

        /// <summary>
        /// Replaces a user's password by new password. Sends confirmation email. 
        /// </summary>
        /// <returns>Status 200 and message</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/account/changepassword
        ///     {
        ///        "email": "test@gmail.com",
        ///        "oldPassword": "Password1.",
        ///        "newPassword": "NewPassword1."
        ///        "confirmNewPassword": "NewPassword1."
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns status 200 and message</response>
        /// <response code="400">If the argument is not valid or some validation rules where broken</response>
        /// <response code="401">If the user is not authorized</response>
        /// <response code="404">If the user not found</response>
        [HttpPost]
        [Authorize(Roles = "Admin, Registered")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto changePasswordDto)
        {
            var user = await userManager.FindByEmailAsync(changePasswordDto.Email);
            if (user == null) return NotFound(responseNotFoundError);
            if (!ModelState.IsValid || !(await signInManager.CheckPasswordSignInAsync(user, changePasswordDto.OldPassword, false)).Succeeded)
            {
                responseBadRequestError.Title = "Invalid password.";
                return BadRequest(responseBadRequestError);
            }
            await RemoveTokens(user);
            var result = await userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                responseBadRequestError.Title = result.Errors.ToArray()[0].Description;
                return BadRequest(responseBadRequestError);
            }
            try
            {
                await emailSender.SendEmailAsync($"{changePasswordDto.Email}", "Reset Password",
                    $"Your password changed, use the new password next login.");
            }
            catch
            {
                responseBadRequestError.Title = "Email service is temporarily unavailable.";
                return BadRequest(responseBadRequestError);
            }

            return Ok("Password changed.");
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
        ///        "userId": "808f4f76-6dd3-4183-9e26-5ac696b9327a",
        ///        "neededRoles": ["Admin", "Registered"]
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns status 200 and list of actual user's roles</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="401">If the user is not authorized</response>
        /// <response code="404">If the user not found</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeUserRolesAsync([FromBody] ChangeRolesDto changeRolesDto)
        {
            var user = await userManager.FindByIdAsync(changeRolesDto.UserId);
            if (user == null) return NotFound(responseNotFoundError);

            foreach (var role in roleManager.Roles.Select(x => x.Name).ToList())
            {
                if (changeRolesDto.NeededRoles.Contains(role) && !(await userManager.IsInRoleAsync(user, role)))
                {
                    await userManager.AddToRoleAsync(user, role);
                }
                else if (!changeRolesDto.NeededRoles.Contains(role) && (await userManager.IsInRoleAsync(user, role)))
                {
                    await userManager.RemoveFromRoleAsync(user, role);
                }
                else
                {
                    continue;
                }
            }

            await RemoveTokens(user);

            return Ok(await userManager.GetRolesAsync(user));
        }

        /// <summary>
        /// Changes User.EmailConfirmed property
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/account/confirmemail?code=TntRy0cNM271AjzCXZ6Tz&amp;email=g@gmail.com
        /// 
        /// </remarks>
        /// <param name="code">Code from confirmation email</param>
        /// <param name="email">Users email</param>
        /// <returns>Ok("Email confirmed.")</returns>
        /// <response code="200">Returns the confirmation of success</response>
        /// <response code="400">If the code or email are wrong</response>
        /// <response code="404">If the user with given email is not found</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string code, string email)
        {
            if (code == null || email == null)
            {
                responseBadRequestError.Title = "Wrong code or email.";
                return BadRequest(responseBadRequestError);
            }
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return NotFound(responseNotFoundError);
            var result = await userManager.GetAuthenticationTokenAsync(user, "CoreWebApi", "emailConfirmation");
            if (result == null || result != code)
            {
                responseBadRequestError.Title = "Unable to confirm email.";
                return BadRequest(responseBadRequestError);
            }
            user.EmailConfirmed = true;
            await userManager.UpdateAsync(user);
            await userManager.RemoveAuthenticationTokenAsync(user, "CoreWebApi", "emailConfirmation");

            return Ok("Email confirmed.");
        }

        /// <summary>
        /// Gets a list of ApplicationUserDto's with pagination params and value for search, sorts by Email.
        /// Uses the 'accountService' for transform ApplicationUser object to ApplicationUserDto
        /// </summary>
        /// <param name="limit">Number of items per page</param>
        /// <param name="page">Requested page</param>
        /// <param name="search">Part of Email for searching</param>
        /// <returns>Status 200 and list of ApplicationUserDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/account/get/?limit=10&amp;page=1&amp;search=&amp;sort_field=Id&amp;order=0
        ///     
        /// </remarks>
        /// <response code="200">List of ApplicationUserDto's</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get(int limit, int page, string search) =>
            Ok(accountService.GetUsersSearchResultAsync(limit, page, search, userManager.Users));

        /// <summary>
        /// Gets all existing user's roles from db. 
        /// </summary>
        /// <returns>Status 200 and list of existing roles</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/account/getroles
        ///     
        /// </remarks>
        /// <response code="200">Returns status 200 and list of actual roles</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetRoles() => Ok(roleManager.Roles.Select(x => x.Name).ToList());

        /// <summary>
        /// Gets ApplicationUserDto by given id. 
        /// </summary>
        /// <returns>Status 200 and ApplicationUserDto-object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/account/getuserbyid/{id}
        ///     
        /// </remarks>
        /// <response code="200">Returns status 200 and ApplicationUserDto</response>
        /// <response code="404">If the user not found</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Registered")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound(responseNotFoundError);

            return Ok(accountService.GetApplicationUserDto(user));
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
        ///         "password": "Password1.",
        ///         "remember": "true"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the newly created access / refresh tokens</response>
        /// <response code="400">If the arguments / password are wrong or email is not confirmed</response>
        /// <response code="404">If the user is not found</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginUserDto loginUserDto)
        {
            var user = await userManager.FindByEmailAsync(loginUserDto.Email);
            if (user == null) return NotFound(responseNotFoundError);
            if (!user.EmailConfirmed)
            {
                responseBadRequestError.Title = "Please confirm your email first.";
                return BadRequest(responseBadRequestError);
            }
            if (!(await signInManager.CheckPasswordSignInAsync(user, loginUserDto.Password, false)).Succeeded)
            {
                responseBadRequestError.Title = "Invalid password.";
                return BadRequest(responseBadRequestError);
            }
            var authModel = await CreateAuthModelAsync(user, loginUserDto.Remember ? 60 * 24 : 15);
            await SaveTokensAsync(user, authModel.Tokens);

            return Ok(authModel);
        }

        /// <summary>
        /// Makes user logged out - deletes access and refresh tokens from db
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/account/logout
        ///     
        /// </remarks>
        /// <param name="email">Users email</param>
        /// <returns>Ok("User logged out.")</returns>
        /// <response code="200">Returns the confirmation of success</response>
        /// <response code="404">If the user with given email not found</response>
        [HttpGet("{email}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LogoutAsync([FromRoute] string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                responseBadRequestError.Title = "User Not Found.";
                return NotFound(responseBadRequestError);
            }
            await RemoveTokens(user);

            return Ok();
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
        ///        "email": "test@gmail.com",
        ///        "password": "Password.",
        ///        "confirmPassword": "Password1."
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created User.Id</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterAsync(RegisterUserDto registerUserDto)
        {
            var user = new ApplicationUser { UserName = registerUserDto.Email, Email = registerUserDto.Email };
            if (!ModelState.IsValid || !(await userManager.CreateAsync(user, registerUserDto.Password)).Succeeded)
            {
                responseBadRequestError.Title = "Unable to register user with the specified credentials.";
                return BadRequest(responseBadRequestError);
            }
            var code = tokenService.GenerateRandomToken(30);
            await userManager.AddToRoleAsync(user, nameof(AppRoles.Registered));
            try
            {
                await Task.WhenAll(userManager.SetAuthenticationTokenAsync(user, "CoreWebApi", "emailConfirmation", code),
                    emailSender.SendEmailAsync($"{user.Email}", "Confirmation email link",
                    $" Please use the confirmation email link: http://localhost:3000/email-confirm/?code={code}&email={user.Email}"));
            }
            catch (Exception ex)
            {
                responseBadRequestError.Title = "Service temporarily unavailable." + ex.Message;
                return BadRequest(responseBadRequestError);
            }

            return Created("/account/register", code);
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
        /// <response code="404">If the user with given email not found</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RefreshTokenAsync(TokenModel tokenModel)
        {
            var userEmail = tokenService.GetUserEmailFromExpiredToken(tokenModel.AccessToken);
            var user = await userManager.FindByEmailAsync(userEmail);
            if (user == null) return NotFound(responseNotFoundError);
            if (tokenService.IsTokenExpired(tokenModel.RefreshToken))
            {
                responseBadRequestError.Title = "Refresh token has expired."; // so, user should log in
                return BadRequest(responseBadRequestError);
            }
            if ((await userManager.GetAuthenticationTokenAsync(user, "CoreWebApi", "refresh")) != tokenModel.RefreshToken)
            {
                responseBadRequestError.Title = "Unable to update tokens.";
                return BadRequest(responseBadRequestError);
            }
            var authModel = await CreateAuthModelAsync(user, 15);
            await SaveTokensAsync(user, authModel.Tokens);

            return Created("/account/refresh", authModel);
        }

        /// <summary>
        /// Partly updates an existing ApplicationUser item.
        /// Uses the 'accountService' for attaching patchDocument and mapping ApplicationUser object to ApplicationUserDto
        /// </summary>
        /// <param name="id">Identifier string id</param>
        /// <param name="patchDocument">Json Patch Document as array of operations</param>
        /// <returns>Status 200 and updated ApplicationUserDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH /api/account/partialuserupdate/{id}
        ///     [
        ///         {
        ///             "op": "replace",
        ///             "path": "/emailConfirmed",
        ///             "value": "true"
        ///         }
        ///     ]
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated ApplicationUserDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the ApplicationUser with given id not found</response>
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PartialUserUpdateAsync(string id, JsonPatchDocument<object> patchDocument)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound(responseNotFoundError);
            try
            {
                return Ok(await accountService.PartialUpdateAsync(user, patchDocument));
            }
            catch
            {
                responseBadRequestError.Title = "Unable to update user's data.";
                return BadRequest(responseBadRequestError);
            }
        }

        /// <summary>
        /// Updates an existing user fields: UserName, PhoneNumber, AvatarUrl.
        /// </summary>
        /// <returns>Status 200 and updated ApplicationUserDto</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/account/update
        ///     {
        ///        "Id": "92c79472-9da6-4da3-a052-358f465d8864",
        ///        "UserName": "John Smith",
        ///        "Email": "test@gmail.com",
        ///        "EmailConfirmed": true,
        ///        "PhoneNumber": "+380961111111",
        ///        "AvatarUrl": "https://somewhere.com/?userphoto=asdasdas"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns status 200 and updated ApplicationUserDto</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="401">If the user is not authorized</response>
        /// <response code="404">If the user not found</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin, Registered")]
        public async Task<IActionResult> UpdateAsync([FromBody] ApplicationUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                responseBadRequestError.Title = "Unable to update user's data.";
                return BadRequest(responseBadRequestError);
            }
            var user = await userManager.FindByIdAsync(userDto.Id);
            if (user == null) return NotFound(responseNotFoundError);

            user.UserName = userDto.UserName;
            user.PhoneNumber = userDto.PhoneNumber;
            user.AvatarUrl = userDto.AvatarUrl;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                responseBadRequestError.Title = result.Errors.ToArray()[0].Description;
                return BadRequest(responseBadRequestError);
            }

            return Ok(userDto);
        }

        private async Task<AuthModel> CreateAuthModelAsync(ApplicationUser user, int rememberPeriod)
        {
            var roles = await userManager.GetRolesAsync(user);
            var tokenClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
            foreach (var role in roles) tokenClaims.Add(new Claim("role", role));

            return new AuthModel()
            {
                User = accountService.GetApplicationUserDto(user),
                Roles = roles,
                Tokens = new TokenModel()
                {
                    AccessToken = tokenService.GenerateAccessToken(tokenClaims, rememberPeriod),
                    RefreshToken = tokenService.GenerateAccessToken(tokenClaims, 60 * 24 * 14)
                }
            };
        }

        private async Task RemoveTokens(ApplicationUser user)
        {
            await userManager.RemoveAuthenticationTokenAsync(user, "CoreWebApi", "access");
            await userManager.RemoveAuthenticationTokenAsync(user, "CoreWebApi", "refresh");
        }

        private async Task SaveTokensAsync(ApplicationUser user, TokenModel tokenModel)
        {
            await userManager.SetAuthenticationTokenAsync(user, "CoreWebApi", "access", tokenModel.AccessToken);
            await userManager.SetAuthenticationTokenAsync(user, "CoreWebApi", "refresh", tokenModel.RefreshToken);
        }
    }
}
