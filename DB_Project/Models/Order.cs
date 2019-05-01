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
        public int TotalCost { get; set; }
        public string Date { get; set; }
        public string OrderStatus { get; set; }
        public List<Tuple<int,int,int>> Items { get; set; }
    }
}