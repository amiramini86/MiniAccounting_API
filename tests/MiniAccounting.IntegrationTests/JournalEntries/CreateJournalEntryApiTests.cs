using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace MiniAccounting.IntegrationTests.JournalEntries;

public class CreateJournalEntryApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CreateJournalEntryApiTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostJournalEntry_WithBalancedLines_ReturnsCreated()
    {
        // Arrange
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var account1Response = await _client.PostAsJsonAsync(
            "/api/accounts",
            new
            {
                code = $"C{suffix}1",
                name = "Test Cash"
            });

        //account1Response.EnsureSuccessStatusCode();
        if (!account1Response.IsSuccessStatusCode)
        {
            var error = await account1Response.Content.ReadAsStringAsync();

            throw new Exception(
                $"Create account failed. Status: {(int)account1Response.StatusCode}. Response: {error}");
        }
        var account1Json =
            await account1Response.Content.ReadFromJsonAsync<JsonElement>();

        var account1Id =
            account1Json.GetProperty("id").GetInt32();

        var account2Response = await _client.PostAsJsonAsync(
            "/api/accounts",
            new
            {
                code = $"C{suffix}2",
                name = "Test Capital"
            });

        account2Response.EnsureSuccessStatusCode();

        var account2Json =
            await account2Response.Content.ReadFromJsonAsync<JsonElement>();

        var account2Id =
            account2Json.GetProperty("id").GetInt32();

        var request = new
        {
            date = DateTime.UtcNow,
            description = "API integration test",
            lines = new[]
            {
                new
                {
                    accountId = account1Id,
                    debit = 1000m,
                    credit = 0m
                },
                new
                {
                    accountId = account2Id,
                    debit = 0m,
                    credit = 1000m
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/journal-entries",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    
    
    
    [Fact]
    public async Task GetJournalEntry_WhenEntryDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync(
            "/api/journal-entries/999999999");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task GetJournalEntry_WhenIdIsInvalid_ReturnsBadRequest()
    {
        var response = await _client.GetAsync(
            "/api/journal-entries/invalid");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }
    
    
    [Fact]
    public async Task PostJournalEntry_WithUnbalancedLines_ReturnsBadRequest()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var account1Response = await _client.PostAsJsonAsync(
            "/api/accounts",
            new { code = $"D{suffix}1", name = "Test Debit Account" });

        account1Response.EnsureSuccessStatusCode();

        var account1Json =
            await account1Response.Content.ReadFromJsonAsync<JsonElement>();

        var account1Id = account1Json.GetProperty("id").GetInt32();

        var account2Response = await _client.PostAsJsonAsync(
            "/api/accounts",
            new { code = $"C{suffix}2", name = "Test Credit Account" });

        account2Response.EnsureSuccessStatusCode();

        var account2Json =
            await account2Response.Content.ReadFromJsonAsync<JsonElement>();

        var account2Id = account2Json.GetProperty("id").GetInt32();

        var request = new
        {
            date = DateTime.UtcNow,
            description = "Unbalanced journal entry",
            lines = new[]
            {
                new { accountId = account1Id, debit = 1000m, credit = 0m },
                new { accountId = account2Id, debit = 0m, credit = 800m }
            }
        };

        var response = await _client.PostAsJsonAsync(
            "/api/journal-entries",
            request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}