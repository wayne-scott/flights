using AcmeRemoteFlights.DomainModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AcmeRemoteFlights.DataAccess
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new FlightsContext(
                serviceProvider.GetRequiredService<DbContextOptions<FlightsContext>>()))
            {
                context.Database.EnsureCreated();

                // Look for any movies.
                if (context.Flights.Any())
                {
                    return;   // DB has been seeded
                }

                context.Flights.AddRange(
                     new Flight
                     {
                         Number = 1,
                         Capacity = 12,
                         StartTime = new TimeSpan(6, 0, 0),
                         EndTime = new TimeSpan(6, 45, 0),
                         DepartureCity = "Muttaburra",
                         ArrivalCity = "Camooweal"
                     },
                    new Flight
                    {
                        Number = 2,
                        Capacity = 4,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(10, 15, 0),
                        DepartureCity = "Thargomindah",
                        ArrivalCity = "Einasleigh",
                        Bookings = new List<Booking>
                        {
                            new Booking
                            {
                                Id = 1,
                                Date = new DateTime(2018, 5, 10),
                                FlightNumber = 2,
                                PassengerName = "Jane Ford"
                            }
                        }
                    },
                    new Flight
                    {
                        Number = 3,
                        Capacity = 6,
                        StartTime = new TimeSpan(13, 0, 0),
                        EndTime = new TimeSpan(15, 0, 0),
                        DepartureCity = "Parachilna",
                        ArrivalCity = "Betoota",
                        Bookings = new List<Booking>
                        {
                            new Booking
                            {
                                Id = 2,
                                Date = new DateTime(2018, 5, 12),
                                FlightNumber = 3,
                                PassengerName = "John Smith"
                            }
                        }
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
