// File Name : SPTripController.cs
// An empty MVC Controller that shows and creates trip record
//
// Author : Sam Sangkyun Park
// Date Created : Oct. 11, 2015
// Revision History : Version 1 created : Oct. 11, 2015

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPBusService.Models;
using System.Collections;


namespace SPBusService.Controllers
{
    public class SPTripController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // GET: SPTrip
        /// <summary>
        /// Shows the list of trip record list
        /// </summary>
        /// <param name="busRouteCode">busRouteCode from query string</param>
        /// <param name="routeName">route Name from query string</param>
        /// <returns>trip view model</returns>
        public ActionResult Index(string busRouteCode, string routeName)
        {
            string tempBusRouteCode = "";
            string tempRouteName = "";

            //Checks busRoteCode and routeName on the query string
            //If they are in there, store them into Session variables
            if (busRouteCode != null && routeName != null)
            {
                tempBusRouteCode = busRouteCode;
                tempRouteName = routeName;
                Session["busRouteCode"] = tempBusRouteCode;
                Session["routeName"] = tempRouteName;
            }

            //Checks Session variable for alternative data collection
            else if (Session["busRouteCode"] != null && Session["routeName"] != null)
            {
                tempBusRouteCode = Session["busRouteCode"].ToString();
                tempRouteName = Session["routeName"].ToString();
            }

            //Noting to do without busRouteCode and routeName
            else
            {
                TempData["message"] = "No Bus Route selected";
                RedirectToAction("Index", "SPBusRoute");
            }

            //LINQ expression for retrieving trip and related tables
            //Sorted by recent date first(descending) and then earlies time first
            var trips = from r in db.routeSchedules
                        join t in db.trips
                        on r.routeScheduleId equals t.routeScheduleId
                        join d in db.drivers
                        on t.driverId equals d.driverId
                        join b in db.buses
                        on t.busId equals b.busId
                        where r.busRouteCode == tempBusRouteCode
                        orderby t.tripDate descending, r.startTime

                        select new SearchedTrip
                        {
                            tripDate = t.tripDate,
                            startTime = r.startTime,
                            fullName = d.fullName,
                            busNumber = b.busNumber,
                            comments = t.comments
                        };

            return View(trips);
        }
        /// <summary>
        /// An initial ActionResult for Create action
        /// Create a new trip recored - no parameter
        /// </summary>
        /// <returns>Revised trip view model which is in SPBusService.Model</returns>
        public ActionResult Create()
        {
            //Retrieves Session variable to local variable
            string tempBusRouteCode = Session["busRouteCode"].ToString();

            //A LINQ expression that retrieves all scheduels for selecte busRoute
            var routeSchedule = from r in db.routeSchedules
                                where r.busRouteCode == tempBusRouteCode
                                orderby r.isWeekDay descending, r.startTime
                                select r;

            //Instantiates a List object that has the StartTime template which is in SPService.Models
            List<StartTime> startTimeList = new List<StartTime>();
            string timeString = "";

            //A loop that adds all record from routeSchedule intto the List object that has two variables.
            //Deals with a special format for the timeString like [hh-mm weekday]
            foreach (var time in routeSchedule)
            {
                if (time.startTime == null)
                {
                    continue;
                }
                timeString = time.startTime.ToString("hh");
                timeString += "-";
                timeString += time.startTime.ToString("mm");
                if (time.isWeekDay)
                {
                    timeString += " weekday";
                }
                else
                {
                    timeString += " weekend";
                }
                startTimeList.Add(new StartTime()
                {
                    routeScheduleId = time.routeScheduleId.ToString(),
                    timeString = timeString
                });
            }

            //Instantiates new trip object and assign the List object generated above into the value timeList
            //The variable "timeList" is in the revised trip model which is added to the SPBusService.Models
            var tripTimeListOnly = new trip
            {
                timeList = new SelectList(startTimeList, "routeScheduleId", "timeString")
            };

            //A LINQ expression that retreives all drivers
            var drivers = from d in db.drivers
                          orderby d.fullName
                          select d;

            //SelectListItem type for dropdown list of Create view
            List<SelectListItem> listDrivers = new List<SelectListItem>();
            int driverIndex = 0;

            foreach (var driver in drivers)
            {
                listDrivers.Insert(driverIndex, new SelectListItem { Value = driver.driverId.ToString(), Text = driver.fullName});
                driverIndex++;
            }
            ViewBag.drivers = listDrivers;

            //A LINQ expression that retreives all buses that are "available"
            var buses = from b in db.buses
                        where b.status == "available"
                        orderby b.busNumber
                        select b;
            ViewBag.buses = buses;

            //The type of returning view is revised trip model in SPService.Models
            return View(tripTimeListOnly);

        }
        /// <summary>
        /// [HttpPost] ActionResult of Create View
        /// </summary>
        /// <param name="trip">A revised model which is in SPBusService.Model</param>
        /// <returns>Revised trip view model which is in SPBusService.Model</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(trip trip)
        {
            //Does exactly same thing that has done in first Create ActionResult for dropdown lists.

            string tempBusRouteCode = Session["busRouteCode"].ToString();

            var routeSchedule = from r in db.routeSchedules
                                where r.busRouteCode == tempBusRouteCode
                                orderby r.isWeekDay descending, r.startTime
                                select r;

            List<StartTime> startTimeList = new List<StartTime>();
            string timeString = "";
            foreach (var time in routeSchedule)
            {
                if (time.startTime == null)
                {
                    continue;
                }
                timeString = time.startTime.ToString("hh");
                timeString += "-";
                timeString += time.startTime.ToString("mm");
                if (time.isWeekDay)
                {
                    timeString += " weekday";
                }
                else
                {
                    timeString += " weekend";
                }
                startTimeList.Add(new StartTime()
                {
                    routeScheduleId = time.routeScheduleId.ToString(),
                    timeString = timeString
                });
            }

            var tripTimeListOnly = new trip
            {
                timeList = new SelectList(startTimeList, "routeScheduleId", "timeString")
            };

            var drivers = from d in db.drivers
                          orderby d.fullName
                          select d;

            List<SelectListItem> listDrivers = new List<SelectListItem>();
            int driverIndex = 0;

            foreach (var driver in drivers)
            {
                listDrivers.Insert(driverIndex, new SelectListItem { Value = driver.driverId.ToString(), Text = driver.fullName });
                driverIndex++;
            }
            ViewBag.drivers = listDrivers;

            var buses = from b in db.buses
                        where b.status == "available"
                        orderby b.busNumber
                        select b;
            ViewBag.buses = buses;

            //Check the ModelState and generate error message when it is invalid
            if (ModelState.IsValid)
            {
                try
                {
                    db.trips.Add(trip);
                    db.SaveChanges();
                    //When succeeds, put a success message and goes to the refreshed list
                    TempData["message"] = "New trip added!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    //At any exception in that "try" clause, this takes innerException into ModelState,
                    //generate exception message to the TempData and returns current view model to Create View
                    ModelState.AddModelError("", ex.InnerException.Message);
                    TempData["message"] = "exception getting Trips: " + ex.GetBaseException().Message;
                    return View(tripTimeListOnly);
                }
            }
            //When the ModelState is invalid, put a message to the TempData and returns current view model to Create View
            TempData["message"] = "Model State is invalid";
            return View(tripTimeListOnly);
        }
    }
}