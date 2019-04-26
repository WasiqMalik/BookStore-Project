using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace DB_Project.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Synopsis { get; set; }
        public string Publisher { get; set; }
        public string Category { get; set; }
        public List<string> Authors { get; set; }
        public List<string> Genres { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; }
        public bool SubStatus { get; set; }  
    }
}