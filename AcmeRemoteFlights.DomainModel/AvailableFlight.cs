using System;

namespace AcmeRemoteFlights.DomainModel
{
    public class AvailableFlight
    {
        public DateTime Date { get; set; }
        public Flight Flight { get; set; }
        public int RemainingCapacity { get; set; }
    }
}
