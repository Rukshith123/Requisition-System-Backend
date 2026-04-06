namespace RequisitionManagement.API.Models
{
    public class Approval
    {
        public int Id { get; set; }
        public int RequisitionId { get; set; }
        public int ApproverId { get; set; }
        public string ApprovalLevel { get; set; } = "";
        public string Status { get; set; } = "";
        public string? Comments { get; set; }
        public DateTime ActionDate { get; set; }

        // Navigation properties
        public User? Approver { get; set; }
        public Requisition? Requisition { get; set; }
    }
}