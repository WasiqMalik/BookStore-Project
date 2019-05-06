using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DB_Project.Models
{
    public class BookCRUD
    {
        static string ConnectionString = "data source=PAVILION14-BF1X; database=BookStore; integrated security = SSPI;";
        //static string ConnectionString = "data source=DESKTOP-QGDLCC0; database=BookStore; integrated security = SSPI;";

        //methods
        public static Book GetBook(int id)
        {

            using (SqlConnection Server = new SqlConnection(ConnectionString))
            {
                Server.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "GetBook";
                cmd.Connection = Server;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //input para
                cmd.Parameters.Add(new SqlParameter("@Itid", id));

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
                cmd.Parameters.Add(new SqlParameter("@disc", SqlDbType.Float));
                cmd.Parameters["@disc"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@authorStr", SqlDbType.VarChar, 500));
                cmd.Parameters["@authorStr"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@genreStr", SqlDbType.VarChar, 500));
                cmd.Parameters["@genreStr"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();  //run procedure

                int Flag = (int)cmd.Parameters["@flag"].Value;  //check if required book id retrieved 

                if (Flag == 1)
                {
                    Book getBook = new Book();

                    //call procedure from db
                    SqlCommand cmd2 = new SqlCommand();
                    cmd2.CommandText = "CalculateAverageRating";
                    cmd2.Connection = Server;
                    cmd2.CommandType = System.Data.CommandType.StoredProcedure;

                    //procedure paras
                    cmd2.Parameters.Add(new SqlParameter("@ITEMID", id));
                    cmd2.Parameters.Add(new SqlParameter("@avg", SqlDbType.Int));
                    cmd2.Parameters["@avg"].Direction = ParameterDirection.Output;
                    cmd2.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                    cmd2.Parameters["@flag"].Direction = ParameterDirection.Output;

                    cmd2.ExecuteNonQuery();  //run procedure

                    //intializing book obj 
                    getBook.BookID = id;
                    getBook.Title = (string)cmd.Parameters["@title"].Value;
                    getBook.Synopsis = (string)cmd.Parameters["@synp"].Value;
                    getBook.Publisher = (string)cmd.Parameters["@pub"].Value;
                    getBook.Category = (string)cmd.Parameters["@cat"].Value;
                    getBook.Price = Convert.ToInt32(cmd.Parameters["@price"].Value);
                    getBook.Stock = (int)cmd.Parameters["@stock"].Value;
                    getBook.Discount = (double)cmd.Parameters["@disc"].Value;
                    getBook.SubStatus = (bool)cmd.Parameters["@sub"].Value;
                    getBook.Authors = ((string)cmd.Parameters["@authorStr"].Value).Split(',').ToList<string>();
                    getBook.Genres = ((string)cmd.Parameters["@genreStr"].Value).Split(',').ToList<string>();
                    getBook.AverageRating = Convert.ToInt32(cmd2.Parameters["@avg"].Value);
                   
                    Server.Close();

                    return getBook;
                }
                else
                    return null;
            }
        }

        public static List<Book> GetAllBooks()
        {

            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                SqlCommand cmd = new SqlCommand();
                DataTable sqlBooks = new DataTable(); //stores IDs of all books in db
                List<Book> BooksList = new List<Book>(); //store books objects for all books in db            
                SqlDataAdapter Data = new SqlDataAdapter("Select ItemID From [Books]", ServerConnection);
                Data.Fill(sqlBooks);

                foreach (DataRow row in sqlBooks.Rows)
                {
                    Book book = GetBook((int)row["ItemID"]);
                    if (book != null)
                        BooksList.Add(book);
                }

                ServerConnection.Close();

                return BooksList;
            }
        }

        public static KeyValuePair<Book, List<Review>> GetBookReviews(int bid, int uid)
        {
            Book book = GetBook(bid);
            List<Review> reviews = ReviewCRUD.GetReviews(bid);

            int index = reviews.FindIndex(item => item.UserID == uid);

            //if user's review doesnt exist, place null at start of list
            if (index < 0)
                reviews.Insert(0, null);
            else if (index > 0)
            {
                //move user's review to start of list
                Review UserReview = reviews[index];
                reviews.RemoveAt(index);
                reviews.Insert(0, UserReview);
            }

            return new KeyValuePair<Book, List<Review>>(book, reviews);
        }

        public static List<Book> TitleSearch(string search)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                List<Book> books = new List<Book>();

                ServerConnection.Open();
                SqlCommand cmd = new SqlCommand();
                //setting up command to call procedure
                cmd.CommandText = "SearchBook";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //parameters
                cmd.Parameters.Add(new SqlParameter("@namestr", search));

                DataTable itemIDs = new DataTable(); //stores IDs      
                SqlDataAdapter Data = new SqlDataAdapter(cmd);
                Data.Fill(itemIDs);  //get procedure result set

                foreach (DataRow row in itemIDs.Rows)
                    books.Add(BookCRUD.GetBook((int)row["ItemID"]));

                ServerConnection.Close();

                return books;
            }
        }

        public static List<Book> AuthorSearch(string search)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                List<Book> books = new List<Book>();

                ServerConnection.Open();
                SqlCommand cmd = new SqlCommand();
                //setting up command to call procedure
                cmd.CommandText = "SearchByAuthor";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //parameters
                cmd.Parameters.Add(new SqlParameter("@authstr", search));
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                int Flag = (int)cmd.Parameters["@flag"].Value;

                if (Flag == 1)
                {
                    DataTable itemIDs = new DataTable(); //stores IDs      
                    SqlDataAdapter Data = new SqlDataAdapter(cmd);
                    Data.Fill(itemIDs);  //get procedure result set

                    foreach (DataRow row in itemIDs.Rows)
                        books.Add(BookCRUD.GetBook((int)row["ItemID"]));
                }

                ServerConnection.Close();
                return books;
            }
        }

        public static List<Book> GenreSearch(string search)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                List<Book> books = new List<Book>();

                ServerConnection.Open();
                SqlCommand cmd = new SqlCommand();
                //setting up command to call procedure
                cmd.CommandText = "SearchByGenre";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //parameters
                cmd.Parameters.Add(new SqlParameter("@genre", search));
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                int Flag = (int)cmd.Parameters["@flag"].Value;  //check if required order id found

                if (Flag == 1)
                {
                    DataTable itemIDs = new DataTable(); //stores IDs      
                    SqlDataAdapter Data = new SqlDataAdapter(cmd);
                    Data.Fill(itemIDs);  //get procedure result set

                    foreach (DataRow row in itemIDs.Rows)
                        books.Add(BookCRUD.GetBook((int)row["ItemID"]));

                }

                ServerConnection.Close();
                return books;
            }
        }

        public static List<Book> CategorySearch(string search)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                List<Book> books = new List<Book>();

                ServerConnection.Open();
                SqlCommand cmd = new SqlCommand();
                //setting up command to call procedure
                cmd.CommandText = "SearchByCategory";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //parameters
                cmd.Parameters.Add(new SqlParameter("@cat", search));
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                int Flag = (int)cmd.Parameters["@flag"].Value;

                if (Flag == 1)
                {
                    DataTable itemIDs = new DataTable(); //stores IDs      
                    SqlDataAdapter Data = new SqlDataAdapter(cmd);
                    Data.Fill(itemIDs);  //get procedure result set

                    foreach (DataRow row in itemIDs.Rows)
                        books.Add(BookCRUD.GetBook((int)row["ItemID"]));
                }

                ServerConnection.Close();
                return books;
            }
        }

        public static List<Book> UserRecommendations(int id)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                List<Book> books = new List<Book>();

                ServerConnection.Open();
                SqlCommand cmd = new SqlCommand();
                //setting up command to call procedure
                cmd.CommandText = "RecommendBooks";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //parameters
                cmd.Parameters.Add(new SqlParameter("@uid", id));
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                DataTable itemIDs = new DataTable(); //stores IDs      
                SqlDataAdapter Data = new SqlDataAdapter(cmd);
                Data.Fill(itemIDs);  //get procedure result set

                int Flag = (int)cmd.Parameters["@flag"].Value;  //check if required order id found

                if (Flag == 1)                                
                    foreach (DataRow row in itemIDs.Rows)                   
                        books.Add(BookCRUD.GetBook((int)row["ID"]));                                    

                ServerConnection.Close();

                return books;
            }
        }

        public static List<Book> BestSellers()
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                List<Book> books = new List<Book>();

                ServerConnection.Open();
                SqlCommand cmd = new SqlCommand();
                //setting up command to call procedure
                cmd.CommandText = "BestSellers";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                DataTable itemIDs = new DataTable(); //stores IDs      
                SqlDataAdapter Data = new SqlDataAdapter(cmd);
                Data.Fill(itemIDs);  //get procedure result set

                foreach (DataRow row in itemIDs.Rows)
                {
                    books.Add(BookCRUD.GetBook((int)row["ItemID"]));
                }

                ServerConnection.Close();

                return books;
            }
        }

        public static bool CreateBook(Book newBook)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                //calling procedure from db
                SqlCommand cmd = new SqlCommand();
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
                DataTable authtable = new DataTable();
                authtable.Columns.Add("auth", typeof(string));
                foreach (string str in newBook.Authors)
                {
                    DataRow row = authtable.NewRow();
                    row["auth"] = str;
                    authtable.Rows.Add(row);
                }

                DataTable gentable = new DataTable();
                gentable.Columns.Add("auth", typeof(string));
                foreach (string str in newBook.Genres)
                {
                    DataRow row = gentable.NewRow();
                    row["auth"] = str;
                    gentable.Rows.Add(row);
                }

                cmd.Parameters.Add(new SqlParameter("@auth", authtable));
                cmd.Parameters["@auth"].SqlDbType = SqlDbType.Structured;
                cmd.Parameters.Add(new SqlParameter("@gen", gentable));
                cmd.Parameters["@gen"].SqlDbType = SqlDbType.Structured;

                //passing output para
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();  //run procedure

                int Flag = (int)cmd.Parameters["@flag"].Value;
                ServerConnection.Close();

                return Flag == 1;
            }
        }

        public static bool UpdateBook(Book newBook)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                //calling procedure from db
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "UpdateBook";
                cmd.Connection = ServerConnection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //passing parameters to procedure
                cmd.Parameters.Add(new SqlParameter("@id", newBook.BookID));
                cmd.Parameters.Add(new SqlParameter("@title", newBook.Title));
                cmd.Parameters.Add(new SqlParameter("@synp", newBook.Synopsis));
                cmd.Parameters.Add(new SqlParameter("@pub", newBook.Publisher));
                cmd.Parameters.Add(new SqlParameter("@cat", newBook.Category));
                cmd.Parameters.Add(new SqlParameter("@price", newBook.Price));
                cmd.Parameters.Add(new SqlParameter("@stock", newBook.Stock));
                cmd.Parameters.Add(new SqlParameter("@sub", newBook.SubStatus));
                cmd.Parameters.Add(new SqlParameter("@disc", newBook.Discount));

                //passing table paras
                DataTable authtable = new DataTable();
                authtable.Columns.Add("auth", typeof(string));
                foreach (string str in newBook.Authors)
                {
                    DataRow row = authtable.NewRow();
                    row["auth"] = str;
                    authtable.Rows.Add(row);
                }

                DataTable gentable = new DataTable();
                gentable.Columns.Add("auth", typeof(string));
                foreach (string str in newBook.Genres)
                {
                    DataRow row = gentable.NewRow();
                    row["auth"] = str;
                    gentable.Rows.Add(row);
                }

                cmd.Parameters.Add(new SqlParameter("@auth", authtable));
                cmd.Parameters["@auth"].SqlDbType = SqlDbType.Structured;
                cmd.Parameters.Add(new SqlParameter("@gen", gentable));
                cmd.Parameters["@gen"].SqlDbType = SqlDbType.Structured;

                //passing output para
                cmd.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
                cmd.Parameters["@flag"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();  //run procedure

                int Flag = (int)cmd.Parameters["@flag"].Value;
                ServerConnection.Close();

                return Flag == 1;
            }
        }

        public static bool UpdatePrice(int id, int newPrice)
        {

            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                SqlCommand cmd = new SqlCommand();
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

                return Flag == 1;
            }
        }

        public static bool UpdateStock(int id, int newStock)
        {

            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                SqlCommand cmd = new SqlCommand();
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

                return Flag == 1;
            }
        }

        public static bool DeleteBook(int id)
        {
            using (SqlConnection ServerConnection = new SqlConnection(ConnectionString))
            {
                ServerConnection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "DeleteBook";
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


