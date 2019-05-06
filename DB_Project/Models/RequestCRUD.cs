using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace DB_Project.Models
{
    public class RequestCRUD
    {
        public static string ConnectionString = "data source=PAVILION14-BF1X; database=BookStore; integrated security = SSPI;";
        //public static string ConnectionString = "data source=DESKTOP-QGDLCC0; database=BookStore; integrated security = SSPI;";

        public static bool CreateRequest(Request newRequest)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                //calling procedure from db
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "createRequest";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@uid", newRequest.UserID));
                cmd.Parameters.Add(new SqlParameter("@Descp", newRequest.Description));

                //passing output para
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();  //run procedure

                int Flag = (int)cmd.Parameters["@flag"].Value;
                ServerConnection.Close();

                return Flag == 1;

            }
        }

        public static bool UpdateRequest(int reqid, string rstat)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                //calling procedure from db
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "UpdateRequestStatus";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@requestID", reqid));
                cmd.Parameters.Add(new SqlParameter("@rstatus", rstat));

                //passing output para
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;


                cmd.ExecuteNonQuery();

                int Flag = (int)cmd.Parameters["@flag"].Value;
                ServerConnection.Close();

                return Flag == 1;
            }
        }

        public static List<Request> GetRequest(int userid)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                //calling procedure from db
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "getRequest";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@uid", userid));

                //passing output para
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;
                
                DataTable sqlRequests = new DataTable();                 //stores requests of a user or all requests for an admin
                List<Request> RequestsList = new List<Request>();        //store requests objects for all user's requests in db
                SqlDataAdapter Data = new SqlDataAdapter(cmd);
                Data.Fill(sqlRequests);

                
                foreach (DataRow row in sqlRequests.Rows)
                {
                    Request addrequest = new Request();
                    addrequest.RequestID = (int)row["ReqID"];
                    addrequest.UserID = (int)row["UserID"];
                    addrequest.Description = (string)row["Req_Description"];
                    addrequest.RequestStatus = (string)row["Request_Status"];
                    addrequest.Date = Convert.ToString(row["Request_Date"]);

                    RequestsList.Add(addrequest);
                }

                ServerConnection.Close();

                return RequestsList;

            }
        }

        public static bool DeleteRequest(int id)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                //calling procedure from db
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "RemoveRequest";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@rid" , id));

                //passing output para
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                int Flag = (int)cmd.Parameters["@flag"].Value;

                return Flag == 1;

            }
        }

    }

}