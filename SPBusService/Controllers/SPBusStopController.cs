// File Name : SPBusStopController.cs
// CRUD controller for busStop table
//
// Author : Sam Sangkyun Park
// Date Created : Sep. 17, 2015
// Revision History : Version 1 created : Sep. 17, 2015
//                    Updated for Index View Sorting and RouteSelector View : Oct. 04, 2015

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
    public class SPBusStopController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // GET: SPBusStop
        /// <summary>
        /// return sorted view
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns>locationOrder model or busStopNumberOrder model</returns>
        public ActionResult Index(string orderBy)
        {
            
            if( orderBy == "location")
            {
                var locationOrder = db.busStops.OrderBy(a => a.location);
                return View(locationOrder);
            }
            else
            {
                var busStopNumberOrder = db.busStops.OrderBy(a => a.busStopNumber);
                return View(busStopNumberOrder);
            }
        }

        // GET: SPBusStop/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        // GET: SPBusStop/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SPBusStop/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "busStopNumber,location,locationHash,goingDowntown")] busStop busStop)
        {
            if (ModelState.IsValid)
            {
                // Calculating byte sum of a String and it's going to be a locationHash
                int byteSum = 0;

                for (int i = 0; i < busStop.location.Length; i++)
                {
                    byteSum += (byte)busStop.location[i];
                }
                busStop.locationHash = byteSum;

                db.busStops.Add(busStop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(busStop);
        }

        // GET: SPBusStop/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        // POST: SPBusStop/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "busStopNumber,location,locationHash,goingDowntown")] busStop busStop)
        {
            if (ModelState.IsValid)
            {
                // Calculating byte sum of a String and it's going to be a locationHash
                int byteSum = 0;

                for (int i = 0; i < busStop.location.Length; i++ )
                {
                    byteSum += (byte)busStop.location[i];
                }
                busStop.locationHash = byteSum;


                db.Entry(busStop).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(busStop);
        }

        // GET: SPBusStop/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        // POST: SPBusStop/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            busStop busStop = db.busStops.Find(id);
            db.busStops.Remove(busStop);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Handles post actions depends on number of routeCodes
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// case 1 : no routeCode returns Index View
        /// case 2 : 1 routeCode returns RouteStopSchedule View of SPRouteSchedule controller
        /// case 3 : more than 1 routeCodes ruturn RouteSelector View
        /// </returns>
        public ActionResult RouteSelector(int? id)
        {
            if(id == null)
            {
                TempData["message"] = "Select bus stop..";
                return RedirectToAction("Index");
            }
            else
            {
                Session["busStopNumber"] = id;
                int selectedCount = 0;
                var query = (from r in db.routeStops where r.busStopNumber == id select r.busRouteCode).Distinct().ToList();
                string routeCodeToRouteStopSchedule = "";

                List<SelectListItem> selectRouteNames = new List<SelectListItem>();
                foreach (var routeCode in query)
                {
                    if(routeCode == null)
                    {
                        continue;
                    }
                    busRoute busRoute = db.busRoutes.Find(routeCode);
                    selectRouteNames.Insert(selectedCount, new SelectListItem { Value = routeCode, Text = busRoute.routeName });
                    routeCodeToRouteStopSchedule = routeCode;
                    selectedCount++;
                }
                ViewBag.routeNames = selectRouteNames.OrderBy(a=>a.Text);

                //selectedCount = selectedCount - 1;
                
                if (selectedCount == 1)
                {
                    Session["routeCodeFromBusStop"] = routeCodeToRouteStopSchedule;
                    return RedirectToAction("RouteStopSchedule", "SPRouteSchedule");
                }
                else if (selectedCount > 1)
                {
                    return View();
                }
                else
                {
                    var busStop = db.busStops.Find(id);
                    string busLocation = busStop.location;
                    TempData["message"] = "No route found for " + busLocation + "(" + id + ")";
                    return RedirectToAction("Index");
                }
            }
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
