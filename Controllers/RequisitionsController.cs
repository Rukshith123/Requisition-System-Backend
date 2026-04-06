using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequisitionManagement.API.Data;
using RequisitionManagement.API.DTOs;
using RequisitionManagement.API.Services;

namespace RequisitionManagement.API.Controllers
{
    [ApiController]
    [Route("api/requisitions")]
    public class RequisitionsController : ControllerBase
    {
        private readonly RequisitionService _service;
        private readonly AppDbContext _context;

        public RequisitionsController(RequisitionService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRequisitionDto dto, [FromQuery] int createdBy)
        {
            try
            {
                var result = await _service.CreateRequisitionAsync(dto, createdBy);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMy([FromQuery] string createdBy)
        {
            if (string.IsNullOrEmpty(createdBy))
                return BadRequest(new { error = "createdBy username is required." });

            var result = await _service.GetMyRequisitionsAsync(createdBy);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _context.Requisitions
                .Include(r => r.Creator)
                .Where(r => r.Id == id)
                .Select(r => new {
                    r.Id,
                    r.Title,
                    r.Department,
                    r.Skillset,
                    r.ExperienceLevel,
                    r.NumberOfPositions,
                    r.Location,
                    r.HireByDate,
                    r.CustomerName,
                    r.Comments,
                    r.JdContent,
                    r.Status,
                    r.CreatedAt,
                    r.UpdatedAt,
                    CreatedBy = r.Creator.Username
                })
                .FirstOrDefaultAsync();

            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var result = await _service.CancelRequisitionAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpGet("cancelled")]
        public async Task<IActionResult> GetCancelled()
        {
            var result = await _service.GetCancelledRequisitionsAsync();
            return Ok(result);
        }

        [HttpDelete("{id}/permanent")]
        public async Task<IActionResult> DeletePermanent(int id)
        {
            try
            {
                await _service.DeleteRequisitionAsync(id);
                return Ok(new { message = "Permanently deleted." });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}