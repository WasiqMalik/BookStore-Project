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
    public class OrderCRUD
    {
        public static string ConnectionString = "data source=PAVILION14-BF1X; database=BookStore; integrated security = SSPI;";

        public static List<Order> GetAllOrders()
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                SqlCommand cmd = new SqlCommand();
                DataTable orderIDs = new DataTable(); //stores IDs of all books in db
                List<Order> OrdersList = new List<Order>(); //store books objects for all books in db            
                SqlDataAdapter Data = new SqlDataAdapter("Select OrderID From [Orders]", ServerConnection);
                Data.Fill(orderIDs);

                //using procedure that returns book info for one book
                cmd.CommandText = "GetOrder";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //output parameters
                cmd.Parameters.Add(new SqlParameter("@user", SqlDbType.Int));
                cmd.Parameters["@user"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime));
                cmd.Parameters["@date"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@status", SqlDbType.VarChar, 10));
                cmd.Parameters["@status"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@items", SqlDbType.Structured));
                cmd.Parameters["@items"].Direction = ParameterDirection.Output;               
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                foreach (DataRow row in orderIDs.Rows)
                {
                    Order getOrder = new Order();
                    getOrder.OrderID = (int)row["OrderID"];

                    //input para
                    cmd.Parameters.Add(new SqlParameter("@orderID", getOrder.OrderID));

                    cmd.ExecuteNonQuery();  //run procedure

                    int Flag = (int)cmd.Parameters["@flag"].Value;  //check if required book id retrieved 

                    if (Flag == 1)
                    {
                        //intializing book obj 
                        getOrder.UserID = (int) cmd.Parameters["@user"].Value;
                        getOrder.Date = (string) cmd.Parameters["@date"].Value;
                        getOrder.OrderStatus = (string)cmd.Parameters["@status"].Value;
                        getOrder.Items = DataTabletoListConverter.TableToList<KeyValuePair<int, int>>((DataTable)cmd.Parameters["@items"].Value);

                        OrdersList.Add(getOrder);
                    }

                }

                ServerConnection.Close();

                return OrdersList;
            }

        }

        public static bool CreateOrder(Order newOrder)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();
                
                //calling procedure from db
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "placeOrder";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@uid", newOrder.UserID));
                
                //passing table paras
                cmd.Parameters.Add(new SqlParameter("@ITEMTABLE", ListtoDataTableConverter.ListToDataTable<KeyValuePair<int,int>>(newOrder.Items)));
                cmd.Parameters["@ITEMTABLE"].SqlDbType = SqlDbType.Structured;               

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