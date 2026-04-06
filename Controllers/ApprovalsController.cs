using Microsoft.AspNetCore.Mvc;
using RequisitionManagement.API.Data;
using RequisitionManagement.API.DTOs;
using RequisitionManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace RequisitionManagement.API.Controllers
{
    [ApiController]
    [Route("api/approvals")]
    public class ApprovalsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ApprovalsController(AppDbContext context)
        {
            _context = context;
        }

        // BU Manager sees Pending, BA Manager sees BUApproved
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending([FromQuery] string role)
        {
            string status = role == "BU_MANAGER" ? "Pending" : "BUApproved";

            var requisitions = await _context.Requisitions
                .Include(r => r.Creator)
                .Where(r => r.Status == status)
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
                    r.Status,
                    r.CreatedAt,
                    r.UpdatedAt,
                    CreatedBy = r.Creator.Username
                })
                .ToListAsync();

            return Ok(requisitions);
        }

        // Approve
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id, [FromBody] ApprovalDto dto)
        {
            var requisition = await _context.Requisitions
                .Include(r => r.Creator)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (requisition == null) return NotFound(new { message = "Requisition not found" });

            var approver = await _context.Users.FindAsync(dto.ApproverId);
            if (approver == null) return NotFound(new { message = "Approver not found" });

            // Allow approving from OnHold too
            if (approver.Role == "BU_MANAGER" &&
                (requisition.Status == "Pending" || requisition.Status == "OnHold"))
                requisition.Status = "BUApproved";
            else if (approver.Role == "BA_MANAGER" &&
                (requisition.Status == "BUApproved" || requisition.Status == "OnHold"))
                requisition.Status = "BAApproved";
            else
                return BadRequest(new { message = $"Invalid approval. Role: {approver.Role}, Status: {requisition.Status}" });

            requisition.UpdatedAt = DateTime.Now;
            _context.Requisitions.Update(requisition);

            var approval = new Approval
            {
                RequisitionId = id,
                ApproverId = dto.ApproverId,
                ApprovalLevel = approver.Role == "BU_MANAGER" ? "BU" : "BA",
                Status = "Approved",
                Comments = dto.Comments,
                ActionDate = DateTime.Now
            };

            _context.Approvals.Add(approval);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Approved successfully", status = requisition.Status });
        }

        // Reject
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] ApprovalDto dto)
        {
            var requisition = await _context.Requisitions.FindAsync(id);
            if (requisition == null) return NotFound(new { message = "Requisition not found" });

            var approver = await _context.Users.FindAsync(dto.ApproverId);
            if (approver == null) return NotFound(new { message = "Approver not found" });

            if (requisition.Status == "Cancelled" || requisition.Status == "Closed")
                return BadRequest(new { message = "Cannot reject at this stage" });

            requisition.Status = "Rejected";
            requisition.UpdatedAt = DateTime.Now;
            _context.Requisitions.Update(requisition);

            var approval = new Approval
            {
                RequisitionId = id,
                ApproverId = dto.ApproverId,
                ApprovalLevel = approver.Role == "BU_MANAGER" ? "BU" : "BA",
                Status = "Rejected",
                Comments = dto.Comments,
                ActionDate = DateTime.Now
            };

            _context.Approvals.Add(approval);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Rejected successfully", status = requisition.Status });
        }

        // Hold
        [HttpPut("{id}/hold")]
        public async Task<IActionResult> Hold(int id, [FromBody] ApprovalDto dto)
        {
            var requisition = await _context.Requisitions.FindAsync(id);
            if (requisition == null) return NotFound(new { message = "Requisition not found" });

            var approver = await _context.Users.FindAsync(dto.ApproverId);
            if (approver == null) return NotFound(new { message = "Approver not found" });

            if (requisition.Status == "Cancelled" || requisition.Status == "Closed" || requisition.Status == "Rejected")
                return BadRequest(new { message = "Cannot hold at this stage" });

            requisition.Status = "OnHold";
            requisition.UpdatedAt = DateTime.Now;
            _context.Requisitions.Update(requisition);

            var approval = new Approval
            {
                RequisitionId = id,
                ApproverId = dto.ApproverId,
                ApprovalLevel = approver.Role == "BU_MANAGER" ? "BU" : "BA",
                Status = "OnHold",
                Comments = dto.Comments,
                ActionDate = DateTime.Now
            };

            _context.Approvals.Add(approval);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Put on hold", status = requisition.Status });
        }

        // Get approvals by approver — includes Requisition with Creator for CU manager name
        [HttpGet("my")]
        public async Task<IActionResult> GetMyApprovals([FromQuery] int approverId)
        {
            var approvals = await _context.Approvals
                .Include(a => a.Approver)
                .Include(a => a.Requisition)
                    .ThenInclude(r => r.Creator)
                .Where(a => a.ApproverId == approverId)
                .Select(a => new {
                    a.Id,
                    a.RequisitionId,
                    a.ApprovalLevel,
                    a.Status,
                    a.Comments,
                    a.ActionDate,
                    Approver = new
                    {
                        a.Approver.Id,
                        a.Approver.Username,
                        a.Approver.Role
                    },
                    Requisition = new
                    {
                        a.Requisition.Id,
                        a.Requisition.Title,
                        a.Requisition.Department,
                        a.Requisition.Skillset,
                        a.Requisition.ExperienceLevel,
                        a.Requisition.NumberOfPositions,
                        a.Requisition.Location,
                        a.Requisition.HireByDate,
                        a.Requisition.CustomerName,
                        a.Requisition.Comments,
                        a.Requisition.Status,
                        a.Requisition.CreatedAt,
                        a.Requisition.UpdatedAt,
                        CreatedBy = a.Requisition.Creator.Username
                    }
                })
                .ToListAsync();

            return Ok(approvals);
        }
    }
}