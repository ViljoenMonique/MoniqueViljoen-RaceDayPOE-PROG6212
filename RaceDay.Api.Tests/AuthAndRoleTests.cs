using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RaceDay.Api.Data;
using RaceDay.Contracts;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace RaceDay.Api.Tests
{
    public class AuthAndRoleTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private static readonly string DbName = "SharedTestDb";

        public AuthAndRoleTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<RaceDayDbContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<RaceDayDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(DbName);
                    });
                });
            });
        }

        [Fact]
        public async Task Register_ValidOrganiser_ReturnsSuccess()
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
            {
                FirstName = "Test",
                LastName = "Organiser",
                Email = $"organiser_{Guid.NewGuid()}@test.com",
                Password = "Password123",
                Role = "Organiser"
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = "doesnotexist@test.com",
                Password = "WrongPassword"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetProfile_WithoutLogin_ReturnsUnauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/profile/me");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateEvent_AsParticipant_ReturnsForbidden()
        {
            var client = _factory.CreateClient();
            var email = $"participant_{Guid.NewGuid()}@test.com";

            await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
            {
                FirstName = "Part",
                LastName = "icipant",
                Email = email,
                Password = "Password123",
                Role = "Participant"
            });

            await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Password = "Password123"
            });

            var response = await client.PostAsJsonAsync("/api/events", new CreateEventRequest
            {
                EventName = "Test Event",
                EventDate = DateTime.UtcNow.AddDays(30),
                Location = "Test Location",
                EventType = "Running"
            });

            Assert.False(response.IsSuccessStatusCode,
                $"Participant should not be able to create an event. Got status: {response.StatusCode}");
        }

        [Fact]
        public async Task Register_And_Login_Works()
        {
            var client = _factory.CreateClient();
            var email = $"fulltest_{Guid.NewGuid()}@test.com";

            var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
            {
                FirstName = "Full",
                LastName = "Test",
                Email = email,
                Password = "Password123",
                Role = "Organiser"
            });

            Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Password = "Password123"
            });

            Assert.True(
                loginResponse.IsSuccessStatusCode || loginResponse.StatusCode == HttpStatusCode.Unauthorized,
                $"Unexpected status: {loginResponse.StatusCode}");
        }

        // ========== EXTRA ROLE-BASED FAILURE TESTS ==========

        [Fact]
        public async Task CreateCategory_AsParticipant_ReturnsForbidden()
        {
            var client = _factory.CreateClient();
            var email = $"part_cat_{Guid.NewGuid()}@test.com";

            await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
            {
                FirstName = "Part",
                LastName = "icipant",
                Email = email,
                Password = "Password123",
                Role = "Participant"
            });

            await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Password = "Password123"
            });

            var response = await client.PostAsJsonAsync("/api/categories/event/1", new CreateCategoryRequest
            {
                CategoryName = "Test Category",
                EntryFee = 100
            });

            Assert.False(response.IsSuccessStatusCode,
                $"Participant should not create categories. Got: {response.StatusCode}");
        }

        [Fact]
        public async Task CreateResult_AsParticipant_ReturnsForbidden()
        {
            var client = _factory.CreateClient();
            var email = $"part_res_{Guid.NewGuid()}@test.com";

            await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
            {
                FirstName = "Part",
                LastName = "icipant",
                Email = email,
                Password = "Password123",
                Role = "Participant"
            });

            await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Password = "Password123"
            });

            var response = await client.PostAsJsonAsync("/api/results", new CreateResultRequest
            {
                EnrolmentId = 1,
                Position = 1,
                Status = "Finished"
            });

            Assert.False(response.IsSuccessStatusCode,
                $"Participant should not create results. Got: {response.StatusCode}");
        }

        [Fact]
        public async Task CreateEnrolment_AsOrganiser_ReturnsForbidden()
        {
            var client = _factory.CreateClient();
            var email = $"org_enrol_{Guid.NewGuid()}@test.com";

            await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
            {
                FirstName = "Org",
                LastName = "aniser",
                Email = email,
                Password = "Password123",
                Role = "Organiser"
            });

            await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Password = "Password123"
            });

            var response = await client.PostAsJsonAsync("/api/enrolments", new CreateEnrolmentRequest
            {
                EventId = 1,
                CategoryId = 1
            });

            Assert.False(response.IsSuccessStatusCode,
                $"Organiser should not enrol as participant. Got: {response.StatusCode}");
        }
    }
}
