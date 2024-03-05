using System.Security.Cryptography;
using System.Text;
using DistSysAcwServer.Crypto;
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

        public ProtectedController(RsaCryptoService rsaProvider)
        {
            _rsaProvider = rsaProvider;
        }
        // GET api/protected/hello
        [HttpGet("hello")]
        public IActionResult Hello()
        {
            // Assuming that the user's username is stored as a claim
            var username = User.Identity?.Name ?? "Unknown";
            return Ok($"Hello {username}");
        }

        // GET api/protected/sha1?message={message}
        [HttpGet("sha1")]
        public IActionResult GetSHA1([FromQuery] string message)
        {
            if (string.IsNullOrEmpty(message))
            {
               
                return BadRequest("Bad Request");
            }

            using (var sha1 = SHA1.Create())
            {
                var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(message));
                var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
                return Ok(hash);
            }
        }

        // GET api/protected/sha256?message={message}
        [HttpGet("sha256")]
        public IActionResult GetSHA256([FromQuery] string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("Bad Request");
            }

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));
                var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
                return Ok(hash);
            }
        }

        [HttpGet("getpublickey")]
        public IActionResult GetPublicKey()
        {
            // This should return the public key in XML format
            var publicKeyXml = _rsaProvider.GetPublicKey();
            return Ok(publicKeyXml);
        }


    }
}
