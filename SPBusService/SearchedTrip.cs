using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SPBusService
{
    public class SearchedTrip
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:ddd dd MMM yyyy}")]
        public DateTime tripDate { get; set; }
        
        public string shortTripDate { get; set; }
        public TimeSpan startTime { get; set; }
        public string fullName { get; set; }
        public int busNumber { get; set; }
        public string comments { get; set; }
    }
}