using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Dtos
{
    /// <summary>
    /// Login model for the user entity
    /// </summary>
    public class LoginUserModel
    {
        /// <summary>
        ///  Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Hashed user password
        /// </summary>
        public string Password { get; set; }

    }//end class
}
