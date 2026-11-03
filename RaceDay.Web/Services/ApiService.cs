using System.Net;
using System.Net.Http.Json;
using RaceDay.Contracts;

namespace RaceDay.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly CookieContainer _cookieContainer = new CookieContainer();

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private void ApplyCookies()
        {
            var cookieHeader = _cookieContainer.GetCookieHeader(new Uri("https://localhost:7187/"));
            if (!string.IsNullOrEmpty(cookieHeader))
            {
                _httpClient.DefaultRequestHeaders.Remove("Cookie");
                _httpClient.DefaultRequestHeaders.Add("Cookie", cookieHeader);
            }
        }

        // ========== AUTH ==========
        public async Task<AuthResponse?> RegisterAsync(RaceDay.Contracts.RegisterRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }

        public async Task<AuthResponse?> LoginAsync(RaceDay.Contracts.LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            if (!response.IsSuccessStatusCode) return null;

            if (response.Headers.TryGetValues("Set-Cookie", out var setCookies))
            {
                foreach (var cookie in setCookies)
                {
                    _cookieContainer.SetCookies(new Uri("https://localhost:7187/"), cookie);
                }
            }

            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }

        // ========== EVENTS ==========
        public async Task<List<EventDto>?> GetAllEventsAsync()
        {
            var response = await _httpClient.GetAsync("api/events");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<List<EventDto>>();
        }

        public async Task<EventDto?> GetEventByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/events/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<EventDto>();
        }

        public async Task<EventDto?> CreateEventAsync(CreateEventRequest request)
        {
            ApplyCookies();
            var response = await _httpClient.PostAsJsonAsync("api/events", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<EventDto>();
        }

        public async Task<List<EventImageDto>?> GetEventImagesAsync(int eventId)
        {
            ApplyCookies();
            var response = await _httpClient.GetAsync($"api/events/{eventId}/images");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<List<EventImageDto>>();
        }

        public async Task<EventImageDto?> UploadEventImageAsync(int eventId, IFormFile imageFile, string? caption)
        {
            ApplyCookies();

            using var content = new MultipartFormDataContent();
            using var stream = imageFile.OpenReadStream();
            content.Add(new StreamContent(stream), "imageFile", imageFile.FileName);

            if (!string.IsNullOrEmpty(caption))
                content.Add(new StringContent(caption), "caption");

            var response = await _httpClient.PostAsync($"api/events/{eventId}/images", content);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<EventImageDto>();
        }

        // ========== CATEGORIES ==========
        public async Task<List<CategoryDto>?> GetCategoriesByEventAsync(int eventId)
        {
            ApplyCookies();
            var response = await _httpClient.GetAsync($"api/categories/event/{eventId}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
        }

        public async Task<CategoryDto?> CreateCategoryAsync(int eventId, CreateCategoryRequest request)
        {
            ApplyCookies();
            request.EventId = eventId;
            var response = await _httpClient.PostAsJsonAsync("api/categories", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<CategoryDto>();
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            ApplyCookies();
            var response = await _httpClient.DeleteAsync($"api/categories/{id}");
            return response.IsSuccessStatusCode;
        }

        // ========== ENROLMENTS ==========
        public async Task<EnrolmentDto?> CreateEnrolmentAsync(CreateEnrolmentRequest request)
        {
            ApplyCookies();
            var response = await _httpClient.PostAsJsonAsync("api/enrolments", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<EnrolmentDto>();
        }

        public async Task<List<EnrolmentDto>?> GetEnrolmentsByEventAsync(int eventId)
        {
            ApplyCookies();
            var response = await _httpClient.GetAsync($"api/enrolments/event/{eventId}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<List<EnrolmentDto>>();
        }

        public async Task<List<EnrolmentDto>?> GetMyEnrolmentsAsync()
        {
            ApplyCookies();
            var response = await _httpClient.GetAsync("api/enrolments/me");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<List<EnrolmentDto>>();
        }

        // ========== RESULTS ==========
        public async Task<ResultDto?> CreateResultAsync(CreateResultRequest request)
        {
            ApplyCookies();
            var response = await _httpClient.PostAsJsonAsync("api/results", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {error}");
            }
            return await response.Content.ReadFromJsonAsync<ResultDto>();
        }

        public async Task<List<ResultDto>?> GetMyResultsAsync()
        {
            ApplyCookies();
            var response = await _httpClient.GetAsync("api/results/me");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<List<ResultDto>>();
        }

        // ========== PROFILE ==========
        public async Task<ProfileDto?> GetMyProfileAsync()
        {
            ApplyCookies();
            var response = await _httpClient.GetAsync("api/profile/me");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ProfileDto>();
        }

        public async Task<ProfileDto?> UpdateProfileAsync(UpdateProfileRequest request)
        {
            ApplyCookies();
            var response = await _httpClient.PutAsJsonAsync("api/profile/me", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ProfileDto>();
        }

        public async Task<string?> UploadProfileImageAsync(IFormFile imageFile)
        {
            ApplyCookies();

            using var content = new MultipartFormDataContent();
            using var stream = imageFile.OpenReadStream();
            content.Add(new StreamContent(stream), "imageFile", imageFile.FileName);

            var response = await _httpClient.PostAsync("api/profile/me/image", content);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            return result?["profileImageUrl"];
        }

        public async Task<EventDto?> UpdateEventAsync(int id, UpdateEventRequest request)
        {
            ApplyCookies();
            var response = await _httpClient.PutAsJsonAsync($"api/events/{id}", request);
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
                return new EventDto();

            return System.Text.Json.JsonSerializer.Deserialize<EventDto>(content);
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            ApplyCookies();
            var response = await _httpClient.DeleteAsync($"api/events/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
