using AcmeRemoteFlights.BLL;
using AcmeRemoteFlights.DataAccess;
using AcmeRemoteFlights.DomainModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace AcmeRemoteFlights.API.Controllers
{
    [Route("api/[controller]")]
    public class BookingController : Controller
    {
        public FlightsContext FlightsContext { get; }

        public BookingController(FlightsContext flightsContext)
        {
            FlightsContext = flightsContext;
        }

        /// <summary>
        /// Gets a list of all current bookings.
        /// Or the results of searching for a booking.
        /// </summary>
        /// <remarks>
        /// Search parameters are joined by 'AND'.
        /// 
        /// Sample requests:
        /// 
        ///     GET /api/booking
        ///     GET /api/booking?passengerName=John
        ///     GET /api/booking?passengerName=John&amp;date=2018-05-12
        ///     
        /// </remarks>
        /// <param name="passengerName">Any bookings with this passenger.</param>
        /// <param name="date">Any bookings on this date.</param>
        /// <param name="flightNumber">Any bookings with this flight number.</param>
        /// <param name="arrivalCity">Any bookings with this arrival city.</param>
        /// <param name="departureCity">Any bookings with this departure city.</param>
        /// <returns>A list of bookings.</returns>
        /// <response code="200">The bookings requested.</response>
        /// <response code="404">No bookings were found.</response>
        // GET api/booking
        [HttpGet]
        [ProducesResponseType(typeof(Booking[]), 200)]
        [ProducesResponseType(404)]
        public IActionResult Get(string passengerName, DateTime date, int? flightNumber, string arrivalCity, string departureCity)
        {
            if (IsASearch(passengerName, date, arrivalCity, departureCity, flightNumber))
            {
                var search = new Search(FlightsContext);
                var booking = search.SearchForBooking(passengerName, date, flightNumber, arrivalCity, departureCity);
                if (booking?.Count > 0)
                {
                    return new ObjectResult(booking);
                }
                return NotFound();
            }
            return new ObjectResult(FlightsContext.Bookings.Where(b => b.Date >= DateTime.Now));
        }

        private static bool IsASearch(string passengerName, DateTime date, string arrivalCity, string departureCity, int? flightNumber)
        {
            return !string.IsNullOrEmpty(passengerName) || (date != null && date != DateTime.MinValue) ||
                !string.IsNullOrEmpty(arrivalCity) || !string.IsNullOrEmpty(departureCity) || flightNumber.HasValue;
        }

        /// <summary>
        /// Get a specific booking.
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     GET /api/booking/1
        ///     
        /// </remarks>
        /// <param name="bookingId">ID of booking required.</param>
        /// <returns>The booking requested.</returns>
        /// <response code="200">The booking requested.</response>
        /// <response code="404">The booking was not found.</response>
        // POST api/booking
        [HttpGet("{bookingId}", Name = "GetById")]
        [ProducesResponseType(typeof(Booking), 200)]
        [ProducesResponseType(404)]
        public IActionResult Get(int bookingId)
        {
            var booking = FlightsContext.BookingsIncludingFlight.FirstOrDefault(b => b.Id == bookingId);
            if (booking != null)
            {
                return new ObjectResult(booking);
            }
            return NotFound();
        }

        /// <summary>
        /// Create a booking.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/booking
        ///     {
        ///         "date": "2018-05-11T00:00:00",
        ///         "passengerName": "Fred Jones",
        ///         "flightNumber": 2
        ///     }
        ///
        /// </remarks>
        /// <param name="booking">Booking details</param>
        /// <returns>Newly created booking.</returns>
        /// <response code="200">Newly created booking.</response>
        /// <response code="404">Invalid parameters.  Flight at maximum capacity.</response>
        [HttpPost]
        public IActionResult Create([FromBody] Booking booking)
        {
            if (booking == null || !booking.IsValid()) { return BadRequest(); }

            var flight = FlightsContext.FlightsIncludingBookings.FirstOrDefault(f => f.Number == booking.FlightNumber);

            if (flight == null) { return BadRequest(); }

            booking.Flight = flight;
            if (flight.Bookings.Count == flight.Capacity) { return BadRequest(); }

            FlightsContext.Bookings.Add(booking);
            FlightsContext.SaveChanges();

            booking.Flight = flight.Clone() as Flight; // we don't want to send additional booking details back so clone the flight object.
            return CreatedAtRoute("GetById", new { bookingId = booking.Id }, booking);
        }
    }
}
