﻿using DB_Project.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DB_Project.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Console()
        {
            return View();
        }

        public ActionResult Users()
        {
            return View(AccountCRUD.GetAllUsers());
        }

        public ActionResult UserDetail(int id)
        {
            return PartialView("_UserDetail", AccountCRUD.GetAccount(id));
        }

        public ActionResult ChangeAccess(int id)
        {
            return PartialView("_changeAccess", id);
        }

        [HttpPost]
        public ActionResult UpdateUserPriviledges(FormCollection collection)
        {
            int id = Int32.Parse(collection["UserID"]);
            int value = Int32.Parse(collection["AccessStatus"]);
            if (AccountCRUD.ChangePriviledges(id, value == 1 ? "Admin" : "User"))
                return Content("<script>alert('User's Access Changed Successfully.');window.location = 'Users';</script>");
            else
                return Content("<script>alert('User not found.');window.location = 'Users';</script>");
        }

        public ActionResult RemoveUsers(int id)
        {
            if (AccountCRUD.RemoveUser(id))
                return Content("<script>alert('Account Deleted Successfully.');window.location = 'Users';</script>");
            else
                return Content("<script>alert('Account Deletion Failed.');window.location = 'Users';</script>");

        }

        // GET: All Books
        public ActionResult BooksList()
        {
            return View("~/Views/Admin/Console.cshtml", BookCRUD.GetAllBooks());
        }

        public ActionResult BookDetails(int id)
        {
            int temp = 0;
            return View(BookCRUD.GetBookReviews(id, temp));
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
            newBook.SubStatus = Convert.ToBoolean(collection["SubStatus"]);
            newBook.Authors = collection["Authors"].Split(',').ToList();
            newBook.Genres = collection["Genres"].Split(',').ToList();


            if (BookCRUD.UpdateBook(newBook))
                return Content("<script>alert('Book has been Updated Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Book could not be Update.');window.location.href=document.referrer</script>");
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
                return Content("<script>alert('Book has been added Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Book could not be added.');window.location.href=document.referrer</script>");
        }

        [HttpPost]
        public ActionResult RemoveBook(int id)
        {
            if (BookCRUD.DeleteBook(id))
                return Content("<script>alert('Book Deleted Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Book could not be found.');window.location.href=document.referrer</script>");
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
                return Content("<script>alert('Review has been added Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Review Failed.');window.location.href=document.referrer;</script>");
        }

        public ActionResult AllOrders()
        {
            return View("~/Views/Admin/Order.cshtml", OrderCRUD.GetAllOrders());
        }

        public ActionResult RemoveOrder(int id)
        {
            if (OrderCRUD.DeleteOrder(id))
                return Content("<script>alert('Order Deleted Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Order could not be found.');window.location.href=document.referrer</script>");
        }

        public ActionResult OrderStatus(int id)
        {
            return PartialView("_OrderStatus", id);
        }

        [HttpPost]
        public ActionResult UpdateStatus(FormCollection collection)
        {
            int oid = Int32.Parse(collection["OrderID"]);
            string status = collection["OrderStatus"];

            if (OrderCRUD.UpdateStatus(oid, status))
                return Content("<script>alert('Order Status has been Updated Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Order Status could not be Update.');window.location.href=document.referrer</script>");
        }

        public ActionResult OrderDetails(int id)
        {
            return PartialView("_OrderDetails", OrderCRUD.GetOrderItems(id));
        }

        public ActionResult BookSubscriptions(int id)
        {
            return View(SubscriptionCRUD.GetSubscribers(id));
        }

        public ActionResult UnSubscribe(int id)
        {
            if (SubscriptionCRUD.UnSubscribe(id, (int)Session["UserID"]))
                return Content("<script>alert('Unsubscribed Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Operation Failed.');window.location.href=document.referrer;</script>");
        }

    }
}