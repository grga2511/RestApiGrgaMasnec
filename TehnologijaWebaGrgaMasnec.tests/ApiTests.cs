using System.Net;
using TehnologijaWebaGrgaMasnec;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;


public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    

    public ApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    
    [Fact]
    public async Task ReservationsMine_WithoutToken_Returns401()
    {
        var res = await _client.GetAsync("/api/reservations/mine");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    
    [Fact]
    public async Task TimeSlotsAvailable_Returns200()
    {
        var url = "/api/timeslots/available?roomId=1&from=2026-02-01T00:00:00&to=2026-03-01T00:00:00";
        var res = await _client.GetAsync(url);
        Assert.True(res.IsSuccessStatusCode);
    }

    
    [Fact]
    public async Task TimeSlotsAll_Returns200()
    {
        var res = await _client.GetAsync("/api/timeslots/all");
        Assert.True(res.IsSuccessStatusCode);
    }

    
    [Fact]
    public async Task Rooms_GetAll_Returns200()
    {
        var res = await _client.GetAsync("/api/rooms");
        Assert.True(res.IsSuccessStatusCode);
    }

    
    [Fact]
    public async Task Facilities_Get_Returns200()
    {
        var res = await _client.GetAsync("/api/facilities");
        Assert.True(res.IsSuccessStatusCode);
    }

    
    [Fact]
    public async Task RoomTypes_Get_Returns200()
    {
        var res = await _client.GetAsync("/api/roomtypes");
        Assert.True(res.IsSuccessStatusCode);
    }

    
    [Fact]
    public async Task Auth_Login_Invalid_Returns401()
    {
        var payload = new { email = "nope@nope.local", password = "badpassword" };

        var res = await _client.PostAsJsonAsync("/api/auth/login", payload);

        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }


}

    

