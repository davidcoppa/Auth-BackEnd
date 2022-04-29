using Auth_BackEnd.DTOs;
using Auth_BackEnd.Helpers;
using Auth_BackEnd.Model;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Auth_BackEnd.Controllers
{
    [ApiController]
    [Route("api/user")]

    public class AccountController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly Token token;
        private readonly ILogger<AccountController> logger;
        private List<string> errors;


        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IMapper mapper,
            ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.mapper = mapper;
            this.logger = logger;
            this.token = new Token(configuration, userManager);

        }


        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserToken>> Create([FromBody] NewUserDTO model)
        {
            try
            {

                UserInfo userInfo = mapper.Map<UserInfo>(model);

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);
            //    await userManager.AddClaimAsync(user, new Claim("IsAdmin", "True"));

                if (result.Succeeded)
                {
                    return await token.BuildTokenAsync(userInfo);
                }
                else
                {
                    errors = new();
                    foreach (var error in result.Errors)
                    {
                        errors.Add(error.Description);
                    }
                    return BadRequest(errors);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);

                return BadRequest(ex.Message);

            }

        }



        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("Login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserDTO userDTO)
        {
            try
            {
                UserInfo userInfo = mapper.Map<UserInfo>(userDTO);

                Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(userInfo.Email, userInfo.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return await token.BuildTokenAsync(userInfo);
                }
                else if (result.IsLockedOut)
                {
                    return BadRequest("User is locked.");
                }
                else if (result.IsNotAllowed)
                {
                    return BadRequest("User is not allowed to singIn.");
                }
                else if (result.RequiresTwoFactor)
                {
                    ApplicationUser user = await userManager.FindByEmailAsync(userInfo.Email);

                    var token2FA = await userManager.GenerateTwoFactorTokenAsync(user, "Email");







                    return Ok("Requires two factor authentication.");
                }
                else
                {
                    return BadRequest("Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);

                return BadRequest(ex.Message);
            }
        }


        //TODO: accept a list of roles and status
        [HttpPost("SetRol")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> SetRoles([FromBody] UserRolDTO userDTO)
        {
            try
            {
                ApplicationUser user = await userManager.FindByEmailAsync(userDTO.Email);
                if (user != null)
                {
                    if (userDTO.Status)
                    {
                        await userManager.AddClaimAsync(user, new Claim(userDTO.Rol, userDTO.Status.ToString()));
                    }
                    else
                    {
                        await userManager.RemoveClaimAsync(user, new Claim(userDTO.Rol, userDTO.Status.ToString()));
                    }

                    return NoContent();
                }
                else
                {
                    return BadRequest("we coudn't find the user, to assign the rol");
                }

            }
            catch (Exception ex)
            {

                logger.LogError(ex.Message);

                return BadRequest(ex.Message);
            }

        }
        



        [HttpGet("GetUserLoggedData")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetUserLoggedData()
        {
            try
            {
                var userClaim = HttpContext.User.Claims.Where(claim => claim.Type.Contains("email")).FirstOrDefault();

                var email = userClaim.Value;
                ApplicationUser usuario = await userManager.FindByEmailAsync(email);

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);

                return BadRequest(ex.Message);
            }


        }

    }
}
