using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DB_Project.Models;

namespace DB_Project.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult DashBoard()
        {
            return View();
        }

        public ActionResult Books()
        {
            return View();
        }

        public ActionResult BooksList()
        {
            return View("~/Views/User/Books.cshtml", BookCRUD.GetAllBooks());
        }

        public ActionResult SearchByTitle(string svalue)
        {
            return View("~/Views/User/DashBoard.cshtml", BookCRUD.TitleSearch(svalue));
        }

        

    }
}