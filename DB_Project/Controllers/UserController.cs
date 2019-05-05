using DB_Project.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DB_Project.Controllers
{
    public class UserController : Controller
    {
        public ActionResult DashBoard()
        {
            //displaying books in best seller section and recommended section
            return View(new Tuple<List<Book>, List<Book>>(BookCRUD.BestSellers(), BookCRUD.UserRecommendations((int)Session["UserID"])));
            //return View(new Tuple<List<Book>, List<Book>>(BookCRUD.GetAllBooks(), BookCRUD.GetAllBooks()));
        }

        //Shopping Cart and User Order related methods
        public ActionResult Cart()
        {
            List<Tuple<int, int, int>> items = (List<Tuple<int, int, int>>)Session["OrderItems"];
            List<Book> BooksList = new List<Book>();
            foreach (var tuple in items)
            {
                Book getBook = BookCRUD.GetBook(tuple.Item1);
                if (getBook != null)
                    BooksList.Add(getBook);
            }

            return View(BooksList);
        }

        public ActionResult AddToCart(FormCollection collection)
        {
            int item = Int32.Parse(collection["ID"]);
            int price = Int32.Parse(collection["Price"]);
            int quantity = Int32.Parse(collection["Quantity"]);
            ((List<Tuple<int, int, int>>)Session["OrderItems"]).Add(new Tuple<int, int, int>(item, quantity, price));

            return Content("<script>alert('Item Added to Cart.');window.location.href=document.referrer;</script>");
        }

        public ActionResult RemoveFromCart(int id)
        {
            int index = ((List<Tuple<int, int, int>>)Session["OrderItems"]).FindIndex(tuple => tuple.Item1 == id);
            if (index >= 0)
            {
                ((List<Tuple<int, int, int>>)Session["OrderItems"]).RemoveAt(index);

                return Content("<script>alert('Cart has been Updated.');window.location.href=document.referrer;</script>");
            }
            else
                return Content("<script>alert('Item not found in Cart.');window.location.href=document.referrer;</script>");
        }

        public ActionResult EmptyCart()
        {
            ((List<Tuple<int, int, int>>)Session["OrderItems"]).Clear();
            ((List<Tuple<int, int, int>>)Session["OrderItems"]).TrimExcess();

            return Content("<script>alert('Cart has been Emptied.');window.location.href=document.referrer;</script>");
        }

        public ActionResult PlaceOrder()
        {
            Order newOrder = new Order();
            newOrder.UserID = (int)Session["UserID"];
            newOrder.Items = (List<Tuple<int, int, int>>)Session["OrderItems"];
            newOrder.TotalCost = OrderCRUD.CalcTotalCost(newOrder.Items);

            if (OrderCRUD.CreateOrder(newOrder))
                return Content("<script>alert('Order Placed Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Order could not be placed.');window.location.href=document.referrer;</script>");
        }

        public ActionResult ViewOrders()
        {
            List<Order> orders = OrderCRUD.GetUserOrders((int)Session["UserID"]);
            orders.RemoveAll(item => item.OrderStatus == "Delivered");

            return View(orders);
        }

        public ActionResult Requests()
        {
            return View(RequestCRUD.GetRequest((int)Session["UserID"]));
        }

        [HttpPost]
        public ActionResult AddRequest(FormCollection collection)
        {
            Request myreq = new Request();
            myreq.Description = collection["Description"];
            myreq.UserID = (int)Session["UserID"];
            if (RequestCRUD.CreateRequest(myreq))
                return Content("<script>alert('Request Added Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Request could not be Added.');window.location.href=document.referrer;</script>");
        }

        
        public ActionResult RemoveRequest(int id)
        {
            if (RequestCRUD.DeleteRequest(id))
                return Content("<script>alert('Request Deleted Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Request could not be Deleted.');window.location.href=document.referrer</script>");
        }

        public ActionResult History()
        {
            List<Order> orders = OrderCRUD.GetUserOrders((int)Session["UserID"]);
            orders.RemoveAll(item => item.OrderStatus != "Delivered");

            return View(orders);
        }

        //Books related methods
        public ActionResult Books()
        {
            return View(BookCRUD.GetAllBooks());
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
            newReview.Rating = 6 - Int32.Parse(collection["review-rating"]);

            if (ReviewCRUD.CreateReview(newReview))
                return Content("<script>alert('Review has been added Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Review Failed.');window.location.href=document.referrer;</script>");
        }

        public ActionResult Subscriptions()
        {
            return View(SubscriptionCRUD.GetSubscribedItems((int)Session["UserID"]));
        }

        public ActionResult Subscribe(int id)
        {
            if (SubscriptionCRUD.AddSubscription(id, (int)Session["UserID"]))
                return Content("<script>alert('Subscribed Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Subscription Failed.');window.location.href=document.referrer;</script>");
        }

        public ActionResult UnSubscribe(int id)
        {
            if (SubscriptionCRUD.UnSubscribe(id, (int)Session["UserID"]))
                return Content("<script>alert('Unsubscribed Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Operation Failed.');window.location.href=document.referrer;</script>");
        }

        //User Account related methods
        public ActionResult ProfileInfo()
        {
            return View(AccountCRUD.GetAccount((int)Session["UserID"]));
        }

        public ActionResult PasswordChange()
        {
            return PartialView("_PasswordChange");
        }

        [HttpPost]
        public ActionResult ChangePassword(FormCollection collection)
        {
            //int id = 1;
            string newPass = collection["Password"];

            if (AccountCRUD.ChangePassword((int)Session["UserID"], newPass))
                return Content("<script>alert('Password Changed Successfully Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Password could not be Changed');window.location.href=document.referrer</script>");
        }

        public ActionResult EditInfo(int id)
        {
            return PartialView("_EditInfo", AccountCRUD.GetAccount(id));
        }

        [HttpPost]
        public ActionResult ChangeInfo(FormCollection collection)
        {
            Account myacc = new Account();
            myacc.UserID = Int32.Parse(collection["UserID"]);
            myacc.Email = collection["Email"];
            myacc.Username = collection["Username"];
            myacc.ContactNo = collection["ContactNo"];
            myacc.Address = collection["Address"];
            myacc.Gender = Convert.ToChar(collection["Gender"]);
            if (AccountCRUD.UpdateUser(myacc))
                return Content("<script>alert('Profile Edited Successfully.');window.location.href=document.referrer;</script>");
            else
                return Content("<script>alert('Profile Could not be Updated');window.location.href=document.referrer</script>");
        }

        public ActionResult DeleteAccount()
        {
            List<Order> orders = OrderCRUD.GetUserOrders((int)Session["UserID"]);

            //if any order exists that hasnt been delievered then cannot delete account
            if (orders.FindIndex(item => item.OrderStatus != "Delivered") < 0)
            {
                AccountCRUD.RemoveUser((int)Session["UserID"]);
                return Redirect("Home");
            }
            else
                return Content("<script>alert('You still have pending Orders.');window.location.href=document.referrer</script>");
        }

    }



}