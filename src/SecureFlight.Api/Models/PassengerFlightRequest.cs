namespace SecureFlight.Api.Models
{
    public class PassengerFlightRequest
    {
        public required string PassengerId { get; set; }
        public required string CodeFlight { get; set; }
    }
}
