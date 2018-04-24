using AcmeRemoteFlights.API.Controllers;
using AcmeRemoteFlights.DataAccess;
using AcmeRemoteFlights.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AcmeRemoteFlights.API.Tests.Controllers
{
    [TestClass]
    public class BookingControllerTests
    {
        [TestMethod]
        public void CreateBookingController()
        {
            var bookingController = new BookingController(null);
            Assert.IsNotNull(bookingController);
        }

        [TestMethod]
        public void GetBookings()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                    .UseInMemoryDatabase(databaseName: "GetBookings")
                    .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.AddRange(
                    new Booking { Id = 1, PassengerName = "Joe Bloggs", Date = new DateTime(2018, 5, 10) },
                    new Booking { Id = 2, PassengerName = "Joe Bloggs", Date = new DateTime(2018, 4, 10) });
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var bookingController = new BookingController(flightContext);
                var result = bookingController.Get(string.Empty, DateTime.MinValue, null, string.Empty, string.Empty);
                Assert.IsTrue(result is ObjectResult);
                var objectResult = result as ObjectResult;
                Assert.IsTrue(objectResult.Value is IQueryable<Booking>);
                var bookings = (objectResult.Value as IQueryable<Booking>).ToList();
                Assert.AreEqual(1, bookings.Count);
                Assert.AreEqual(1, bookings[0].Id);
                Assert.AreEqual("Joe Bloggs", bookings[0].PassengerName);
                Assert.AreEqual(new DateTime(2018, 5, 10), bookings[0].Date);
            }
        }

        [TestMethod]
        public void GetBookingsUsingSearch()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                    .UseInMemoryDatabase(databaseName: "GetBookingsUsingSearch")
                    .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.AddRange(
                    new Booking { Id = 1, PassengerName = "Joe Bloggs", Date = new DateTime(2018, 5, 10) },
                    new Booking { Id = 2, PassengerName = "Joe Bloggs", Date = new DateTime(2018, 4, 10) });
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var bookingController = new BookingController(flightContext);
                var result = bookingController.Get("Bloggs", DateTime.MinValue, null, string.Empty, string.Empty);
                Assert.IsTrue(result is ObjectResult);
                var objectResult = result as ObjectResult;
                Assert.IsTrue(objectResult.Value is IList<Booking>);
                var bookings = (objectResult.Value as IList<Booking>).ToList();
                Assert.AreEqual(2, bookings.Count);
                Assert.AreEqual(1, bookings[0].Id);
                Assert.AreEqual("Joe Bloggs", bookings[0].PassengerName);
                Assert.AreEqual(new DateTime(2018, 5, 10), bookings[0].Date);
                Assert.AreEqual(2, bookings[1].Id);
                Assert.AreEqual("Joe Bloggs", bookings[1].PassengerName);
                Assert.AreEqual(new DateTime(2018, 4, 10), bookings[1].Date);
            }
        }

        [TestMethod]
        public void GetBookingsUsingSearchWithNoResults()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                    .UseInMemoryDatabase(databaseName: "GetBookingsUsingSearchWithNoResults")
                    .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.AddRange(
                    new Booking { Id = 1, PassengerName = "Joe Bloggs", Date = new DateTime(2018, 5, 10) },
                    new Booking { Id = 2, PassengerName = "Joe Bloggs", Date = new DateTime(2018, 4, 10) });
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var bookingController = new BookingController(flightContext);
                var result = bookingController.Get("John", DateTime.MinValue, null, string.Empty, string.Empty);
                Assert.IsTrue(result is NotFoundResult);
            }
        }

        [TestMethod]
        public void GetBooking()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                    .UseInMemoryDatabase(databaseName: "GetBooking")
                    .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.AddRange(
                    new Booking { Id = 1, PassengerName = "Joe Bloggs", Date = new DateTime(2018, 5, 10) });
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var bookingController = new BookingController(flightContext);
                var result = bookingController.Get(1);
                Assert.IsTrue(result is ObjectResult);
                var objectResult = result as ObjectResult;
                Assert.IsTrue(objectResult.Value is Booking);
                var booking = objectResult.Value as Booking;
                Assert.AreEqual(1, booking.Id);
                Assert.AreEqual("Joe Bloggs", booking.PassengerName);
                Assert.AreEqual(new DateTime(2018, 5, 10), booking.Date);
            }
        }

        [TestMethod]
        public void GetBookingInvalidId()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                    .UseInMemoryDatabase(databaseName: "GetBookingInvalidId")
                    .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.AddRange(
                    new Booking { Id = 1, PassengerName = "Joe Bloggs", Date = new DateTime(2018, 5, 10) });
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var bookingController = new BookingController(flightContext);
                var result = bookingController.Get(2);
                Assert.IsTrue(result is NotFoundResult);
            }
        }

        [TestMethod]
        public void CreateBookingWithNoBooking()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                    .UseInMemoryDatabase(databaseName: "CreateBookingWithNoBooking")
                    .Options;

            using (var flightContext = new FlightsContext(options))
            {
                var bookingController = new BookingController(flightContext);
                var result = bookingController.Create(null);
                Assert.IsTrue(result is BadRequestResult);
            }
        }

        [TestMethod]
        public void CreateBookingWithInvalidBooking()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                    .UseInMemoryDatabase(databaseName: "CreateBookingWithInvalidBooking")
                    .Options;

            using (var flightContext = new FlightsContext(options))
            {
                var bookingController = new BookingController(flightContext);
                var result = bookingController.Create(new Booking {});
                Assert.IsTrue(result is BadRequestResult);
            }
        }

        [TestMethod]
        public void CreateBookingWithInvalidFlight()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                    .UseInMemoryDatabase(databaseName: "CreateBookingWithInvalidFlight")
                    .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.Add(new Flight { Number = 1, Capacity = 1 });
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var bookingController = new BookingController(flightContext);
                var result = bookingController.Create(new Booking
                    {
                        PassengerName = "Joe Bloggs",
                        Date = new DateTime(2018, 5, 10),
                        FlightNumber = 2
                    });
                Assert.IsTrue(result is BadRequestResult);
            }
        }

        [TestMethod]
        public void CreateBookingNoCapacity()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                    .UseInMemoryDatabase(databaseName: "CreateBookingNoCapacity")
                    .Options;

            using (var flightContext = new FlightsContext(options))
            {
                var flight = new Flight { Number = 1, Capacity = 1 };
                flightContext.Add(flight);
                flightContext.Add(
                    new Booking
                    {
                        Id = 1,
                        FlightNumber = 1,
                        Flight = flight,
                        Date = new DateTime(2018, 5, 10),
                        PassengerName = "Joe Bloggs"
                    });
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var bookingController = new BookingController(flightContext);
                var result = bookingController.Create(new Booking
                {
                    PassengerName = "Joe Bloggs",
                    Date = new DateTime(2018, 5, 10),
                    FlightNumber = 1
                });
                Assert.IsTrue(result is BadRequestResult);
            }
        }

        [TestMethod]
        public void CreateBooking()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                    .UseInMemoryDatabase(databaseName: "CreateBooking")
                    .Options;

            using (var flightContext = new FlightsContext(options))
            {
                var flight = new Flight { Number = 1, Capacity = 1 };
                flightContext.Add(flight);
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var bookingController = new BookingController(flightContext);
                var result = bookingController.Create(new Booking
                {
                    PassengerName = "Joe Bloggs",
                    Date = new DateTime(2018, 5, 10),
                    FlightNumber = 1
                });
                Assert.IsTrue(result is CreatedAtRouteResult);
                var objectResult = result as CreatedAtRouteResult;
                Assert.IsTrue(objectResult.Value is Booking);
                var bookings = objectResult.Value as Booking;
                Assert.AreEqual(1, bookings.Id);
                Assert.AreEqual("Joe Bloggs", bookings.PassengerName);
                Assert.AreEqual(new DateTime(2018, 5, 10), bookings.Date);
                Assert.AreEqual(1, flightContext.Bookings.Count());
                Assert.AreEqual(1, flightContext.Bookings.First().Id);
                Assert.AreEqual("Joe Bloggs", flightContext.Bookings.First().PassengerName);
                Assert.AreEqual(new DateTime(2018, 5, 10), flightContext.Bookings.First().Date);
            }
        }
    }
}
