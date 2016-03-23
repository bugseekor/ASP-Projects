// File Name : SPRouteScheduleController.cs
// CRUD controller for routeSchedule table
//
// Author : Sam Sangkyun Park
// Date Created : Oct. 04, 2015
// Revision History : Version 1 created : Oct. 04, 2015

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
    public class SPRouteScheduleController : Controller
    {
        private BusServiceContext db = new BusServiceContext();
        
        /// <summary>
        /// Combine routeCode and busStop to return schedule
        /// </summary>
        /// <param name="form"></param>
        /// <returns>selected busStop and selected busRoute schedule</returns>
        public ActionResult RouteStopSchedule(FormCollection form)
        {
            string busRouteCode = "";

            //RouteSelector ActionResult of SPBusStopController may set this Session variable
            if(Session["routeCodeFromBusStop"] == null)
            {
                //RouteSelector View of SPBusStopController may select one of dropDownList
                if (form["routeNames"] == null)
                {
                    TempData["message"] = "No Bus Route selected!";
                    return RedirectToAction("index", "SPBusStop");
                }
                else
                {
                    busRouteCode = form["routeNames"];
                }
            }
            else
            {
                busRouteCode = Session["routeCodeFromBusStop"].ToString();
                Session["routeCodeFromBusStop"] = null;
            }
            
            int busStopNumber = (int)Session["busStopNumber"];
            int offsetMinutes = 0;

            busRoute busRoute = db.busRoutes.Find(busRouteCode);
            ViewBag.selectedRouteName = busRouteCode + " - " + busRoute.routeName;
            busStop busStop = db.busStops.Find(busStopNumber);
            ViewBag.selectedLocation = busStopNumber + " - " + busStop.location;

            //Finds out offsetMunutes of selected busRoute and selected busStop
            //If there are more than one record found accidently, it abends!!
            var routeStop = db.routeStops.Where(a => a.busRouteCode == busRouteCode && a.busStopNumber == busStopNumber).Single();
            offsetMinutes = (int)routeStop.offsetMinutes;

            //Composes all startTimes within the schedule
            var selectedSchedule = from r in db.routeSchedules
                                   where (r.busRouteCode == busRouteCode)
                                   orderby r.startTime
                                   select r;

            //Callculates busStop arrival times and returns model
            foreach (var item in selectedSchedule)
            {
                item.startTime = item.startTime.Add(TimeSpan.FromMinutes(offsetMinutes));
            }

            return View(selectedSchedule);
        }

        // GET: SPRouteSchedule
        public ActionResult Index()
        {
            var routeSchedules = db.routeSchedules.Include(r => r.busRoute);
            return View(routeSchedules.ToList());
        }

        // GET: SPRouteSchedule/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            return View(routeSchedule);
        }

        // GET: SPRouteSchedule/Create
        public ActionResult Create()
        {
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName");
            return View();
        }

        // POST: SPRouteSchedule/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "routeScheduleId,busRouteCode,startTime,isWeekDay,comments")] routeSchedule routeSchedule)
        {
            if (ModelState.IsValid)
            {
                db.routeSchedules.Add(routeSchedule);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        // GET: SPRouteSchedule/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        // POST: SPRouteSchedule/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "routeScheduleId,busRouteCode,startTime,isWeekDay,comments")] routeSchedule routeSchedule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(routeSchedule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        // GET: SPRouteSchedule/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            return View(routeSchedule);
        }

        // POST: SPRouteSchedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            db.routeSchedules.Remove(routeSchedule);
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
