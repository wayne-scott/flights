using AcmeRemoteFlights.DataAccess;
using AcmeRemoteFlights.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AcmeRemoteFlights.BLL
{
    public class Search
    {
        private FlightsContext FlightsContext { get; }

        public Search(FlightsContext flightsContext)
        {
            FlightsContext = flightsContext;
        }

        public IList<Booking> SearchForBooking(string passengerName, DateTime date, int? flightNumber, string arrivalCity, string departureCity)
        {
            return FilterBookings(passengerName, date, flightNumber, arrivalCity, departureCity).ToList();
        }

        public IList<AvailableFlight> GetAvailableFlights(DateTime startDate, DateTime endDate, int? numberOfPassengers)
        {
            if (startDate == null || startDate == DateTime.MinValue || endDate == null || endDate == DateTime.MinValue)
            {
                throw new ArgumentException("Missing required argument.");
            }
            var existingBookings = FilterBookings(startDate, endDate);

            var availableFlights = new List<AvailableFlight>();
            foreach (var date in AllDatesBetween(startDate, endDate))
            {
                foreach (var flight in FlightsContext.Flights)
                {
                    var bookings = existingBookings.Count(b => b.Date == date && b.FlightNumber == flight.Number);
                    if ((flight.Capacity - bookings - (numberOfPassengers ?? 0)) >= 0)
                    {
                        availableFlights.Add(
                            new AvailableFlight
                            {
                                Date = date,
                                Flight = flight.Clone() as Flight,
                                RemainingCapacity = (flight.Capacity - bookings)
                            });
                    }
                }
            }
            return availableFlights;
        }

        private IEnumerable<DateTime> AllDatesBetween(DateTime startDate, DateTime endDate)
        {
            for (var day = startDate.Date; day <= endDate; day = day.AddDays(1))
                yield return day;
        }

        private IEnumerable<Booking> FilterBookings(DateTime startDate, DateTime endDate)
        {
            var filters = new List<Filter>();
            if (startDate != null && startDate != DateTime.MinValue)
            {
                filters.Add(new Filter
                {
                    PropertyName = "Date",
                    Comparison = Filter.ComparisonType.GreaterThanOrEqual,
                    Value = startDate
                });
            }
            if (endDate != null && endDate != DateTime.MinValue)
            {
                filters.Add(new Filter
                {
                    PropertyName = "Date",
                    Comparison = Filter.ComparisonType.LessThanOrEqual,
                    Value = endDate
                });
            }
            return FlightsContext.BookingsIncludingFlight.Where(CreateFilterFunction(filters));
        }

        private IEnumerable<Booking> FilterBookings(string passengerName, DateTime date, int? flightNumber, string arrivalCity, string departureCity)
        {
            var filters = new List<Filter>();
            if (!string.IsNullOrEmpty(passengerName))
            {
                filters.Add(new Filter
                {
                    PropertyName = "passengerName",
                    Comparison = Filter.ComparisonType.Contains,
                    Value = passengerName
                });
            }
            if (date != null && date != DateTime.MinValue)
            {
                filters.Add(new Filter
                {
                    PropertyName = "Date",
                    Comparison = Filter.ComparisonType.Equal,
                    Value = date
                });
            }
            if (flightNumber.HasValue)
            {
                filters.Add(new Filter
                {
                    PropertyName = "Flight.Number",
                    Comparison = Filter.ComparisonType.Equal,
                    Value = flightNumber.Value
                });
            }
            if (!string.IsNullOrEmpty(arrivalCity))
            {
                filters.Add(new Filter
                {
                    PropertyName = "Flight.ArrivalCity",
                    Comparison = Filter.ComparisonType.Contains,
                    Value = arrivalCity
                });
            }
            if (!string.IsNullOrEmpty(departureCity))
            {
                filters.Add(new Filter
                {
                    PropertyName = "Flight.DepartureCity",
                    Comparison = Filter.ComparisonType.Contains,
                    Value = departureCity
                });
            }
            return FlightsContext.BookingsIncludingFlight.Where(CreateFilterFunction(filters));
        }

        private struct Filter
        {
            public enum ComparisonType
            {
                Equal,
                LessThanOrEqual,
                GreaterThanOrEqual,
                Contains,
            }

            public string PropertyName { get; set; }
            public object Value { get; set; }
            public ComparisonType Comparison { get; set; }
        }

        private Func<Booking, bool> CreateFilterFunction(List<Filter> filters)
        {
            if (filters.Count == 0) { throw new ArgumentException("At least one search argument is required"); }

            var parameterExpression = Expression.Parameter(typeof(Booking), "t");
            var expression = GetExpression(parameterExpression, filters[0]);

            if (filters.Count > 1)
            {
                for (int i = 1; i < filters.Count; i++)
                {
                    expression = Expression.And(expression, GetExpression(parameterExpression, filters[i]));
                }
            }

            return Expression.Lambda<Func<Booking, bool>>(expression, parameterExpression).Compile();
        }

        private Expression GetExpression(ParameterExpression parameterExpression, Filter filter)
        {
            Expression expression = parameterExpression;
            foreach (var member in filter.PropertyName.Split('.'))
            {
                expression = Expression.Property(expression, member);
            }
            var constant = Expression.Constant(filter.Value);

            switch (filter.Comparison)
            {
                case Filter.ComparisonType.Equal:
                    return Expression.Equal(expression, constant);
                case Filter.ComparisonType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(expression, constant);
                case Filter.ComparisonType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(expression, constant);
                case Filter.ComparisonType.Contains:
                    return Expression.Call(expression, typeof(string).GetMethod("Contains"), constant);
                default:
                    return null;
            }
        }
    }
}
