using System;
using System.Collections.Generic;

namespace HoloCrew.Models
{
    public class TrackingInfo
    {
        public string TrackingNumber { get; set; }
        public string Carrier { get; set; }
        public DateTime ShipDate { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public string CurrentStatus { get; set; }
        public string CurrentLocation { get; set; }
        public List<TrackingEvent> Events { get; set; }
    }

    public class TrackingEvent
    {
        public DateTime Timestamp { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}