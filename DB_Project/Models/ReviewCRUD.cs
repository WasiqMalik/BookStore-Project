using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using DB_Project.Models;

namespace DB_Project.Models
{
    public class ReviewCRUD
    {
        public static string ConnectionString = "data source=PAVILION14-BF1X; database=BookStore; integrated security = SSPI;";

        //methods
        public static bool CreateReview(Review newReview)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                //calling procedure from db
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "AddBookReview";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@bID", newReview.BookID));
                cmd.Parameters.Add(new SqlParameter("@user", newReview.UserID));
                cmd.Parameters.Add(new SqlParameter("@rate", newReview.Rating));
                cmd.Parameters.Add(new SqlParameter("@text", newReview.Description));

                //passing output para
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();  //run procedure

                int Flag = (int)cmd.Parameters["@flag"].Value;
                ServerConnection.Close();

                return Flag == 1;
            }
        }

        public static List<Review> GetReviews(int id)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                //calling procedure from db
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "GetBookReview";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@bid", id));

                //passing output para
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                DataTable bookReviews = new DataTable(); //stores IDs returned by db
                List<Review> ReviewList = new List<Review>(); //store objects returned from db          
                SqlDataAdapter Data = new SqlDataAdapter(cmd);
                Data.Fill(bookReviews);    //execute procedure

                int Flag = (int)cmd.Parameters["@flag"].Value;
                ServerConnection.Close();

                if(Flag==1) //if book found
                {
                    foreach (DataRow row in bookReviews.Rows)
                    {
                        Review getReview = new Review();
                        getReview.BookID = id;
                        getReview.UserID = (int)row["UserID"];
                        getReview.Rating = Convert.ToInt32(row["Rating"]);
                        getReview.UserName = (string)row["UserName"];
                        getReview.Description = (string)row["Review"];
                        getReview.DatePosted = Convert.ToString(row["Review_Date"]);

                        ReviewList.Add(getReview);
                    }       
                }

                return ReviewList;
            }
        }

        public static bool RemoveReview(int bID, int uID)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "RemoveReview";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@bid", bID));
                cmd.Parameters.Add(new SqlParameter("@uid", uID));

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