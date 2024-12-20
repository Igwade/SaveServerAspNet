using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SaveLoadApp.Data;
using SaveLoadApp.Models;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace SaveLoadApp.Controllers
{
    [ApiController]
    [Route("/")]
    public class RecordsController(AppDbContext context) : ControllerBase
    {
        [HttpPut("save")]
        public async Task<IActionResult> Save([FromQuery] string version, [FromBody] object content)
        {
            if (string.IsNullOrEmpty(version) || content == null)
            {
                return BadRequest("Version and content are required.");
            }

            string contentString;

            if (content is JsonElement jsonElement)
            {
                contentString = jsonElement.GetRawText();
            }
            else if (content is string plainString)
            {
                contentString = plainString;
            }
            else
            {
                contentString = JsonConvert.SerializeObject(content);
            }

            var existingRecord = await context.Records.FirstOrDefaultAsync(r => r.Version == version);
            if (existingRecord != null)
            {
                existingRecord.Content = contentString;
            }
            else
            {
                var newRecord = new Record
                {
                    Version = version,
                    Content = contentString
                };
                context.Records.Add(newRecord);
            }

            await context.SaveChangesAsync();
            return Ok(new { message = "Record saved successfully." });
        }


        [HttpGet("load")]
        public async Task<IActionResult> Load([FromQuery] string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                return BadRequest("Version is required.");
            }

            var record = await context.Records.FirstOrDefaultAsync(r => r.Version == version);
            if (record == null)
            {
                return NotFound("Record not found.");
            }

            try
            {
                var jsonContent = System.Text.Json.JsonDocument.Parse(record.Content);
                return Ok(jsonContent);
            }
            catch (Exception)
            {
                return Ok(record.Content);
            }
        }

        [HttpGet("latest-version")]
        public async Task<IActionResult> LatestVersion()
        {
            var latestRecord = await context.Records.OrderByDescending(r => r.Id).FirstOrDefaultAsync();
            var newVersion = latestRecord == null ? 0 : int.Parse(latestRecord.Version);
            return Ok(newVersion);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var records = await context.Records
                .Select(r => new
                {
                    r.Id,
                    r.Version,
                    r.Content,
                })
                .ToListAsync();

            return Ok(records);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] string content)
        {
            var record = await context.Records.FindAsync(id);
            if (record == null)
            {
                return NotFound("Record not found.");
            }

            record.Content = content;

            await context.SaveChangesAsync();
            return Ok(new { message = "Record updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var record = await context.Records.FindAsync(id);
            if (record == null)
            {
                return NotFound("Record not found.");
            }
            
            context.Records.Remove(record);
            await context.SaveChangesAsync();

            return Ok(new { message = "Record deleted successfully." });
        }
    }
}