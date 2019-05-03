using DB_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace DB_Project.Controllers
{
    public class AccountController : Controller
    {
        //Controller Members
        SqlCommand cmd = new SqlCommand();
        string ConnectionString = "data source=PAVILION14-BF1X; database=BookStore; integrated security = SSPI;";
        //string ConnectionString = "data source=DESKTOP-QGDLCC0; database=BookStore; integrated security = SSPI;";

        //Controller Methods
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

        public ActionResult Register(Account reg)
        {
            if (AccountCRUD.RegisterUser(reg))
                return Content("<script>alert('Account Registeration Successful.');window.location = 'Login';</script>");
            else
                return Content("<script>alert('Account Registeration Failed.');window.location = 'SignUp'</script>");
        }
    }
}