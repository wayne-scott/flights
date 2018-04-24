using AcmeRemoteFlights.BLL;
using AcmeRemoteFlights.DataAccess;
using AcmeRemoteFlights.DomainModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace AcmeRemoteFlights.API.Controllers
{
    [Route("api/[controller]")]
    public class FlightController : Controller
    {
        public FlightsContext FlightsContext { get; }

        public FlightController(FlightsContext flightsContext)
        {
            FlightsContext = flightsContext;
        }

        /// <summary>
        /// Get a list of all flights.
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     GET /api/flight
        ///     
        /// </remarks>
        /// <returns>A list of flights</returns>
        /// <response code="200">A list of flights</response>
        // GET api/flight
        [HttpGet]
        [ProducesResponseType(typeof(Flight[]), 200)]
        public IActionResult Get()
        {
            return new ObjectResult(FlightsContext.Flights);
        }

        /// <summary>
        /// Get a specific flight.
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     GET /api/flight/1
        ///     
        /// </remarks>
        /// <param name="flightNumber">Number of flight required.</param>
        /// <returns>The flight requested.</returns>
        /// <response code="200">The flight requested.</response>
        /// <response code="404">The flight was not found.</response>
        // GET api/flight/5
        [HttpGet("{flightNumber}")]
        [ProducesResponseType(typeof(Flight), 200)]
        [ProducesResponseType(404)]
        public IActionResult Get(int flightNumber)
        {
            var flight = FlightsContext.Flights.FirstOrDefault(f => f.Number == flightNumber);
            if (flight != null)
            {
                return new ObjectResult(flight);
            }
            return NotFound();
        }

        /// <summary>
        /// Get a list of available flights
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     GET /api/flight/availability?startDate=2018-05-09&amp;endDate=2018-05-15
        ///     
        /// </remarks>
        /// <param name="startDate">Start of the date range.</param>
        /// <param name="endDate">End of the date range.</param>
        /// <param name="numberOfPassengers">Number of passengers flying.</param>
        /// <returns>List of available flights between the date range.</returns>
        /// <response code="200">List of available flights.</response>
        /// <response code="404">Invalid parameters.</response>
        [HttpGet(template: "available")]
        [ProducesResponseType(typeof(AvailableFlight[]), 200)]
        [ProducesResponseType(404)]
        public IActionResult Get(DateTime startDate, DateTime endDate, int? numberOfPassengers)
        {
            if (!ValidParameters(startDate, endDate, numberOfPassengers))
            {
                return BadRequest();
            }
            var search = new Search(FlightsContext);
            var availableFlights = search.GetAvailableFlights(startDate, endDate, numberOfPassengers);
            return new ObjectResult(availableFlights);
        }

        private static bool ValidParameters(DateTime startDate, DateTime endDate, int? numberOfPassengers)
        {
            return startDate != null && startDate != DateTime.MinValue && endDate != null && endDate != DateTime.MinValue && startDate <= endDate
                && (endDate - startDate).Days < 30 && (numberOfPassengers.HasValue ? numberOfPassengers.Value > 0 : true);
        }
    }
}
