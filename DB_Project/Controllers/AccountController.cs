using DB_Project.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DB_Project.Controllers
{
    public class AccountController : Controller
    {

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authenticate(string email, string password)
        {
            Account UserAcc = AccountCRUD.UserLogin(email, password);

            if (UserAcc != null)
            {
                Session["UserID"] = UserAcc.UserID;
                Session["UserName"] = UserAcc.Username;
                Session["Priviledges"] = UserAcc.AccStatus;
                Session["OrderItems"] = new List<Tuple<int, int, int>>();

                return RedirectToAction(UserAcc.AccStatus == "Admin" ? "Console" : "DashBoard", UserAcc.AccStatus);
            }
            else
                return Content("<script>alert('Incorrect Email or Password.');window.location = 'Login';</script>");
        }

        public ActionResult Logout()
        {
            Session["UserID"] = null;
            Session["UserName"] = null;
            Session["Priviledges"] = null;
            Session["OrderItems"] = null;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register(Account reg)
        {
            if (AccountCRUD.RegisterUser(reg))
                return Content("<script>alert('Account Registeration Successful.');window.location = 'Login';</script>");
            else
                return Content("<script>alert('Account Registeration Failed.');window.location = 'SignUp'</script>");
        }
    }
}