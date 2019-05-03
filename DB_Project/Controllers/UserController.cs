using DB_Project.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DB_Project.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult DashBoard()
        {
            //displaying books in best seller section and recommended section
            return View(new Tuple<List<Book>, List<Book>>(BookCRUD.BestSellers(), BookCRUD.UserRecommendations((int)Session["UserID"])));
        }

        public ActionResult AddToCart(int item, int quantity, int price)
        {
            ((List<Tuple<int, int, int>>)Session["OrderItems"]).Add(new Tuple<int, int, int>(item, quantity, price));

            return Content("<script>alert('Item Added to Cart.');window.location.href=document.referrer;</script>");
        }

        public ActionResult PlaceOrder()
        {
            Order newOrder = new Order();
            newOrder.UserID = (int)Session["UserID"];
            newOrder.Items = (List<Tuple<int, int, int>>)Session["OrderItems"];
            newOrder.TotalCost = OrderCRUD.CalcTotalCost(newOrder.Items);

            if(OrderCRUD.CreateOrder(newOrder))
                return Content("<script>alert('Order Placed Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Order could not be placed.');window.location.href=document.referrer;</script>");
        }

        public ActionResult DeleteAccount()
        {
            AccountCRUD.RemoveUser((int)Session["UserID"]);
            return Redirect("Home");
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
        public ActionResult SearchBy(FormCollection collection)
        {
            string svalue = collection["searchString"];
            string scat = collection["searchBy"];

            switch (scat)
            {
                case "Genre":
                    return View("~/Views/User/Books.cshtml", BookCRUD.GenreSearch(svalue));

                case "Author":
                    return View("~/Views/User/Books.cshtml", BookCRUD.AuthorSearch(svalue));

                case "Category":
                    return View("~/Views/User/Books.cshtml", BookCRUD.CategorySearch(svalue));
            }
            return View("~/Views/User/Books.cshtml");
        }

        public ActionResult BookDetails(int id)
        {
            return View(BookCRUD.GetBookReviews(id, (int)Session["UserID"]));
        }

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

        public ActionResult Requests()
        {
            return View();

        }

        public ActionResult History()
        {
            return View();

        }

        public ActionResult Profile()
        {
            return View();

        }

    }



}