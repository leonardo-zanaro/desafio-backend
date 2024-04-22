using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

public class AuthController : MainController
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    
    public AuthController(UserManager<User> userManager, IConfiguration configuration, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _configuration = configuration;
        _signInManager = signInManager;
    }

    /// <summary>
    /// Logs a user in.
    /// </summary>
    /// <param name="userInfo">The user information.</param>
    /// <returns>The token generated for the logged-in user.</returns>
    /// <remarks>
    /// This method logs a user in using the provided user information. It calls the PasswordSignInAsync
    /// method of the SignInManager to verify the username and password. If the login is successful,
    /// it returns a token generated for the logged-in user. Otherwise, it returns a BadRequest response
    /// with an error message.
    /// </remarks>
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
                return await BuildToken(userInfo);
            }
            else
            {
                return BadRequest("Invalid login or password");
            }
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="model">The user information.</param>
    /// <returns>The token generated for the new user.</returns>
    /// <remarks>
    /// This method creates a new user using the provided user information. It creates a new User instance with
    /// the provided email and password, sets the user's role to "Common", and attempts to create the user in
    /// the user manager. If the user creation is successful, it returns a token generated for the new user.
    /// Otherwise, it returns a BadRequest response with an error message.
    /// </remarks>
    [HttpPost]
    [Route("user/create")]
    public async Task<ActionResult<UserToken>> CreateUser(UserInfo.Register model)
    {
        var user = new User
        {
            UserName = model.Email.ToLower().Split("@")[0],
            Email = model.Email.ToUpper(),
            EmailConfirmed = true,
        };
        
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            var userLogin = new UserInfo.Login
            {
                Username = model.Username,
                Password = model.Password
            };
            
            return await BuildToken(userLogin);
        }
        else
        {
            return BadRequest("Error registering");
        }
    }

    /// <summary>
    /// Retrieves the logged-in user.
    /// </summary>
    /// <returns>The logged-in user information.</returns>
    /// <exception cref="BadRequestResult">Thrown when the token has expired.</exception>
    [HttpPost]
    [Authorize]
    [Route("user/logged")]
    public async Task<ActionResult<object>> GetUserLogged()
    {
        var user = _userManager.GetUserAsync(User).Result;

        if (user == null)
            return BadRequest("Expired Token");
        
        var model = new
        {
            id = user.Id,
            username = user.UserName,
            email = user.Email?.ToLower(),
        };

        return Ok(model);
    }

    /// <summary>
    /// Builds a JWT token for the given user information.
    /// </summary>
    /// <param name="userInfo">The user information.</param>
    /// <returns>The built JWT token.</returns>
    private async Task<UserToken> BuildToken(UserInfo.Login userInfo)
    {
        var user = await _userManager.FindByNameAsync(userInfo.Username);
        
        var handler = new JwtSecurityTokenHandler();
        
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["jwt:secretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        DateTime currentTimeUTC = DateTime.UtcNow;

        TimeZoneInfo timeZoneBrasilia = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        DateTime hourBrasilia = TimeZoneInfo.ConvertTimeFromUtc(currentTimeUTC, timeZoneBrasilia);
        
        var expiration = hourBrasilia.AddHours(4);

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

    /// <summary>
    /// Generates claims for a given user.
    /// </summary>
    /// <param name="user">The user for whom the claims are generated.</param>
    /// <returns>A <see cref="ClaimsIdentity"/> object containing the generated claims.</returns>
    private async Task<ClaimsIdentity> GenerateClaims(User user)
    {
        var ci = new ClaimsIdentity();
        ci.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
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