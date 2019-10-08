using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Dtos;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class AuthController : ControllerBase
    {
        #region Private members

        private readonly IAuthRepository repo;
        private readonly IConfiguration configuration;

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="repo"></param>
        public AuthController(IAuthRepository repo, IConfiguration configuration)
        {
            this.repo = repo;
            this.configuration = configuration;
        }

        #endregion

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserModel registerUserModel )
        {
            // TODO : Validate request

            // Convert username to lower case
            registerUserModel.Username = registerUserModel.Username.ToLower();

            // Check if user exists
            if (await repo.UserExists(registerUserModel.Username))
                // return username already exists message
                return BadRequest("Username already exists");

            // Create new user object            
            var userToCreate = new User
            {
                Username = registerUserModel.Username
            };

            // Save user to database
            var createdUser = await repo.Register(userToCreate, registerUserModel.Password);

            return StatusCode(201);
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUserModel loginUserModel)
        {
            // Attempt to log user in
            var userFromDb = await repo.Login(loginUserModel.Username.ToLower(), loginUserModel.Password.ToLower());

            // If return value is null, login failed
            if (userFromDb == null)
                // Avoid giving users hints as to if username exists or password is incorrect, return Unauthorized...
                return Unauthorized();

            // Start building auth token
            // Create claims 
            var claims = new[]
            {
                // Database id claim
                new Claim(ClaimTypes.NameIdentifier, userFromDb.Id.ToString()),

                // Username claim
                new Claim(ClaimTypes.Name, userFromDb.Username)
            };

            // Create security key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value));

            // Create signing credentials to validate the token using encrypted security key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Token descriptor used to create the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // Handler used to create token based on the token descripter
            var tokenHandler = new JwtSecurityTokenHandler();

            // Create actual token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // return token and OK status
            return Ok(new { token = tokenHandler.WriteToken(token) });
        }

    }//end class
}
