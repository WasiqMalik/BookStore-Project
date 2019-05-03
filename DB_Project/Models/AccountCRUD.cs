using System.Data;
using System.Data.SqlClient;

namespace DB_Project.Models
{
    public class AccountCRUD
    {
        public static string ConnectionString = "data source=PAVILION14-BF1X; database=BookStore; integrated security = SSPI;";

        public static List<Account> GetAllUsers()
        {

        }

        public static Account GetAccount(int id)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();
                SqlCommand cmd = new SqlCommand();
                //calling login procedure from db
                cmd.CommandText = "GetUserInfo";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@uid", id));

                //passing output variables to procedure
                cmd.Parameters.Add(new SqlParameter("@uname", SqlDbType.Int));
                cmd.Parameters["@uname"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@gender", SqlDbType.Char));
                cmd.Parameters["@gender"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@cno", SqlDbType.Char, 13));
                cmd.Parameters["@cno"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@add", SqlDbType.VarChar, 30));
                cmd.Parameters["@add"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@acc_pr", SqlDbType.VarChar, 5));
                cmd.Parameters["@acc_pr"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();  //run procedure

                Account retAcc = new Account();
                //get output values from procedure
                int Flag = (int)cmd.Parameters["@flag"].Value;
                if (Flag == 1)
                {
                    retAcc.UserID = (int)cmd.Parameters["@uid"].Value;
                    retAcc.Username = (string)cmd.Parameters["@uname"].Value;
                    retAcc.AccStatus = (string)cmd.Parameters["@acc_pr"].Value;
                    retAcc.Gender = (char)cmd.Parameters["@gender"].Value;
                    retAcc.ContactNo = (string)cmd.Parameters["@cno"].Value;
                    retAcc.Address = (string)cmd.Parameters["@add"].Value;
                }
                ServerConnection.Close();

                return retAcc;
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
                cmd.Parameters.Add(new SqlParameter("@id", id));

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