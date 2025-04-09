using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SecureFlight.Api.Models;
using SecureFlight.Api.Utils;
using SecureFlight.Core.Entities;
using SecureFlight.Core.Interfaces;

namespace SecureFlight.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class FlightsController(IService<Flight> flightService, IRepository<Flight> flightRepository, IRepository<Passenger> passengerRepository, IRepository<PassengerFlight> passengerFlightRepository, IMapper mapper)
    : SecureFlightBaseController(mapper)
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FlightDataTransferObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseActionResult))]
    public async Task<IActionResult> Get()
    {
        var flights = await flightService.GetAllAsync();
        return MapResultToDataTransferObject<IReadOnlyList<Flight>, IReadOnlyList<FlightDataTransferObject>>(flights);
    }

    [HttpPost]
    [ProducesResponseType(typeof(FlightDataTransferObject), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseActionResult))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseActionResult))]
    public async Task<IActionResult> Create([FromBody] FlightDataTransferObject flightDto)
    {

        var flight = mapper.Map<FlightDataTransferObject, Flight>(flightDto);
        var createdFlight = await flightRepository.Create(flight);
        return MapResultToDataTransferObject<Flight, FlightDataTransferObject>(createdFlight);
    }

    [HttpPost("flight-passanger")]
    //[ProducesResponseType(typeof(PassengerFlight), StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseActionResult))]
    public async Task<IActionResult> AddPassengerToFlight([FromBody] PassengerFlightRequest request)
    {
        var flight = await flightRepository.GetByIdAsync(long.Parse(request.CodeFlight));
        if (flight is null)
        {
            return NotFound($"Flight with code {request.CodeFlight} not found.");
        }

        var passenger = await passengerRepository.GetByIdAsync(request.PassengerId);
        if (passenger is null)
        {
            return NotFound($"Passenger with ID {request.PassengerId} not found.");
        }

        var passengerFlight = new PassengerFlight
        {
            PassengerId = passenger.Id,
            Passenger = passenger,
            Flight = flight,
            FlightId = flight.Id
        };

        var result = await passengerFlightRepository.Create(passengerFlight);

        return Ok(result);
    }


}