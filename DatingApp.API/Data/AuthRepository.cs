using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    /// <summary>
    /// Authentication 
    /// </summary>
    public class AuthRepository : IAuthRepository
    {

        #region Private Members

        private readonly DataContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="context"></param>
        public AuthRepository(DataContext context)
        {
            this.context = context;
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Authenticate user credentials 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<User> Login(string username, string password)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return null;

            if (VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt) == false)
                return null;

            return user;
        }


        /// <summary>
        /// Register a new user in the dating site DB
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<User> Register(User user, string password)
        {
            // The password has and salt as byte array
            byte[] passwordHash, passwordSalt;

            // Calculate and generate passwordhash/salt
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            // Set new users password details
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // Add to context
            await context.Users.AddAsync(user);

            // Persist
            await context.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Check if a user exists in the Db
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<bool> UserExists(string username)
        {
            if (await context.Users.AnyAsync(u => u.Username == username))
                return true;
            else
                return false;
        }

        #endregion


        #region Help Methods

        /// <summary>
        /// Verify password in Db
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        /// <returns></returns>
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            // Hash class initialized with the salt value
            using var hmac = new HMACSHA512(passwordSalt);

            // generate hash code from user input 
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Iterate hash values
            for (int i = 0; i < computedHash.Length; i++)
            {
                // Compare each index
                if (computedHash[i] != passwordHash[i])
                    // return false if key values dont match
                    return false;
            }

            // Authentication succeeded
            return true;
        }

        /// <summary>
        /// Create password hash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // Get instance to hash and salt password
            using var hmac = new HMACSHA512();

            // Assign randomly generated key as the salt value
            passwordSalt = hmac.Key;

            // Create password hash
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        #endregion

    }//end class
}
