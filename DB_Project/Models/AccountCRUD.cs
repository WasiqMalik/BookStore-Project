using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DB_Project.Models
{
    public class AccountCRUD
    {
        public static string ConnectionString = "data source=PAVILION14-BF1X; database=BookStore; integrated security = SSPI;";
        //static string ConnectionString = "data source=DESKTOP-QGDLCC0; database=BookStore; integrated security = SSPI;";

        public static List<Account> GetAllUsers()
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                SqlCommand cmd = new SqlCommand();
                DataTable sqlUsers = new DataTable(); //stores IDs from db
                List<Account> Users = new List<Account>(); //store objects for all items from db          
                SqlDataAdapter Data = new SqlDataAdapter("Select UserID From [User]", ServerConnection);
                Data.Fill(sqlUsers);

                foreach (DataRow row in sqlUsers.Rows)
                {
                    Account acc = GetAccount((int)row["UserID"]);
                    if (acc != null)
                        Users.Add(acc);
                }
                ServerConnection.Close();

                return Users;
            }
        }

        public static Account GetAccount(int id)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();
                SqlCommand cmd = new SqlCommand();
                //calling login procedure from db
                cmd.CommandText = "GetUser";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@uid", id));

                //passing output variables to procedure
                cmd.Parameters.Add(new SqlParameter("@uname", SqlDbType.VarChar, 30));
                cmd.Parameters["@uname"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@gen", SqlDbType.Char,1));
                cmd.Parameters["@gen"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@conta", SqlDbType.Char, 13));
                cmd.Parameters["@conta"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@Address", SqlDbType.VarChar, 50));
                cmd.Parameters["@Address"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@access", SqlDbType.VarChar, 5));
                cmd.Parameters["@access"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@dJoined", SqlDbType.Date));
                cmd.Parameters["@dJoined"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@email", SqlDbType.VarChar, 30));
                cmd.Parameters["@email"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();  //run procedure


                //get output values from procedure
                int Flag = (int)cmd.Parameters["@flag"].Value;
                ServerConnection.Close();

                if (Flag == 1)
                {
                    Account retAcc = new Account();
                    retAcc.UserID = id;
                    retAcc.Username = (string)cmd.Parameters["@uname"].Value;
                    retAcc.AccStatus = (string)cmd.Parameters["@access"].Value;
                    retAcc.Gender = Convert.ToChar(cmd.Parameters["@gen"].Value);
                    retAcc.ContactNo = (string)cmd.Parameters["@conta"].Value;
                    retAcc.Address = (string)cmd.Parameters["@Address"].Value;
                    retAcc.DateJoined = Convert.ToString(cmd.Parameters["@dJoined"].Value);
                    retAcc.Email = (string)cmd.Parameters["@email"].Value;

                    return retAcc;
                }
                else
                    return null;
            }
        }

        public static Account UserLogin(string email, string password, ref bool validity)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();
                SqlCommand cmd = new SqlCommand();
                //calling login procedure from db
                cmd.CommandText = "LoginValidate";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@em", email));
                cmd.Parameters.Add(new SqlParameter("@pa", password));

                //passing output variables to procedure
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@uid", SqlDbType.Int));
                cmd.Parameters["@uid"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@acc_pr", SqlDbType.VarChar, 5));
                cmd.Parameters["@acc_pr"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@uname", SqlDbType.VarChar, 30));
                cmd.Parameters["@uname"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();  //run procedure

                Account retAcc = new Account();
                //get output values from procedure
                validity = (int)cmd.Parameters["@flag"].Value == 1;
                if (validity)
                {
                    retAcc.UserID = (int)cmd.Parameters["@uid"].Value;
                    retAcc.AccStatus = (string)cmd.Parameters["@acc_pr"].Value;
                    retAcc.Username = (string)cmd.Parameters["@uname"].Value;
                }
                ServerConnection.Close();

                return retAcc;
            }
        }

        public static bool RegisterUser(Account newUser)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();
                SqlCommand cmd = new SqlCommand();
                //calling signup procedure from db
                cmd.CommandText = "Signup";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@uname", newUser.Username));
                cmd.Parameters.Add(new SqlParameter("@gender", newUser.Gender));
                cmd.Parameters.Add(new SqlParameter("@cno", newUser.ContactNo));
                cmd.Parameters.Add(new SqlParameter("@sadd", newUser.Address));
                cmd.Parameters.Add(new SqlParameter("email", newUser.Email));
                cmd.Parameters.Add(new SqlParameter("@psw", newUser.Password));
                cmd.Parameters.Add(new SqlParameter("@acc_pr", "u"));
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery(); //run procedure

                int Flag = (int)cmd.Parameters["@flag"].Value;
                ServerConnection.Close();


                return Flag == 1;
            }
        }

        public static bool UpdateUser(Account user)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                //calling procedure from db
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "UpdateUser";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@uid", user.UserID));
                cmd.Parameters.Add(new SqlParameter("@uname", user.Username));
                cmd.Parameters.Add(new SqlParameter("@gen", user.Gender));
                cmd.Parameters.Add(new SqlParameter("@conta", user.ContactNo));
                cmd.Parameters.Add(new SqlParameter("@Address", user.Address));
                cmd.Parameters.Add(new SqlParameter("@access", user.AccStatus));
                cmd.Parameters.Add(new SqlParameter("@email", user.Email));

                //passing output para
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();  //run procedure

                int Flag = (int)cmd.Parameters["@flag"].Value;
                ServerConnection.Close();

                return Flag == 1;
            }
        }

        public static bool ChangePassword(int id, string newPass)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "UpdatePassword";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@uid", id));
                cmd.Parameters.Add(new SqlParameter("@newpass", newPass));

                //passing output para
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();  //run procedure

                int Flag = (int)cmd.Parameters["@flag"].Value;
                ServerConnection.Close();

                return Flag == 1;
            }
        }

        public static bool RemoveUser(int id)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "DeleteUser";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@uid", id));

                //passing output para
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();  //run procedure

                int Flag = (int)cmd.Parameters["@flag"].Value;
                ServerConnection.Close();

                return Flag == 1;
            }
        }
    }
}