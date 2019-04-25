using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB_Project.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public List<KeyValuePair<int,int>> Items { get; set; }
    }
}