using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DB_Project.Models;

namespace DB_Project.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult DashBoard()
        {
            return View();
        }

        public ActionResult Books()
        {
            return View();
        }

        public ActionResult BooksList()
        {
            return View("~/Views/User/Books.cshtml", BookCRUD.GetAllBooks());
        }

        [HttpPost]
        public ActionResult SearchByGenre(FormCollection collection)
        {
            string svalue = collection["searchString"];
            return View("~/Views/User/Books.cshtml", BookCRUD.GenreSearch(svalue));
        }

        public ActionResult BookDetails(int id)
        {
            return View(BookCRUD.GetBookReviews(id, (int)Session["UserID"]));
        }

        [HttpPost]
        public ActionResult ReviewBook(FormCollection collection)
        {
            Review newReview = new Review();
            newReview.UserID = (int)Session["UserID"];
            newReview.UserName = (string)Session["UserName"];
            newReview.BookID = Int32.Parse(collection["BookID"]);
            newReview.Description = collection["ReviewText"];
            newReview.Rating = Int32.Parse(collection["Rating"]);

            if (ReviewCRUD.CreateReview(newReview))
                return Content("<script>alert('Review has been added Successfully.');window.location.reload();</script>");
            else
                return Content("<script>alert('Review Failed.');window.location.reload();</script>");
        }

    }
}