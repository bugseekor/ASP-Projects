 //File Name : SPRouteStopController.cs
 //CRUD controller for routeStop table

 //Author : Sam Sangkyun Park
 //Date Created : Sep. 25, 2015
 //Revision History : Version 1 created : Sep. 25, 2015

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
    public class SPRouteStopController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // GET: SPRouteStop
        /// <summary>
        /// Index ActionResult 
        /// Takes busRouteCode as cookie or session variables
        /// Assigns cookie or session variables to busRouteCode when there's no parameter
        /// </summary>
        /// <param name="busRouteCode">Query string that narrows the result</param>
        /// <returns></returns>
        public ActionResult Index(string busRouteCode)
        {
            if(busRouteCode != null)
            {
                Session["busRouteCode"] = busRouteCode;
                Response.Cookies.Add(new HttpCookie("busRouteCode", busRouteCode));
            }
            else if (Request.Cookies["busRoutecode"] != null)
            {
                busRouteCode = Request.Cookies["busRouteCode"].Value;
            }
            else if(Session["busRouteCode"] != null)
            {
                busRouteCode = Session["busRouteCode"].ToString();
            }
            else
            {
                TempData["message"] = "Please select Bus Route..";
                return RedirectToAction("index", "SPBusRoute");
            }

            var routeStops = db.routeStops.Where(r => r.busRouteCode == busRouteCode).OrderBy(r => r.offsetMinutes).Include(r => r.busRoute).Include(r => r.busStop);

            // To display title of this view with busRouteCode and RouteName
            ViewBag.busRouteCode = busRouteCode;
            busRoute busRoute = db.busRoutes.Find(busRouteCode);
            ViewBag.busRouteName = busRoute.routeName;

            return View(routeStops.ToList());
        }
        
        // GET: SPRouteStop/Details/5
        /// <summary>
        /// Details ActionResult
        /// Examines if there are any cookie or session variables to make title of this view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            string busRouteCode = "";

            //Examines cookie or session variables to assign to make title of this view
            if (Request.Cookies["busRoutecode"] != null)
            {
                busRouteCode = Request.Cookies["busRouteCode"].Value;
            }
            else if(Session["busRouteCode"] != null)
            {
                busRouteCode = Session["busRouteCode"].ToString();
            }
            else
            {
                TempData["message"] = "Please select Bus Route..";
                return RedirectToAction("index", "SPBusRoute");
            }

            // To display title of this view with busRouteCode and RouteName
            ViewBag.busRouteCode = busRouteCode;
            busRoute busRoute = db.busRoutes.Find(busRouteCode);
            ViewBag.busRouteName = busRoute.routeName;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeStop routeStop = db.routeStops.Find(id);
            if (routeStop == null)
            {
                return HttpNotFound();
            }
            return View(routeStop);
        }

        // GET: SPRouteStop/Create
        /// <summary>
        /// Create ActionResult
        /// Examines if there are any cookie or session variables to make title of this view
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            string busRouteCode = "";

            //Examines cookie or session variables to assign to make title of this view
            if (Request.Cookies["busRoutecode"] != null)
            {
                busRouteCode = Request.Cookies["busRouteCode"].Value;
            }
            else if (Session["busRouteCode"] != null)
            {
                busRouteCode = Session["busRouteCode"].ToString();
            }
            else
            {
                TempData["message"] = "Please select Bus Route..";
                return RedirectToAction("index", "SPBusRoute");
            }

            // To display title of this view with busRouteCode and RouteName
            ViewBag.busRouteCode_ = busRouteCode;
            busRoute busRoute = db.busRoutes.Find(busRouteCode);
            ViewBag.busRouteName = busRoute.routeName;

            //Commented because this auto generated code doesn't need any more.
            //ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", busRoute.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", busRoute.busRouteCode);
            return View();
        }

        // POST: SPRouteStop/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "routeStopId,busRouteCode,busStopNumber,offsetMinutes")] routeStop routeStop)
        {
            if (ModelState.IsValid)
            {
                //Examines cookie or session variables to assign to routeStop.busRouteCode
                if (Request.Cookies["busRoutecode"] != null)
                {
                    routeStop.busRouteCode = Request.Cookies["busRouteCode"].Value;
                }
                else if (Session["busRouteCode"] != null)
                {
                    routeStop.busRouteCode = Session["busRouteCode"].ToString();
                }
                else
                {
                    TempData["message"] = "Please select Bus Route..";
                    return RedirectToAction("index", "SPBusRoute");
                }

                db.routeStops.Add(routeStop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //Commented because this auto generated code doesn't need any more.
            //ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeStop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routeStop.busStopNumber);
            return View(routeStop);
        }

        // GET: SPRouteStop/Edit/5
        /// <summary>
        /// Edit ActionResult
        /// Examines if there's any cookie or session variables to make title of this view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeStop routeStop = db.routeStops.Find(id);
            if (routeStop == null)
            {
                return HttpNotFound();
            }

            string busRouteCode = "";

            //Examines cookie or session variables to assign to make title of this view
            if (Request.Cookies["busRoutecode"] != null)
            {
                busRouteCode = Request.Cookies["busRouteCode"].Value;
            }
            else if (Session["busRouteCode"] != null)
            {
                busRouteCode = Session["busRouteCode"].ToString();
            }
            else
            {
                TempData["message"] = "Please select Bus Route..";
                return RedirectToAction("index", "SPBusRoute");
            }

            // To display title of this view with busRouteCode and RouteName
            ViewBag.busRouteCode_ = busRouteCode;
            busRoute busRoute = db.busRoutes.Find(busRouteCode);
            ViewBag.busRouteName = busRoute.routeName;

            //This was commented to hide busRotueCode
            //ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeStop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routeStop.busStopNumber);
            return View(routeStop);
        }

        // POST: SPRouteStop/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "routeStopId,busRouteCode,busStopNumber,offsetMinutes")] routeStop routeStop)
        {
            if (ModelState.IsValid)
            {
                //Examines cookie or session variables to assign to routeStop.busRouteCode
                if (Request.Cookies["busRoutecode"] != null)
                {
                    routeStop.busRouteCode = Request.Cookies["busRouteCode"].Value;
                }
                else if (Session["busRouteCode"] != null)
                {
                    routeStop.busRouteCode = Session["busRouteCode"].ToString();
                }
                else
                {
                    TempData["message"] = "Please select Bus Route..";
                    return RedirectToAction("index", "SPBusRoute");
                }

                db.Entry(routeStop).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeStop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routeStop.busStopNumber);
            return View(routeStop);
        }

        // GET: SPRouteStop/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeStop routeStop = db.routeStops.Find(id);
            if (routeStop == null)
            {
                return HttpNotFound();
            }

            string busRouteCode = "";

            //Examines cookie or session variables to assign to make title of this view
            if (Request.Cookies["busRoutecode"] != null)
            {
                busRouteCode = Request.Cookies["busRouteCode"].Value;
            }
            else if (Session["busRouteCode"] != null)
            {
                busRouteCode = Session["busRouteCode"].ToString();
            }
            else
            {
                TempData["message"] = "Please select Bus Route..";
                return RedirectToAction("index", "SPBusRoute");
            }

            // To display title of this view with busRouteCode and RouteName
            ViewBag.busRouteCode = busRouteCode;
            busRoute busRoute = db.busRoutes.Find(busRouteCode);
            ViewBag.busRouteName = busRoute.routeName;

            return View(routeStop);
        }

        // POST: SPRouteStop/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            routeStop routeStop = db.routeStops.Find(id);
            db.routeStops.Remove(routeStop);
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
