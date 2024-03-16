using System.ComponentModel.DataAnnotations;

namespace DistSysAcwServer.Models
{
    public class LogArchive
    {
        [Key]
        public int LogId { get; set; } 
        public string LogString { get; set; }
        public DateTime LogDateTime { get; set; }
        public string UserApiKey { get; set; } // Store the ApiKey of the deleted user for reference
    }
}
