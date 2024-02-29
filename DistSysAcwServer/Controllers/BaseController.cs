using Microsoft.AspNetCore.Mvc;

namespace DistSysAcwServer.Controllers
{
    [Route("api/[Controller]/[Action]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// This DbContext contains the database context defined in UserContext.cs
        /// You can use it inside your controllers to perform database CRUD functionality
        /// </summary>
        protected Models.UserContext DbContext { get; set; }
        /// <summary>
        /// Abstract base controller containing a dependency-injected scoped DbContext (UserContext).
        /// Inherit from this abstract base to access the common DbContext in a controller.
        /// </summary>
        /// <param name="dbcontext">Dependency-injected DbContext</param>
        public BaseController(Models.UserContext dbcontext)
        {
            DbContext = dbcontext;
        }
    }
}