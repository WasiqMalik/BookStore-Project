using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB_Project.Models
{
    public class Review
    {
        public int BookID { get; set; }
        public int UserID { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }
        public string DatePosted { get; set; }
        public string Status { get; set; }
    }
}