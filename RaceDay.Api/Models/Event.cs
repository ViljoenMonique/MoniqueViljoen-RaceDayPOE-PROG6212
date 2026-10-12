namespace RaceDay.Api.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public int OrganiserId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal? DistanceKm { get; set; }
        public string EventType { get; set; } = string.Empty; // Running, Walking, Cycling
        public int? MaxParticipants { get; set; }
        public string Status { get; set; } = "Open";

        // Navigation properties
        public AppUser? Organiser { get; set; }
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}
