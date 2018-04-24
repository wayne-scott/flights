# Acme Remote Flights

This repository contains my implementation of the 'Acme Remote Flights' programming challenge.

A live version of this API can be found [here](http://ws-flights.azurewebsites.net/).  The landing page will display the [Swagger UI](https://swagger.io/swagger-ui/) which provides details on how to interact with the API including sample requests.  A copy of the sameple requests can be found at the bottom of this page.

In addition to this you can also use [Postman](https://www.getpostman.com/) to call endpoints of the API:

[![Run in Postman](https://run.pstmn.io/button.svg)](https://app.getpostman.com/run-collection/94602d34d50ea716ab6e)

## Architecture

This implementation has been written using [ASP.NET Core](https://github.com/aspnet/home) with [.NET Core 2.0](https://github.com/dotnet/core) and [SQLite](https://www.sqlite.org/index.html) database.  The deployment environment for this can be either Windows Server running IIS or Azure App Service.

### Development Environment

The solution will open and run with Visual Studio 2017 providing you have installed the 'ASP.NET and web development' workload during installation.  If not you can update your installation via the Tools -> Get Tools and Features... menu item in Visual Studio.

If you don't have Visual Studio 2017 there is a [free community edition](https://www.visualstudio.com/downloads/).

As with older versions of Visual Studio any missing NuGet packages will be restored when the solution is built.

## Requirement Assumptions

There are a some assumptions I've made regarding the requirements which are outlined below:

* When searching for bookings the search arguments will be joined by AND, e.g. `passengerName == "Bloggs" AND flightNumber == 1`
* When searching for bookings using the passengers name you can use part of the name, i.e. searching for "Bloggs" will return "Joe Bloggs".  It's important to note that the search will be case sensitive.
* When searching for bookings old bookings will also be returned.
* When getting a list of existing bookings only future bookings will be returned.
* Creating bookings will be done for individuals not groups.  Creating group bookings will need to be done by making repeated calls.
* Although checking for capacity exists there is no guarantee that an over subscription for a flight will not happen.


## Design Decisions

### SQLite

To keep complexity to a minimum I decided to use SQLite as the data store.  This proved to be much simpler than trying to incorporate a more traditional database like SQL Server.

### Exception Logging

With brevity in mind I decided not to add exception logging to an external source like Event Viewer, Sumo Logic, etc.  This is not something I would ever do in any other situation.

## Sample Requests

### Flight

```
GET /api/flight

GET /api/flight/1

GET /api/flight/availability?startDate=2018-05-09&amp;endDate=2018-05-15
```

### Booking

```
GET /api/booking
GET /api/booking?passengerName=John
GET /api/booking?passengerName=John&amp;date=2018-05-12

GET /api/booking/1

POST /api/booking
{
    "date": "2018-05-11T00:00:00",
    "passengerName": "Fred Jones",
    "flightNumber": 2
}
```