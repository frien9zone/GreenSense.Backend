using System;
using System.Collections.Generic;
using System.Text;

namespace GreenSense.Backend.Data.Entities
{
    public class User
    {
        public int UserId { get; set; }

        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public ICollection<Plant> Plants { get; set; } = new List<Plant>();
    }
}
