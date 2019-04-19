using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using DB_Project.Models;

namespace DB_Project.Controllers
{
    public class BookController : Controller
    {
        SqlCommand cmd = new SqlCommand();
        string ConnectionString = "data source=PAVILION14-BF1X; database=BookStore; integrated security = SSPI;";

        // GET: All Books
        public ActionResult Index()
        {
            DataTable sqlBooks = new DataTable(); //stores IDs of all books in db
            List<Book> allBooks = new List<Book>(); //store books objects for all books in db

            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();
                SqlDataAdapter Data = new SqlDataAdapter("Select ItemID From [Books]", ServerConnection);
                Data.Fill(sqlBooks);

                //using procedure that returns book info for one book
                cmd.CommandText = "GetBook";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //output parameters
                cmd.Parameters.Add(new SqlParameter("@title", SqlDbType.VarChar, 30));
                cmd.Parameters["@title"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@synp", SqlDbType.VarChar, 500));
                cmd.Parameters["@synp"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@pub", SqlDbType.VarChar, 20));
                cmd.Parameters["@pub"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@cat", SqlDbType.VarChar, 10));
                cmd.Parameters["@cat"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@price", SqlDbType.SmallMoney));
                cmd.Parameters["@price"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@stock", SqlDbType.Int));
                cmd.Parameters["@stock"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@sub", SqlDbType.Bit));
                cmd.Parameters["@sub"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@auth", SqlDbType.VarChar, 500));
                cmd.Parameters["@auth"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@gen", SqlDbType.VarChar, 500));
                cmd.Parameters["@gen"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                foreach (DataRow row in sqlBooks.Rows)
                {
                    Book getBook = new Book();
                    getBook.BookID = (int)row["ItemID"];

                    //input para
                    cmd.Parameters.Add(new SqlParameter("@item_id", getBook.BookID));

                    cmd.ExecuteNonQuery();  //run procedure

                    int Flag = (int)cmd.Parameters["@flag"].Value;  //check if required book id retrieved 

                    if (Flag == 1)
                    {
                        //intializing book obj 
                        getBook.Title = (string)cmd.Parameters["title"].Value;
                        getBook.Synopsis = (string)cmd.Parameters["synp"].Value;
                        getBook.Publisher = (string)cmd.Parameters["pub"].Value;
                        getBook.Category = (string)cmd.Parameters["cat"].Value;
                        getBook.Price = (int)cmd.Parameters["price"].Value;
                        getBook.Stock = (int)cmd.Parameters["stock"].Value;
                        getBook.SubStatus = (char)cmd.Parameters["sub"].Value;
                        getBook.Authors = ((string)cmd.Parameters["auth"].Value).Split(',').ToList<string>();
                        getBook.Genres = ((string)cmd.Parameters["gen"].Value).Split(',').ToList<string>();

                        allBooks.Add(getBook);
                    }

                }

                ServerConnection.Close();
            }

            return View(allBooks);
        }

        // GET: Book/Details/1
        public ActionResult Details(int id)
        {
            
            using (SqlConnection Server = new SqlConnection(ConnectionString))
            {
                Server.Open();

                cmd.CommandText = "GetBook";
                cmd.Connection = Server;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //output parameters
                cmd.Parameters.Add(new SqlParameter("@title", SqlDbType.VarChar, 30));
                cmd.Parameters["@title"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@synp", SqlDbType.VarChar, 500));
                cmd.Parameters["@synp"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@pub", SqlDbType.VarChar, 20));
                cmd.Parameters["@pub"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@cat", SqlDbType.VarChar, 10));
                cmd.Parameters["@cat"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@price", SqlDbType.SmallMoney));
                cmd.Parameters["@price"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@stock", SqlDbType.Int));
                cmd.Parameters["@stock"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@sub", SqlDbType.Bit));
                cmd.Parameters["@sub"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@auth", SqlDbType.VarChar, 500));
                cmd.Parameters["@auth"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@gen", SqlDbType.VarChar, 500));
                cmd.Parameters["@gen"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                //input para
                cmd.Parameters.Add(new SqlParameter("@item_id", id));

                cmd.ExecuteNonQuery();  //run procedure

                int Flag = (int)cmd.Parameters["@flag"].Value;  //check if required book id retrieved 

                if (Flag == 1)
                {
                    Book getBook = new Book();

                    //intializing book obj 
                    getBook.BookID = id;
                    getBook.Title = (string)cmd.Parameters["title"].Value;
                    getBook.Synopsis = (string)cmd.Parameters["synp"].Value;
                    getBook.Publisher = (string)cmd.Parameters["pub"].Value;
                    getBook.Category = (string)cmd.Parameters["cat"].Value;
                    getBook.Price = (int)cmd.Parameters["price"].Value;
                    getBook.Stock = (int)cmd.Parameters["stock"].Value;
                    getBook.SubStatus = (char)cmd.Parameters["sub"].Value;
                    getBook.Authors = ((string)cmd.Parameters["auth"].Value).Split(',').ToList<string>();
                    getBook.Genres = ((string)cmd.Parameters["gen"].Value).Split(',').ToList<string>();

                    Server.Close();

                    return View(getBook);
                }
                else
                    return Content("<script>alert('Book not found.');window.location = 'Index'</script>");
            }
         
        }

        // POST: Book/Create
        [HttpPost] 
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                Book newBook = new Book();

                newBook.Title = collection["Title"];
                newBook.Synopsis = collection["Synopsis"];
                newBook.Publisher = collection["Publisher"];
                newBook.Category = collection["Category"];
                newBook.Price = Int32.Parse(collection["Price"]);
                newBook.Stock = Int32.Parse(collection["Stock"]);
                newBook.SubStatus = collection["SubStatus"][0];
                newBook.Authors = collection["Authors"].Split(',').ToList();
                newBook.Genres = collection["Genres"].Split(',').ToList();

                using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
                {
                    ServerConnection.Open();

                    //calling procedure from db
                    cmd.CommandText = "InsertBook";
                    cmd.Connection = ServerConnection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //passing parameters to procedure
                    cmd.Parameters.Add(new SqlParameter("@title", newBook.Title));
                    cmd.Parameters.Add(new SqlParameter("@synp", newBook.Synopsis));
                    cmd.Parameters.Add(new SqlParameter("@pub", newBook.Publisher));
                    cmd.Parameters.Add(new SqlParameter("@cat", newBook.Category));
                    cmd.Parameters.Add(new SqlParameter("@price", newBook.Price));
                    cmd.Parameters.Add(new SqlParameter("@stock", newBook.Stock));
                    cmd.Parameters.Add(new SqlParameter("@sub", newBook.SubStatus));

                    //passing table paras
                    cmd.Parameters.Add(new SqlParameter("@auth", ListtoDataTableConverter.ListToDataTable<string>(newBook.Authors)));
                    cmd.Parameters["@auth"].SqlDbType = SqlDbType.Structured;
                    cmd.Parameters.Add(new SqlParameter("@gen", ListtoDataTableConverter.ListToDataTable<string>(newBook.Genres)));
                    cmd.Parameters["@gen"].SqlDbType = SqlDbType.Structured;

                    //passing output para
                    cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                    cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();  //run procedure

                    int Flag = (int)cmd.Parameters["@flag"].Value;
                    ServerConnection.Close();

                    if (Flag == 1)
                        return Content("<script>alert('Book has been added Successfully.');window.location = 'Index';</script>");
                    else
                        return Content("<script>alert('Book could not be added.');window.location = 'Index'</script>");
                }
            }
            catch
            {
                return View();
            }
        }

        // POST: Book/Edit/price
        [HttpPost]
        public ActionResult EditPrice(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                int newPrice = Int32.Parse(collection["Price"]);

                using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
                {
                    ServerConnection.Open();

                    cmd.CommandText = "UpdatePrice";
                    cmd.Connection = ServerConnection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //passing parameters to procedure
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.Parameters.Add(new SqlParameter("@price", newPrice));

                    //passing output para
                    cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                    cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();  //run procedure

                    int Flag = (int)cmd.Parameters["@flag"].Value;
                    ServerConnection.Close();

                    if (Flag == 1)
                        return Content("<script>alert('Book Updated Successfully.');window.location = 'Index';</script>");
                    else
                        return Content("<script>alert('Book could not be found.');window.location = 'Index'</script>");

                }
            }
            catch
            {
                return View();
            }
        }

        // POST: Book/Edit/stock
        [HttpPost]
        public ActionResult EditStock(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                int newStock = Int32.Parse(collection["Stock"]);

                using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
                {
                    ServerConnection.Open();

                    cmd.CommandText = "UpdatePrice";
                    cmd.Connection = ServerConnection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //passing parameters to procedure
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.Parameters.Add(new SqlParameter("@stocks", newStock));

                    //passing output para
                    cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                    cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();  //run procedure

                    int Flag = (int)cmd.Parameters["@flag"].Value;
                    ServerConnection.Close();

                    if (Flag == 1)
                        return Content("<script>alert('Book Updated Successfully.');window.location = 'Index';</script>");
                    else
                        return Content("<script>alert('Book could not be found.');window.location = 'Index'</script>");

                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Book/Delete/5
        public ActionResult Delete(int id)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                cmd.CommandText = "RemoveBook";
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

                if (Flag == 1)
                    return Content("<script>alert('Book Deleted Successfully.');window.location = 'Index';</script>");
                else
                    return Content("<script>alert('Book could not be found.');window.location = 'Index'</script>");

            }
        }

        // POST: Book/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }

    public class ListtoDataTableConverter
    {
        public static DataTable ListToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }

    public class DataTabletoListConverter
    {
        public static List<T> TableToList<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        public static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
    }
}
