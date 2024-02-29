using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DistSysAcwServer.Models
{
    /// <summary>
    /// User data class
    /// </summary>
    public class User
    {
        #region Task2
        // TODO: Create a User Class for use with Entity Framework
        // Note that you can use the [key] attribute to set your ApiKey Guid as the primary key 
        // Empty constructor
        public User() { }

        // The ApiKey property serves as the unique identifier and primary key for the User entity.
        [Key]
        public string ApiKey { get; set; }

        // UserName property to store the username of the user.
        public string UserName { get; set; }

        // Role property to store the role of the user.
        public string Role { get; set; }
        #endregion
    }

    #region Task13?
    // TODO: You may find it useful to add code here for Logging
    #endregion


}