using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DB_Project.Models;

namespace DB_Project.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Console()
        {
            return View();
        }

        public ActionResult tempview()
        {
            return View();
        }

        // GET: All Books
        public ActionResult BooksList()
        {
            return View("~/Views/Admin/Console.cshtml", BookCRUD.GetAllBooks());
        }

        public ActionResult BookDetails(int id)
        {
            return View("~/Views/Admin/Console.cshtml", BookCRUD.GetBook(id));
        }

        public ActionResult EditBook(int id)
        {
            Book mybook = BookCRUD.GetBook(id);
            return PartialView("_EditBook", mybook);
        }

        [HttpPost]
        public ActionResult UpdateBook(FormCollection collection)
        {
            Book newBook = new Book();

            newBook.BookID = Int32.Parse(collection["BookID"]);
            newBook.Title = collection["Title"];
            newBook.Synopsis = collection["Synopsis"];
            newBook.Publisher = collection["Publisher"];
            newBook.Category = collection["Category"];
            newBook.Price = Int32.Parse(collection["Price"]);
            newBook.Stock = Int32.Parse(collection["Stock"]);
            newBook.SubStatus = bool.Parse(collection["SubStatus"]);
            newBook.Authors = collection["Authors"].Split(',').ToList();
            newBook.Genres = collection["Genres"].Split(',').ToList();


            if (BookCRUD.UpdateBook(newBook))
                return Content("<script>alert('Book has been Updated Successfully.');window.location = 'Console';</script>");
            else
                return Content("<script>alert('Book could not be Update.');window.location = 'Console'</script>");
        }

        [HttpPost]
        public ActionResult AddBook(FormCollection collection)
        {
            Book newBook = new Book();

            newBook.Title = collection["Title"];
            newBook.Synopsis = collection["Synopsis"];
            newBook.Publisher = collection["Publisher"];
            newBook.Category = collection["Category"];
            newBook.Price = Int32.Parse(collection["Price"]);
            newBook.Stock = Int32.Parse(collection["Stock"]);
            newBook.SubStatus = bool.Parse(collection["SubStatus"]);
            newBook.Authors = collection["Authors"].Split(',').ToList();
            newBook.Genres = collection["Genres"].Split(',').ToList();


            if (BookCRUD.CreateBook(newBook))
                return Content("<script>alert('Book has been added Successfully.');window.location = 'Console';</script>");
            else
                return Content("<script>alert('Book could not be added.');window.location = 'Console'</script>");
        }

        
        [HttpPost]
        public ActionResult RemoveBook(int id)
        {
            if (BookCRUD.DeleteBook(id))
                return Content("<script>alert('Book Deleted Successfully.');window.location = 'Console';</script>");
            else
                return Content("<script>alert('Book could not be found.');window.location = 'Console'</script>");
        }

        
        public ActionResult Order()
        {
            return View();
        }

        public ActionResult AllOrders()
        {
            return View("~/Views/Admin/Order.cshtml", OrderCRUD.GetAllOrders());
        }

        public ActionResult RemoveOrder(int id)
        {
            if (OrderCRUD.DeleteOrder(id))
                return Content("<script>alert('Order Deleted Successfully.');window.location = 'Order';</script>");
            else
                return Content("<script>alert('Order could not be found.');window.location = 'Order'</script>");
        }

    }
}