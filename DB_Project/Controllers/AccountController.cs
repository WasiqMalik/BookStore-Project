using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using DB_Project.Models;

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
            bool flag=false;
            Account UserAcc = AccountCRUD.UserLogin(email, password, ref flag);

            if (flag)
            {
                Session["UserID"] = UserAcc.UserID;
                Session["UserName"] = UserAcc.Username;
                Session["Priviledges"] = UserAcc.AccStatus;
                Session["OrderItems"] = new List<Tuple<int, int, int>>();

                if (UserAcc.AccStatus == "a")
                    return RedirectToAction("Console", "Admin");
                else if (UserAcc.AccStatus == "u")
                    return RedirectToAction("DashBoard", "User");             
                else
                    return Content("<script>alert('No Assigned User Privledges.');window.location = 'Login'</script>");
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