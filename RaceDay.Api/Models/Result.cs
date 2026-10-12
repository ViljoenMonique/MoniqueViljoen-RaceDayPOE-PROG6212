namespace RaceDay.Api.Models
{
    public class Result
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? Position { get; set; }
        public string Status { get; set; } = "Finished"; // Finished, DNF, DNS
        public string? Notes { get; set; }

        // Navigation property
        public Enrolment? Enrolment { get; set; }
    }
}
