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
        public ActionResult Authenticate(Account acc)
        {
            SqlConnection ServerConnection = new SqlConnection(ConnectionString);
            ServerConnection.Open();
            
            //calling login procedure from db
            cmd.CommandText = "LoginValidate";
            cmd.Connection = ServerConnection;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            //passing parameters to procedure
            cmd.Parameters.Add(new SqlParameter("@em", acc.email));
            cmd.Parameters.Add(new SqlParameter("@pa", acc.password));

            //passing output variables to procedure
            cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
            cmd.Parameters["@flag"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add(new SqlParameter("@acc_pr", SqlDbType.VarChar, 5));
            cmd.Parameters["@acc_pr"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add(new SqlParameter("@uname", SqlDbType.VarChar, 30));
            cmd.Parameters["@uname"].Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();  //run procedure

            //get output values from procedure
            int Flag = (int)cmd.Parameters["@flag"].Value;
            string Priviledges = (string)cmd.Parameters["@acc_pr"].Value;
            string UserName = (string)cmd.Parameters["@uname"].Value;
            ServerConnection.Close();

            if (Flag == 1)
            {
                if (Priviledges == "a")
                    return RedirectToAction("Console", "Admin");
                else if (Priviledges == "u")
                    return RedirectToAction("DashBoard", "User");             
                else
                    return Content("<script>alert('No Assigned User Privledges.');window.location = 'Login'</script>");
            }
            else
                return Content("<script>alert('Incorrect Email or Password.');window.location = 'Login';</script>");   
        }

        public ActionResult Register(Register reg)
        {
            SqlConnection ServerConnection = new SqlConnection(ConnectionString);
            ServerConnection.Open();

            //calling signup procedure from db
            cmd.CommandText = "Signup";
            cmd.Connection = ServerConnection;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            //passing parameters to procedure
            cmd.Parameters.Add(new SqlParameter("@uname", reg.Username));
            cmd.Parameters.Add(new SqlParameter("@gender", reg.Gender));
            cmd.Parameters.Add(new SqlParameter("@cno", reg.ContactNo));
            cmd.Parameters.Add(new SqlParameter("@sadd", reg.Address));
            cmd.Parameters.Add(new SqlParameter("email", reg.Email));
            cmd.Parameters.Add(new SqlParameter("@psw", reg.Password));
            cmd.Parameters.Add(new SqlParameter("@acc_pr", "u"));
            cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
            cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery(); //run procedure

            int Flag = (int)cmd.Parameters["@flag"].Value;
            ServerConnection.Close();

            if (Flag == 1) 
                return Content("<script>alert('Account Registeration Successful.');window.location = 'Login';</script>");           
            else         
                return Content("<script>alert('Account Registeration Failed.');window.location = 'SignUp'</script>");
            
        }

    }
}