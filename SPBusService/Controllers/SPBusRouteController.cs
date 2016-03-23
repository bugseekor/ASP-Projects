// File Name : SPBusRouteController.cs
// CRUD controller for busRoute table
//
// Author : Sam Sangkyun Park
// Date Created : Sep. 17, 2015
// Revision History : Version 1 created : Sep. 17, 2015

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SPBusService.Models;

namespace SPBusService.Controllers
{
    public class SPBusRouteController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // GET: SPBusRoutes
        public ActionResult Index()
        {
            return View(db.busRoutes.ToList());
        }

        // GET: SPBusRoutes/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busRoute busRoute = db.busRoutes.Find(id);
            if (busRoute == null)
            {
                return HttpNotFound();
            }
            return View(busRoute);
        }

        // GET: SPBusRoutes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SPBusRoutes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "busRouteCode,routeName")] busRoute busRoute)
        {
            if (ModelState.IsValid)
            {
                db.busRoutes.Add(busRoute);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(busRoute);
        }

        // GET: SPBusRoutes/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busRoute busRoute = db.busRoutes.Find(id);
            if (busRoute == null)
            {
                return HttpNotFound();
            }
            return View(busRoute);
        }

        // POST: SPBusRoutes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "busRouteCode,routeName")] busRoute busRoute)
        {
            if (ModelState.IsValid)
            {
                db.Entry(busRoute).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(busRoute);
        }

        // GET: SPBusRoutes/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busRoute busRoute = db.busRoutes.Find(id);
            if (busRoute == null)
            {
                return HttpNotFound();
            }
            return View(busRoute);
        }

        // POST: SPBusRoutes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            busRoute busRoute = db.busRoutes.Find(id);
            db.busRoutes.Remove(busRoute);
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
