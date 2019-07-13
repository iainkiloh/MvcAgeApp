using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class LoginAttempt
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime LoginAttemptTime { get; set; }
        public bool LoginSuccess { get; set; }


    }
}
