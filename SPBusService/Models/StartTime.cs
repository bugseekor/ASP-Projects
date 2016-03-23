// File Name : StartTime.cs
// A model that has two fields for trip start times
//
// Author : Sam Sangkyun Park
// Date Created : Oct. 11, 2015
// Revision History : Version 1 created : Oct. 11, 2015

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPBusService.Models
{
    public class StartTime
    {
        //equivalent to routeScheduleId type of routeSchedule and trip table
        public string routeScheduleId { get; set; }
        //Type: hh-mm weekday/weekend
        public string timeString { get; set; }
    }
}