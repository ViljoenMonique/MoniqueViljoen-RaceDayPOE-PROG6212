namespace RaceDay.Contracts
{
    // ========== AUTH ==========
    public class RegisterRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class AuthResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
    // ========== PROFILE ==========
    public class ProfileDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
    public class UpdateProfileRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }
    // ========== EVENTS ==========
    public class CreateEventRequest
    {
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal? DistanceKm { get; set; }
        public string EventType { get; set; } = string.Empty;
        public int? MaxParticipants { get; set; }
    }
    public class UpdateEventRequest
    {
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal? DistanceKm { get; set; }
        public string EventType { get; set; } = string.Empty;
        public int? MaxParticipants { get; set; }
        public string Status { get; set; } = "Open";
    }
    public class EventDto
    {
        public int EventId { get; set; }
        public int OrganiserId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal? DistanceKm { get; set; }
        public string EventType { get; set; } = string.Empty;
        public int? MaxParticipants { get; set; }
        public string Status { get; set; } = string.Empty;
    }
    // ========== EVENT IMAGES ==========
    public class EventImageDto
    {
        public int EventImageId { get; set; }
        public int EventId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public bool IsPrimary { get; set; }
    }
    // ========== CATEGORIES ==========
    public class CreateCategoryRequest
    {
        public int EventId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal? DistanceKm { get; set; }
        public int? AgeMin { get; set; }
        public int? AgeMax { get; set; }
        public string? GenderRestriction { get; set; }
        public decimal EntryFee { get; set; }
    }
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public int EventId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal? DistanceKm { get; set; }
        public int? AgeMin { get; set; }
        public int? AgeMax { get; set; }
        public string? GenderRestriction { get; set; }
        public decimal EntryFee { get; set; }
    }
    // ========== ENROLMENTS ==========
    public class CreateEnrolmentRequest
    {
        public int EventId { get; set; }
        public int CategoryId { get; set; }
    }
    public class EnrolmentDto
    {
        public int EnrolmentId { get; set; }
        public int ParticipantId { get; set; }
        public int EventId { get; set; }
        public int CategoryId { get; set; }
        public DateTime EnrolmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
    // ========== RESULTS ==========
    public class CreateResultRequest
    {
        public int EnrolmentId { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? Position { get; set; }
        public string Status { get; set; } = "Finished";
        public string? Notes { get; set; }
    }
    public class ResultDto
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? Position { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
