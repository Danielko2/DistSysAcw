using System.Security.Cryptography;
using System.Text;
using DistSysAcwServer.Crypto;
using DistSysAcwServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace DistSysAcwServer.Controllers
{
    [Authorize(Roles = "User,Admin")] // Ensure that only authorized users can access this controller
    [Route("api/protected")]
    [ApiController]
    public class ProtectedController : ControllerBase
    {
        private readonly RsaCryptoService _rsaProvider;
        private readonly UserDataAccess _userDataAccess;
        public ProtectedController(RsaCryptoService rsaProvider, UserDataAccess userDataAccess)
        {
            _rsaProvider = rsaProvider;
            _userDataAccess = userDataAccess;
        }
        // GET api/protected/hello
        [HttpGet("hello")]
        public async Task<IActionResult> Hello()
        {
            
            var username = User.Identity?.Name ?? "Unknown";
            // Log the action
            string userApiKey = User.Claims.FirstOrDefault(c => c.Type == "ApiKey")?.Value;
            if (userApiKey != null)
            {
                await _userDataAccess.LogActionAsync(userApiKey, "User requested /Protected/Hello");
            }
            return Ok($"Hello {username}");
        }

        // GET api/protected/sha1?message={message}
        [HttpGet("sha1")]
        public async Task<IActionResult> GetSHA1([FromQuery] string message)
        {
            if (string.IsNullOrEmpty(message))
            {
               
                return BadRequest("Bad Request");
            }

           
            using (var sha1 = SHA1.Create())
            {
                var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(message));
                var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
                // Log the action
                string userApiKey = User.Claims.FirstOrDefault(c => c.Type == "ApiKey")?.Value;
                if (userApiKey != null)
                {
                    await _userDataAccess.LogActionAsync(userApiKey, "User requested /Protected/SHA1");
                }
                return Ok(hash);
            }
        }

        // GET api/protected/sha256?message={message}
        [HttpGet("sha256")]
        public async Task<IActionResult> GetSHA256([FromQuery] string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("Bad Request");
            }

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));
                var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
                // Log the action
                string userApiKey = User.Claims.FirstOrDefault(c => c.Type == "ApiKey")?.Value;
                if (userApiKey != null)
                {
                    await _userDataAccess.LogActionAsync(userApiKey, "User requested /Protected/SHA256");
                }
                return Ok(hash);
            }
        }

        [HttpGet("getpublickey")]
        public async Task<IActionResult> GetPublicKey()
        {
            // This should return the public key in XML format
            var publicKeyXml = _rsaProvider.GetPublicKey();
            string userApiKey = User.Claims.FirstOrDefault(c => c.Type == "ApiKey")?.Value;

            // Log the action
            if (userApiKey != null)
            {
                await _userDataAccess.LogActionAsync(userApiKey, "User requested /Protected/GetPublicKey");
            }

            return Ok(publicKeyXml);
        }
        // GET api/protected/sign?message={message}
        [HttpGet("sign")]
        public async Task<IActionResult> SignMessage([FromQuery] string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("Bad Request");
            }

            try
            {
                // Sign the message
                byte[] signatureBytes = _rsaProvider.SignData(message);
                // Convert the signature to a hexadecimal string
                string signatureHex = BitConverter.ToString(signatureBytes).ToUpper(); // By default, BitConverter includes dashes

                // Log the action
                string userApiKey = User.Claims.FirstOrDefault(c => c.Type == "ApiKey")?.Value;
                if (userApiKey != null)
                {
                    await _userDataAccess.LogActionAsync(userApiKey, "User requested /Protected/Sign");
                }
                // Return the signature in the desired format
                return Ok(signatureHex);
            }
            catch (Exception ex)
            {
                // Log the exception details here
                return StatusCode(StatusCodes.Status500InternalServerError, "Error signing message");
            }
        }



    }
}
