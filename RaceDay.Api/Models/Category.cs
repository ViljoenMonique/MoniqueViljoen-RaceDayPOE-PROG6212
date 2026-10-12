namespace RaceDay.Api.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public int EventId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal? DistanceKm { get; set; }
        public int? AgeMin { get; set; }
        public int? AgeMax { get; set; }
        public string? GenderRestriction { get; set; }
        public decimal EntryFee { get; set; }

        // Navigation properties
        public Event? Event { get; set; }
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}
