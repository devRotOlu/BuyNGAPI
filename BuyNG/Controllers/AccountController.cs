using AutoMapper;
using BuyNG.Core.EntityDTOs;
using BuyNG.Core.Models;
using BuyNG.Core.Services;
using BuyNG.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BuyNG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAuthManager _authManager;
        private readonly string _client;

        public AccountController(IMapper mapper, IAuthManager authManager) : base()
        {
            _mapper = mapper;
            _authManager = authManager;
            _client = "http://localhost:3000/";
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterUser([FromBody] CreateAppUserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appUser = _mapper.Map<ApplicationUser>(userDTO);

            appUser.UserName = userDTO.Email;

            var result = await _authManager.CreateAsync(appUser, userDTO.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }


            try
            {
                await _authManager.GenerateEmailConfirmationTokenAsync(appUser, _client, userDTO.EmailConfirmationPage);
            }
            catch
            {

            }
            finally
            {
                await _authManager.AddToRolesAsync(appUser, userDTO.Roles);
            }

            return Ok("Registration Successful");

        }


        [HttpGet]
        [Route("get-user/{id?}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var user = await _authManager.GetUserByIdAsync(id);

            if (user == null)
            {
                return BadRequest();
            }

            var userDetails = _mapper.Map<AppUserDTO>(user);


            return Ok(new {userDetails});
        }


        [HttpPost]
        [Route("valid-user")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> IsValidUser([FromBody] string email)
        {
            var user = await _authManager.GetUserByEmailAsync(email);

            if (user == null)
            {
                return Forbid();
            }

            return Ok();
        }


        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var result = await _authManager.SignInUser(userDTO);

            if (result.IsNotAllowed)
            {
                return Unauthorized("Email must be confirmed before login");
            }

            var user = await _authManager.GetUserByEmailAsync(userDTO.Email);

            var userDetails = _mapper.Map<AppUserDTO>(user);

            return Accepted(new { token = await _authManager.CreateToken(),userDetails });
        }

        [HttpPost("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmUserEmail([FromQuery] string uid, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(token))
            {
                return BadRequest();
            }

            var result = await _authManager.ConfirmEmailAsync(uid, token);

            if (result.Succeeded)
            {
                return Ok();
            }

            return StatusCode(500);
        }

        [HttpPost("resend-confirmation-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] EmailConfirm emailConfirm)
        {
            if (string.IsNullOrEmpty(emailConfirm.Email))
            {
                return BadRequest();
            }

            var user = await _authManager.GetUserByEmailAsync(emailConfirm.Email);

            if (user == null)
            {
                return BadRequest();
            }

            await _authManager.GenerateEmailConfirmationTokenAsync(user,_client, emailConfirm.EmailConfirmationPage);

            return Ok();
        }

        [HttpPost("password-reset-email")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendPasswordResetEmail([FromBody] RecoverPassword recoverPassword)
        {
            if (string.IsNullOrEmpty(recoverPassword.Email))
            {
                return BadRequest();
            }

            var user = await _authManager.GetUserByEmailAsync(recoverPassword.Email);

            if (user == null)
            {
                return BadRequest();
            }

            await _authManager.GenerateForgotPasswordTokenAsync(user, _client, recoverPassword.PasswordRecoveryPage);

            return NoContent();
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authManager.ResetPasswordAsync(model);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
