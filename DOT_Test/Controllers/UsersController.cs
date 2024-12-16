using DOT_Test.Data;
using DOT_Test.Libraries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Globalization;
using DOT_Test.ApiModel;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using NuGet.Common;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace DOT_Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHelper _helper;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        
        private IConfiguration _config;
        

        public UsersController(ApplicationDbContext context, IHelper helper, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _context = context;
            _helper = helper;
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
        }

        private async Task<ApplicationUser> Authenticate(ApiModel.Login login)
        {
            if (ModelState.IsValid)
            {
                var users = await _userManager.FindByEmailAsync(login.Email);
                users = await _context.ApplicationUser.FirstOrDefaultAsync(m => m.Id == users.Id);
                //if (users != null)
                //{
                //    if (!await _userManager.IsEmailConfirmedAsync(users))
                //    {
                //        throw new Exception("Email belum dikonfirmasi, silahkan hubungi administrator!");
                //    }
                //}
                if (users == null)
                {
                    throw new Exception("Email tidak ditemukan, silahkan ulangi!");
                }
                var result = await _signInManager.CheckPasswordSignInAsync(users, login.Password, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return users;
                }
                else
                {
                    throw new Exception("Password salah, silahkan ulangi!");
                }
            }
            return null;
        }

        private string BuildToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["TokenProviderOptions:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["TokenProviderOptions:Issuer"],
                audience: _config["TokenProviderOptions:Audience"],
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds,
                claims: claims
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // POST: api/Login
        [HttpPost("Login")]
        [ProducesResponseType(typeof(ApiModel.Token), 200)]
        [AllowAnonymous]
        public async Task<IActionResult> PostLoginNew([FromBody] ApiModel.Login login)
        {

            var result = new ApiModel.Token();
            var resultString = "";
            var status = "";
            try
            {

                var user = await Authenticate(login);

                if (user != null)
                {
                    status = "Success";
                    var tokenString = BuildToken(user);

                    result = new ApiModel.Token
                    {
                        Value = tokenString,
                        User = user,

                    };

                }
                else
                {
                    status = "Failed";

                    if (user == null)
                    {
                        resultString = "Email atau Password salah. Silahkan coba lagi";
                    }
                }

            }
            catch (Exception e)
            {
                status = "Failed";

                return Ok(new { Status = status, Result = resultString, result.Value, result.User });
            }

            return Ok(new { Status = status, Result = resultString, result.Value, result.User });

        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //if (!await _roleManager.RoleExistsAsync("Guest"))
                    //{
                    //    await _roleManager.CreateAsync(new IdentityRole("Guest"));
                    //}

                    var success = await _userManager.AddToRoleAsync(user, "Guest");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    
                    await _context.SaveChangesAsync();

                    return Ok(new { Status = "Success", Result = "Berhasil register" });
                }
                else
                {
                    return Ok(new { Status = "Failed", Result = result });
                }
            }
            
            string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
            return Ok(new { Status = "Failed", Result = messages });
        }
    }
}
