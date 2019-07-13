using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcAgeApp.Models
{
    public class LoginAttemptViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime LoginAttemptTime { get; set; }
        public bool LoginSuccess { get; set; }
    }
}
