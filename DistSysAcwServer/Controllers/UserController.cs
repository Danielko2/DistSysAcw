using Microsoft.AspNetCore.Mvc;
using DistSysAcwServer.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DistSysAcwServer.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDataAccess _userDataAccess;

        public UserController(UserDataAccess userDataAccess)
        {
            _userDataAccess = userDataAccess;
        }

        // GET api/user/new
        [HttpGet("new")]
        public async Task<IActionResult> NewGet([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return Ok("False - User Does Not Exist! Did you mean to do a POST to create a new user?");
            }

            bool userExists = await _userDataAccess.CheckUsernameExistsAsync(username);
            return Ok(userExists ? "True - User Does Exist! Did you mean to do a POST to create a new user?" : "False - User Does Not Exist! Did you mean to do a POST to create a new user?");
        }

        // POST api/user/new
        [HttpPost("new")]
        public async Task<IActionResult> NewPost([FromBody] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json");
            }

            bool userExists = await _userDataAccess.CheckUsernameExistsAsync(username);
            if (userExists)
            {
                return StatusCode(403, "Oops. This username is already in use. Please try again with a new username.");
            }

            // Assume the first user to be created is an Admin, others are just Users.
            string role = !await _userDataAccess.CheckAnyUserExistsAsync() ? "Admin" : "User";
            var newUser = await _userDataAccess.CreateUserAsync(username, role);

            return Ok(newUser.ApiKey); // Return the new user's ApiKey
        }

        // DELETE api/user/removeuser?username={username}
        [HttpDelete("removeuser")]
        public async Task<IActionResult> RemoveUser([FromQuery] string username)
        {
            // Check if the ApiKey header is present in the request
            if (!Request.Headers.TryGetValue("ApiKey", out var apiKeyHeaderValues))
            {
                return Unauthorized("No API key provided.");
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            // Check if the username and ApiKey from the header belong to the same user
            bool userExists = await _userDataAccess.CheckUserExistsAsync(providedApiKey, username);

            if (!userExists)
            {
                // Return false and OK status as per the task requirement
                return Ok(false);
            }

            // Attempt to delete the user from the database
            bool deleted = await _userDataAccess.DeleteUserAsync(providedApiKey);

            // Return true if the user was deleted, false otherwise
            return Ok(deleted);
        }


        [Authorize(Roles = "Admin")] // Only Admin can access this endpoint
        [HttpPost("changerole")]
        public async Task<IActionResult> ChangeRole([FromBody] UserRoleChangeModel model)
        {
            try
            {
                // Ensure that a username and role are provided
                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Role))
                {
                    return BadRequest("NOT DONE: Username and role must be provided.");
                }

                // Ensure that the role is either "User" or "Admin"
                if (model.Role != "User" && model.Role != "Admin")
                {
                    return BadRequest("NOT DONE: Role does not exist.");
                }

                // Check if the user exists
                var user = await _userDataAccess.GetUserByUsernameAsync(model.Username);
                if (user == null)
                {
                    return BadRequest("NOT DONE: Username does not exist");
                }

                // Update the user's role
                bool result = await _userDataAccess.ChangeUserRoleAsync(user.ApiKey, model.Role);
                if (result)
                {
                    return Ok("DONE");
                }
                else
                {

                    return BadRequest("NOT DONE: An error occurred");
                }
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging purposes
                // _logger.LogError(ex, "An error occurred while changing the user role.");

                // Return a generic error message
                return BadRequest("NOT DONE: An error occurred");
            }
        }

        // Model for the ChangeRole endpoint
        public class UserRoleChangeModel
        {
            public string Username { get; set; }
            public string Role { get; set; }
        }

    }
}
