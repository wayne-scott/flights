using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcmeRemoteFlights.DomainModel
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string PassengerName { get; set; }

        public int FlightNumber { get; set; }
        [ForeignKey("FlightForeignKey")]
        public Flight Flight { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(PassengerName) && Date != null && Date != DateTime.MinValue && FlightNumber > 0;
        }
    }
}