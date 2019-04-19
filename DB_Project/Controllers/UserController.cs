using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DB_Project.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult DashBoard()
        {
            return View();
        }
    }
}