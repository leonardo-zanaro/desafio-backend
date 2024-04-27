using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
public class AuthController : MainController
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<AuthController> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthController(
        UserManager<User> userManager,
        IConfiguration configuration,
        SignInManager<User> signInManager,
        ILogger<AuthController> logger,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _configuration = configuration;
        _signInManager = signInManager;
        _logger = logger;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Logs in a user with the provided username and password.
    /// </summary>
    /// <param name="userInfo">The login information containing the username and password.</param>
    /// <returns>Returns a UserToken object containing the generated token and its expiration date. Returns a BadRequest response if the login is invalid.</returns>
    [HttpPost]
    [Route("user/login")]
    public async Task<ActionResult<UserToken>> Login(UserInfo.Login userInfo)
    {
        try
        {
            var result = await _signInManager.PasswordSignInAsync(userInfo.Username, userInfo.Password,
                isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var resultToken = await BuildToken(userInfo);
                
                if (resultToken != null)
                    return resultToken;

                return BadRequest("Error creating user token");
            }
            else
            {
                return BadRequest("Invalid login or password");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Creates a new user with the provided registration information.
    /// </summary>
    /// <param name="model">The registration information containing the new user's username, password, email, and admin status.</param>
    /// <returns>Returns a UserToken object containing the generated token and its expiration date if the user registration is successful. Returns a BadRequest response if the registration fails.</returns>
    [HttpPost]
    [Route("user/create")]
    public async Task<ActionResult<UserToken>> CreateUser(UserInfo.Register model)
    {
        try
        {
            var user = new User
            {
                UserName = model.Username,
                Email = model.Email.ToUpper(),
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                
                if (model.IsAdmin)
                {
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    }
                    
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                
                var userLogin = new UserInfo.Login
                {
                    Username = model.Username,
                    Password = model.Password
                };

                var resultToken = await BuildToken(userLogin);
                
                if (resultToken != null)
                    return resultToken;

                return BadRequest("Error creating user token");
            }
            else
            {
                return BadRequest("Error registering");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Builds a token for the provided user login information.
    /// </summary>
    /// <param name="userInfo">The user login information containing the username and password.</param>
    /// <returns>Returns a UserToken object containing the generated token and its expiration date. Returns null if the user is not found or an error occurs during token generation.</returns>
    private async Task<UserToken?> BuildToken(UserInfo.Login userInfo)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(userInfo.Username);

            if (user == null)
                return null;

            var handler = new JwtSecurityTokenHandler();

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["jwt:secretKey"] ?? string.Empty));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            DateTime currentTimeUtc = DateTime.UtcNow;

            var expiration = currentTimeUtc.AddHours(8);

            var claimsUser = await GenerateClaims(user);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = claimsUser,
                SigningCredentials = creds,
                Expires = DateTime.UtcNow.AddHours(4)
            };

            var token = handler.CreateToken(tokenDescription);

            return new UserToken()
            {
                Token = handler.WriteToken(token),
                Expiration = expiration
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Generates a ClaimsIdentity for a User object.
    /// </summary>
    /// <param name="user">The User object for which to generate the ClaimsIdentity.</param>
    /// <returns>The ClaimsIdentity object containing the user's claims.</returns>
    private async Task<ClaimsIdentity> GenerateClaims(User user)
    {
        var ci = new ClaimsIdentity();
        if (user.UserName != null)
            ci.AddClaim(new Claim(ClaimTypes.Name, user.UserName));

        if (user.Email != null)
            ci.AddClaim(new Claim(ClaimTypes.Email, user.Email));

        ci.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            ci.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        return ci;
    }
}