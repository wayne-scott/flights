using AcmeRemoteFlights.DataAccess;
using AcmeRemoteFlights.DomainModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AcmeRemoteFlights.BLL.Tests
{
    [TestClass]
    public class SearchTests
    {
        [TestMethod]
        public void CreateObject()
        {
            var search = new Search(null);
            Assert.IsNotNull(search);
        }

        #region Search Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchWithNoValidParameters()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "SearchWithNoValidParameters")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                var search = new Search(flightContext);
                try
                {
                    search.SearchForBooking(string.Empty, DateTime.MinValue, null, string.Empty, string.Empty);
                }
                catch (ArgumentException exception)
                {
                    Assert.AreEqual("At least one search argument is required", exception.Message);
                    throw;
                }
            }
        }

        [TestMethod]
        public void SearchWithPassengerName()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "SearchWithPassengerName")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.Bookings.AddRange(
                    new Booking { PassengerName = "Joe Bloggs" },
                    new Booking { PassengerName = "Jane Smith" }
                );
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var search = new Search(flightContext);
                var results = search.SearchForBooking("Bloggs", DateTime.MinValue, null, string.Empty, string.Empty);

                Assert.AreEqual(1, results.Count);
                Assert.AreEqual("Joe Bloggs", results[0].PassengerName);
            }
        }

        [TestMethod]
        public void SearchWithDate()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "SearchWithDate")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.Bookings.AddRange(
                    new Booking { Date = new DateTime(2018, 5, 10) },
                    new Booking { Date = new DateTime(2018, 5, 11) }
                );
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var search = new Search(flightContext);
                var results = search.SearchForBooking(string.Empty, new DateTime(2018, 5, 10), null, string.Empty, string.Empty);

                Assert.AreEqual(1, results.Count);
                Assert.AreEqual(new DateTime(2018, 5, 10), results[0].Date);
            }
        }

        [TestMethod]
        public void SearchWithFlightNumber()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "SearchWithFlightNumber")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.Bookings.AddRange(
                    new Booking { Flight = new Flight { Number = 1 } },
                    new Booking { Flight = new Flight { Number = 2 } }
                );
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var search = new Search(flightContext);
                var results = search.SearchForBooking(string.Empty, DateTime.MinValue, 1, string.Empty, string.Empty);

                Assert.AreEqual(1, results.Count);
                Assert.AreEqual(1, results[0].Flight.Number);
            }
        }

        [TestMethod]
        public void SearchWithArrivalCity()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "SearchWithArrivalCity")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.Bookings.AddRange(
                    new Booking { Flight = new Flight { ArrivalCity = "Camooweal" } },
                    new Booking { Flight = new Flight { ArrivalCity = "Einasleigh" } }
                );
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var search = new Search(flightContext);
                var results = search.SearchForBooking(string.Empty, DateTime.MinValue, null, "Camooweal", string.Empty);

                Assert.AreEqual(1, results.Count);
                Assert.AreEqual("Camooweal", results[0].Flight.ArrivalCity);
            }
        }

        [TestMethod]
        public void SearchWithDepartureCity()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "SearchWithDepartureCity")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.Bookings.AddRange(
                    new Booking { Flight = new Flight { DepartureCity = "Camooweal" } },
                    new Booking { Flight = new Flight { DepartureCity = "Einasleigh" } }
                );
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var search = new Search(flightContext);
                var results = search.SearchForBooking(string.Empty, DateTime.MinValue, null, string.Empty, "Camooweal");

                Assert.AreEqual(1, results.Count);
                Assert.AreEqual("Camooweal", results[0].Flight.DepartureCity);
            }
        }

        [TestMethod]
        public void SearchWithTwoArguments()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "SearchWithTwoArguments")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.Bookings.AddRange(
                    new Booking { PassengerName = "Joe Bloggs", Date = new DateTime(2018, 5, 10) },
                    new Booking { PassengerName = "Jane Bloggs", Date = new DateTime(2018, 5, 11) },
                    new Booking { PassengerName = "Jane Smith", Date = new DateTime(2018, 5, 10) }
                );
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var search = new Search(flightContext);
                var results = search.SearchForBooking("Bloggs", new DateTime(2018, 5, 10), null, string.Empty, string.Empty);

                Assert.AreEqual(1, results.Count);
                Assert.AreEqual("Joe Bloggs", results[0].PassengerName);
                Assert.AreEqual(new DateTime(2018, 5, 10), results[0].Date);
            }
        }

        #endregion

        #region Available Flights Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AvailableFlightsNoValidParameters()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "AvailableFlightsNoValidParameters")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                var search = new Search(flightContext);
                try
                {
                    search.GetAvailableFlights(DateTime.MinValue, DateTime.MinValue, null);
                }
                catch (ArgumentException exception)
                {
                    Assert.AreEqual("Missing required argument.", exception.Message);
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AvailableFlightsWithStartDate()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "AvailableFlightsWithStartDate")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                var search = new Search(flightContext);
                try
                {
                    search.GetAvailableFlights(new DateTime(2018, 5, 10), DateTime.MinValue, null);
                }
                catch (ArgumentException exception)
                {
                    Assert.AreEqual("Missing required argument.", exception.Message);
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AvailableFlightsWithEndDate()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "AvailableFlightsWithEndDate")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                var search = new Search(flightContext);
                try
                {
                    search.GetAvailableFlights(DateTime.MinValue, new DateTime(2018, 5, 10), null);
                }
                catch (ArgumentException exception)
                {
                    Assert.AreEqual("Missing required argument.", exception.Message);
                    throw;
                }
            }
        }

        [TestMethod]
        public void AvailableFlightsWithStartAndEndDate()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "AvailableFlightsWithStartAndEndDate")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.Add(new Flight { Number = 1, Capacity = 1 });
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var search = new Search(flightContext);
                var results = search.GetAvailableFlights(new DateTime(2018, 5, 10), new DateTime(2018, 5, 12), null);
                Assert.AreEqual(3, results.Count);
                Assert.AreEqual(new DateTime(2018, 5, 10), results[0].Date);
                Assert.AreEqual(1, results[0].Flight.Number);
                Assert.AreEqual(1, results[0].RemainingCapacity);
                Assert.AreEqual(new DateTime(2018, 5, 11), results[1].Date);
                Assert.AreEqual(1, results[1].Flight.Number);
                Assert.AreEqual(1, results[1].RemainingCapacity);
                Assert.AreEqual(new DateTime(2018, 5, 12), results[2].Date);
                Assert.AreEqual(1, results[2].Flight.Number);
                Assert.AreEqual(1, results[2].RemainingCapacity);
            }
        }

        [TestMethod]
        public void AvailableFlightsWithStartAndEndDateNumberOfPassengers()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "AvailableFlightsWithStartAndEndDateNumberOfPassengers")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.Add(new Flight { Number = 1, Capacity = 1 });
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var search = new Search(flightContext);
                var results = search.GetAvailableFlights(new DateTime(2018, 5, 10), new DateTime(2018, 5, 12), 1);
                Assert.AreEqual(3, results.Count);
                Assert.AreEqual(new DateTime(2018, 5, 10), results[0].Date);
                Assert.AreEqual(1, results[0].Flight.Number);
                Assert.AreEqual(1, results[0].RemainingCapacity);
                Assert.AreEqual(new DateTime(2018, 5, 11), results[1].Date);
                Assert.AreEqual(1, results[1].Flight.Number);
                Assert.AreEqual(1, results[1].RemainingCapacity);
                Assert.AreEqual(new DateTime(2018, 5, 12), results[2].Date);
                Assert.AreEqual(1, results[2].Flight.Number);
                Assert.AreEqual(1, results[2].RemainingCapacity);
            }
        }

        [TestMethod]
        public void AvailableFlightsWithToManyPassengers()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "AvailableFlightsWithToManyPassengers")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.Add(new Flight { Number = 1, Capacity = 1 });
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var search = new Search(flightContext);
                var results = search.GetAvailableFlights(new DateTime(2018, 5, 10), new DateTime(2018, 5, 12), 2);
                Assert.AreEqual(0, results.Count);
            }
        }

        [TestMethod]
        public void AvailableFlightsWithToManyPassengersDueToBookings()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "AvailableFlightsWithToManyPassengersDueToBookings")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                var flight = new Flight { Number = 1, Capacity = 1 };
                flightContext.Add(flight);
                flightContext.Add(new Booking { Date = new DateTime(2018, 5, 11), FlightNumber = 1, Flight = flight });
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var search = new Search(flightContext);
                var results = search.GetAvailableFlights(new DateTime(2018, 5, 10), new DateTime(2018, 5, 12), 1);
                Assert.AreEqual(2, results.Count);
                Assert.AreEqual(new DateTime(2018, 5, 10), results[0].Date);
                Assert.AreEqual(1, results[0].Flight.Number);
                Assert.AreEqual(1, results[0].RemainingCapacity);
                Assert.AreEqual(new DateTime(2018, 5, 12), results[1].Date);
                Assert.AreEqual(1, results[1].Flight.Number);
                Assert.AreEqual(1, results[1].RemainingCapacity);
            }
        }

        #endregion
    }
}
