using AcmeRemoteFlights.DomainModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;

namespace AcmeRemoteFlights.DataAccess
{
    public class FlightsContext : DbContext
    {
        public FlightsContext(DbContextOptions options) : base (options) {}

        public DbSet<Flight> Flights { get; set; }
        public IIncludableQueryable<Flight, List<Booking>> FlightsIncludingBookings => Flights.Include(f => f.Bookings);
        public DbSet<Booking> Bookings { get; set; }
        public IQueryable<Booking> BookingsIncludingFlight => Bookings.Include(b => b.Flight);
    }
}