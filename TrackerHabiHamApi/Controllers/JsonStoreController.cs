using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;
using TrackerHabiHamApi.Services;

namespace TrackerHabiHamApi.Controllers
{
    [ApiController]
    [Route("api/json")]
    public class JsonStoreController : ControllerBase
    {
        private readonly IJsonStoreService _jsonStoreService;

        public JsonStoreController(IJsonStoreService jsonStoreService)
        {
            _jsonStoreService = jsonStoreService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromQuery] string? id = null, CancellationToken ct = default)
        {
            try
            {
                // Read raw JSON from request body
                using var reader = new StreamReader(Request.Body);
                var jsonString = await reader.ReadToEndAsync(ct);
                var obj = new { DateTime = DateTime.Now };
                jsonString = JsonSerializer.Serialize(obj);


                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    return BadRequest(new { message = "Request body cannot be empty." });
                }

                JsonElement jsonElement;
                try
                {
                    jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonString);
                }
                catch (JsonException)
                {
                    return BadRequest(new { message = "Invalid JSON format." });
                }

                if (jsonElement.ValueKind != JsonValueKind.Object)
                {
                    return BadRequest(new { message = "Only JSON objects are allowed. Arrays, strings, and numbers are not accepted." });
                }

                var bsonDocument = BsonDocument.Parse(jsonString);                     
                var createdId = await _jsonStoreService.CreateAsync(bsonDocument, id, ct);

                return Ok(new { id = createdId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetAsync(string id, CancellationToken ct = default)
        {
            try
            {
                // Validate id is not empty
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest(new { message = "Id cannot be empty." });
                }

                var payload = await _jsonStoreService.GetAsync(id, ct);

                if (payload == null)
                {
                    return NotFound();
                }

                // Convert BsonDocument to JsonElement and return as JSON
                var jsonString = payload.ToJson();
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonString);
                return Ok(jsonElement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet("check")]
        public async Task<ActionResult> CheckConnectionAsync(CancellationToken ct = default)
        {
            try
            {
                var isConnected = await _jsonStoreService.CheckConnectionAsync(ct);
                
                if (isConnected)
                {
                    return Ok(new { connected = true, message = "MongoDB connection is healthy." });
                }
                else
                {
                    return StatusCode(503, new { connected = false, message = "MongoDB connection failed." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { connected = false, message = $"An error occurred while checking connection: {ex.Message}" });
            }
        }
    }
}

