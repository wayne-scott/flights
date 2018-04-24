using AcmeRemoteFlights.API.Controllers;
using AcmeRemoteFlights.DataAccess;
using AcmeRemoteFlights.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AcmeRemoteFlights.API.Tests
{
    [TestClass]
    public class FlightControllerTests
    {
        [TestMethod]
        public void CreateFlightController()
        {
            var flightController = new FlightController(null);
            Assert.IsNotNull(flightController);
        }

        [TestMethod]
        public void GetFlights()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "GetFlights")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.Add(new Flight { Number = 1, Capacity = 1 });
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var flightController = new FlightController(flightContext);
                var result = flightController.Get();
                Assert.IsTrue(result is ObjectResult);
                var objectResult = result as ObjectResult;
                Assert.IsTrue(objectResult.Value is DbSet<Flight>);
                var flights = (objectResult.Value as DbSet<Flight>).ToList();
                Assert.AreEqual(1, flights.Count);
                Assert.AreEqual(1, flights[0].Number);
                Assert.AreEqual(1, flights[0].Capacity);
            }
        }

        [TestMethod]
        public void GetFlight()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "GetFlight")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.Add(new Flight { Number = 1, Capacity = 1 });
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var flightController = new FlightController(flightContext);
                var result = flightController.Get(1);
                Assert.IsTrue(result is ObjectResult);
                var objectResult = result as ObjectResult;
                Assert.IsTrue(objectResult.Value is Flight);
                var flight = objectResult.Value as Flight;
                Assert.AreEqual(1, flight.Number);
                Assert.AreEqual(1, flight.Capacity);
            }
        }

        [TestMethod]
        public void GetInvalidFlight()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "GetInvalidFlight")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.Add(new Flight { Number = 1, Capacity = 1 });
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var flightController = new FlightController(flightContext);
                var result = flightController.Get(2);
                Assert.IsTrue(result is NotFoundResult);
            }
        }

        [TestMethod]
        public void GetAvailableFlightNoValidArgumnets()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "GetAvailableFlight")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                var flightController = new FlightController(flightContext);
                var result = flightController.Get(DateTime.MinValue, DateTime.MinValue, null);
                Assert.IsTrue(result is BadRequestResult);
            }
        }

        [TestMethod]
        public void GetAvailableFlightWithStartDate()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "GetAvailableFlightWithStartDate")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                var flightController = new FlightController(flightContext);
                var result = flightController.Get(new DateTime(2018, 5, 10), DateTime.MinValue, null);
                Assert.IsTrue(result is BadRequestResult);
            }
        }

        [TestMethod]
        public void GetAvailableFlightWithEndDate()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "GetAvailableFlightWithEndDate")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                var flightController = new FlightController(flightContext);
                var result = flightController.Get(DateTime.MinValue, new DateTime(2018, 5, 10), null);
                Assert.IsTrue(result is BadRequestResult);
            }
        }

        [TestMethod]
        public void GetAvailableFlightWithStartAndEndDate()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "GetAvailableFlightWithEndDate")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                flightContext.Add(new Flight { Number = 1, Capacity = 1 });
                flightContext.SaveChanges();
            }

            using (var flightContext = new FlightsContext(options))
            {
                var flightController = new FlightController(flightContext);
                var result = flightController.Get(new DateTime(2018, 5, 10), new DateTime(2018, 5, 10), null);
                Assert.IsTrue(result is ObjectResult);
                var objectResult = result as ObjectResult;
                Assert.IsTrue(objectResult.Value is IList<AvailableFlight>);
                var flights = (objectResult.Value as IList<AvailableFlight>).ToList();
                Assert.AreEqual(1, flights.Count);
                Assert.AreEqual(new DateTime(2018, 5, 10), flights[0].Date);
                Assert.AreEqual(1, flights[0].RemainingCapacity);
                Assert.AreEqual(1, flights[0].Flight.Number);
            }
        }

        [TestMethod]
        public void GetAvailableFlightWithInvalidCapacity()
        {
            var options = new DbContextOptionsBuilder<FlightsContext>()
                .UseInMemoryDatabase(databaseName: "GetAvailableFlightWithInvalidCapacity")
                .Options;

            using (var flightContext = new FlightsContext(options))
            {
                var flightController = new FlightController(flightContext);
                var result = flightController.Get(new DateTime(2018, 5, 10), new DateTime(2018, 5, 10), -1);
                Assert.IsTrue(result is BadRequestResult);
            }
        }
    }
}
