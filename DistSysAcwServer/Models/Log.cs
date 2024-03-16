using System;
using System.ComponentModel.DataAnnotations;

namespace DistSysAcwServer.Models
{
    public class Log
    {
        [Key]
        public int LogId { get; set; }
        public string LogString { get; set; }
        public DateTime LogDateTime { get; set; }
        // Foreign key property
        public string UserApiKey { get; set; }

        // Navigation property
        public User User { get; set; }
    }
}
