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

        // GET: All Books
        public ActionResult BooksList()
        {
            return View(BookCRUD.GetAllBooks());
        }

        public ActionResult BookDetails(int id)
        {
            return View(BookCRUD.GetBook(id));
        }

        public ActionResult UpdateBook()
        {
            return View();
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
            newBook.SubStatus = Convert.ToBoolean(collection["SubStatus"]);
            newBook.Authors = collection["Authors"].Split(',').ToList();
            newBook.Genres = collection["Genres"].Split(',').ToList();


            if (BookCRUD.CreateBook(newBook))
                return Content("<script>alert('Book has been added Successfully.');window.location = 'Console';</script>");
            else
                return Content("<script>alert('Book could not be added.');window.location = 'Console'</script>");
        }

        [HttpPost]
        public ActionResult EditPrice(FormCollection collection)
        {
            int id = Int32.Parse(collection["BookID"]);
            int newPrice = Int32.Parse(collection["Price"]);

            if (BookCRUD.UpdatePrice(id,newPrice))
                return Content("<script>alert('Book Updated Successfully.');window.location = 'Console';</script>");
            else
                return Content("<script>alert('Book could not be found.');window.location = 'Console'</script>");
        }

        [HttpPost]
        public ActionResult EditStock(FormCollection collection)
        {
            int id = Int32.Parse(collection["BookID"]);
            int newStock = Int32.Parse(collection["Stock"]);

            if (BookCRUD.UpdateStock(id, newStock))
                return Content("<script>alert('Book Updated Successfully.');window.location = 'Console';</script>");
            else
                return Content("<script>alert('Book could not be found.');window.location = 'Console'</script>");
        }

        [HttpPost]
        public ActionResult EditAll(FormCollection collection)
        {
            int id = Int32.Parse(collection["BookID"]);
            int newStock = Int32.Parse(collection["Stock"]);
            int newPrice = Int32.Parse(collection["Price"]);

            if (BookCRUD.UpdateStock(id, newStock) && BookCRUD.UpdatePrice(id,newPrice))
                return Content("<script>alert('Book Updated Successfully.');window.location = 'Console';</script>");
            else
                return Content("<script>alert('Book could not be found.');window.location = 'Console'</script>");
        }

        [HttpPost]
        public ActionResult RemoveBook(int id)
        {
            if (BookCRUD.DeleteBook(id))
                return Content("<script>alert('Book Deleted Successfully.');window.location = 'Index';</script>");
            else
                return Content("<script>alert('Book could not be found.');window.location = 'Index'</script>");
        }
    }
}