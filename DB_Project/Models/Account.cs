using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB_Project.Models
{
    public class Account
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public string AccStatus { get; set; }
        public string DateJoined { get; set; }
        public char Gender { get; set; }
    }
}