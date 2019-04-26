using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB_Project.Models
{
    public class Request
    {
        public int RequestID { get; set; }
        public int UserID { get; set; }
        public string Date { get; set; }
        public string RequestStatus { get; set; }
        public string Description { get; set; }
    }
}