﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalProject.Infrastructure;
using FinalProject.Models;

namespace FinalProject.Controllers
{
    //This controller gets called by the default rout. If you look in the AppStart folder, you'll see
    //a file called RouteConfig.cs. There were changes made there to make this the default controller
    //and I added some comments there to explain how it works exactly.
    [Authorize]
    public class ticketsController : Controller
    {
        private FinalProjectDBEntities db = new FinalProjectDBEntities();

        // GET: tickets
        //By default the route goes tickets/Index (see RouteConfig.cs). This method is controlling the first page you see.
        public ActionResult Index()
        {
            var tickets = db.tickets.Include(t => t.user); //As of now this gets all the tickets.
            //var email = System.Security.Claims.ClaimsPrincipal.Current.Claims.First().Value; (ignore this)

            //Just an example of how you can change the display. You can play around with it to see. Eventually we will
            //have something like this that will easily make it so we just see the current user's tickets. Once we can
            //differentiate administrators, we can just include an if statement to control the output.
            //var tickets = db.tickets.Where(t => t.user.userID == 3);
            
            //Returns the view with all the tickets as a list. Views/Tickets/Index is the associated html page.
            //I'll add some comments there soon.
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
                //Gets the current highest value of any ticketID.
                var maxValue = db.tickets.Max(t => t.ticketID);
                //Sets the ticketID of the new ticket to 1 + the current highest ticketId. So as new tickets are created, they always
                //have an ID one higher than the last one. You can see this when you run the program and look at ticket details.
                ticket.ticketID = maxValue + 1;
                //Sets the ticket's status to 1 (unopen). I will be changing this to display words rather than numbers soon.
                ticket.status = 1;
                //The rest of the values are the values the user enters on the create ticket page. This next line adds this new ticket to the DB.
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
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userID = new SelectList(db.users, "userID", "firstName", ticket.userID);
            return View(ticket);
        }

        // GET: tickets/Delete/5
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
