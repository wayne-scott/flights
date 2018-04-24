using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AcmeRemoteFlights.DomainModel
{
    public class Flight : ICloneable
    {
        [Key]
        public int Number { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Capacity { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }

        public List<Booking> Bookings { get; set; }

        public object Clone()
        {
            return new Flight
            {
                Number = Number,
                StartTime = StartTime,
                EndTime = EndTime,
                Capacity = Capacity,
                DepartureCity = DepartureCity,
                ArrivalCity = ArrivalCity
            };
        }
    }
}