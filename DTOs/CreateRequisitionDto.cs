namespace RequisitionManagement.API.DTOs
{
    public class CreateRequisitionDto
    {
        public string Title { get; set; } = "";
        public string Department { get; set; } = "";
        public string Skillset { get; set; } = "";
        public string ExperienceLevel { get; set; } = "";
        public int NumberOfPositions { get; set; }
        public string? Location { get; set; }
        public DateTime? HireByDate { get; set; }
        public string? CustomerName { get; set; }
        public string? Comments { get; set; }
        public string? JdContent { get; set; }
    }
}