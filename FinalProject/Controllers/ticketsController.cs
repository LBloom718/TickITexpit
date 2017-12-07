using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalProject.Infrastructure;
using FinalProject.Models;
using Microsoft.AspNet.Identity;

namespace FinalProject.Controllers
{
     
    //This controller gets called by the default rout. If you look in the AppStart folder, you'll see
    //a file called RouteConfig.cs. There were changes made there to make this the default controller
    //and I added some comments there to explain how it works exactly.
    [Authorize]
    public class ticketsController : Controller
    {
        private FinalProjectDBEntities db = new FinalProjectDBEntities();
        public string userEmail;
        public string fName;
        public string lName;
        public int userType;
        public int id;

        [NonAction]
        int GetUserType()
        {
            this.userEmail = User.Identity.GetUserName();
            this.userType = (from users in db.users
                             where users.email == userEmail
                             select users.userType).Single();
            return userType;
        }
        // GET: tickets
        //By default the route goes tickets/Index (see RouteConfig.cs). This method is controlling the first page you see.
        [HttpPost] 
        public ActionResult Index(string submit)
        {
            this.userEmail = User.Identity.GetUserName();
            bool inDatabase = false;

            foreach (user user in db.users)
            {
                if (user.email == this.userEmail)
                {
                    inDatabase = true;
                }
            }

            if (inDatabase == false)
            {
                return View("NotAuthorized");
            }

            this.userType = (from users in db.users
                             where users.email == userEmail
                             select users.userType).Single();
            var strings = submit.Split(' ');
            var submit1 = strings[0];
            List<ticket> list = null;
            
            if (this.userType == 2)
            {
                var adminTickets = db.tickets.Include(t => t.user);
                ViewBag.name = "Administrator";
                switch (submit1)
                {
                    case "UnOpen":
                        list = db.tickets.Where(t =>  (t.status == Status.UnOpen)).ToList(); break;
                    case "Open":
                        list = db.tickets.Where(t =>(t.status == Status.Open)).ToList(); break;
                    case "Closed":
                        list = db.tickets.Where(t =>  (t.status == Status.Closed)).ToList(); break;

                }
                //Uses the user's email address to control the display.
                //var tickets = db.tickets.Where(t => t.user.email == userEmail);

                //Returns the view with all the tickets as a list. Views/Tickets/Index is the associated html page.
                //I'll add some comments there soon.
                ViewBag.UnOpen = "UnOpen - " + db.tickets.Where(t =>  (t.status == Status.UnOpen)).Count();
                ViewBag.Open = "Open -" + db.tickets.Where(t =>  (t.status == Status.Open)).Count(); ;
                ViewBag.Closed = "Closed - " + db.tickets.Where(t =>  (t.status == Status.Closed)).Count();
                ViewBag.User = "Admin";
                return View("AdminView", list);
            }

            this.fName = (from users in db.users
                          where users.email == userEmail
                          select users.firstName).Single();
            this.lName = (from users in db.users
                          where users.email == userEmail
                          select users.lastName).Single();
            ViewBag.name = $"{fName} {lName}";

            switch (submit1)
            {
                case "UnOpen":
                    list = db.tickets.Where(t => (t.user.email == userEmail) && (t.status == Status.UnOpen)).ToList(); break;
                case "Open":
                    list =db.tickets.Where(t => (t.user.email == userEmail) && (t.status == Status.Open)).ToList();
                    break;
                case "Closed":
                    list = db.tickets.Where(t => (t.user.email == userEmail) && (t.status == Status.Closed)).ToList(); break;

            }
            //Uses the user's email address to control the display.
            //var tickets = db.tickets.Where(t => t.user.email == userEmail);

            //Returns the view with all the tickets as a list. Views/Tickets/Index is the associated html page.
            //I'll add some comments there soon.
            ViewBag.UnOpen = "UnOpen - " + db.tickets.Where(t =>( t.user.email == userEmail) && (t.status== Status.UnOpen)).Count();
            ViewBag.Open = "Open -" + db.tickets.Where(t => (t.user.email == userEmail) && (t.status == Status.Open)).Count(); ;
            ViewBag.Closed = "Closed - " + db.tickets.Where(t => (t.user.email == userEmail) && ( t.status == Status.Closed)).Count();
            // return View(tickets.ToList());
            return View(list.ToList());
        }
        public ActionResult Index()
        {
            this.userEmail = User.Identity.GetUserName();
            bool inDatabase = false;

            foreach (user user in db.users)
            {
                if (user.email == this.userEmail)
                {
                    inDatabase = true;
                }
            }

            if (inDatabase == false)
            {
                return View("NotAuthorized");
            }

            this.userType = (from users in db.users
                             where users.email == userEmail
                             select users.userType).Single();

            if (this.userType == 2)
            {
                var list = db.tickets.Where(t => (t.status == Status.UnOpen));
                ViewBag.name = "Administrator";
                ViewBag.UnOpen = "UnOpen - " + db.tickets.Where(t => (t.status == Status.UnOpen)).Count();
                ViewBag.Open = "Open -" + db.tickets.Where(t =>  (t.status == Status.Open)).Count(); ;
                ViewBag.Closed = "Closed - " + db.tickets.Where(t => (t.status == Status.Closed)).Count();

                return View("AdminView",list.ToList() );
            }

            this.fName = (from users in db.users
                          where users.email == userEmail
                          select users.firstName).Single();
            this.lName = (from users in db.users
                          where users.email == userEmail
                          select users.lastName).Single();
            ViewBag.name = $"{fName} {lName}";


            //Uses the user's email address to control the display.
           // var tickets = db.tickets.Where(t => t.user.email == userEmail);
            var tickets = db.tickets.Where(t => (t.user.email == userEmail) && (t.status == Status.UnOpen));

            //Returns the view with all the tickets as a list. Views/Tickets/Index is the associated html page.
            //I'll add some comments there soon.
            ViewBag.UnOpen = "UnOpen - " + db.tickets.Where(t => (t.user.email == userEmail) && (t.status == Status.UnOpen)).Count();
            ViewBag.Open = "Open -" + db.tickets.Where(t => (t.user.email == userEmail) && (t.status == Status.Open)).Count(); ;
            ViewBag.Closed = "Closed - " + db.tickets.Where(t => (t.user.email == userEmail) && (t.status == Status.Closed)).Count();
            return View(tickets.ToList());
        }


        // GET: tickets/Details/5
        //Controlls what happens when you click on 'details' when running the program, using the ticketID.
        //Automatically generated.
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ticket ticket = db.tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: tickets/Create
        //Automatically generated and to be honest I don't really know right now.
        public ActionResult Create()
        {
            ViewBag.userID = new SelectList(db.users, "userID", "firstName");
            return View();
        }

        // POST: tickets/Create
        // Here is where we are creating the new ticket and adding it to the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ticketID,userID,date,description,status,type")] ticket ticket)
        {
            if (ModelState.IsValid)
            {
                this.userEmail = User.Identity.GetUserName();
                //Gets the current highest value of any ticketID.
                var maxValue = db.tickets.Max(t => t.ticketID);
                //Sets the ticketID of the new ticket to 1 + the current highest ticketId. So as new tickets are created, they always
                //have an ID one higher than the last one. You can see this when you run the program and look at ticket details.
                ticket.ticketID = maxValue + 1;
                ticket.userID = (from users in db.users
                                 where users.email == this.userEmail
                                 select users.userID).Single();
                //ticket.user.firstName = fName;
                //ticket.user.lastName = this.lName;
                //Sets the ticket's status to 1 (unopen). I will be changing this to display words rather than numbers soon.
                ticket.status = Status.UnOpen;
                //The rest of the values are the values the user enters on the create ticket page. This next line adds this new ticket to the DB.
                ticket.date = DateTime.Now.ToShortDateString();
                ViewBag.email = this.userEmail;
                db.tickets.Add(ticket);
                db.SaveChanges();
                //Takes us back to tickets/Index.
                return RedirectToAction("Index");
            }

            ViewBag.userID = new SelectList(db.users, "userID", "firstName", ticket.userID);
            return View(ticket);
        }



        //These methods aren't working properly right now and I'm not positive why, as they were auto generated.
        //If anyone wants to give it a try, feel free!
        // GET: tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ticket ticket = db.tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.userID = new SelectList(db.users, "userID", "firstName", ticket.userID);
            ViewBag.userType = GetUserType().ToString();
            return View(ticket);
        }

        // POST: tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ticketID,userID,date,description,status,type")] ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                //ticket.userID = 2;
                //ticket.date = "1/1/11";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userID = new SelectList(db.users, "userID", "firstName", ticket.userID);
            ViewBag.userType = GetUserType().ToString();
            return View(ticket);
        }

        // GET: tickets/Delete/5
        //The delet methods work fine, but we'll have to change the access for them.
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ticket ticket = db.tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ticket ticket = db.tickets.Find(id);
            db.tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
