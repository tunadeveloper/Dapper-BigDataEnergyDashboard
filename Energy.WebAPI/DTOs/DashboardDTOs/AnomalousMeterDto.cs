namespace Energy.WebAPI.DTOs.DashboardDTOs
{
    public class AnomalousMeterDto
    {
        public int MeterId { get; set; }
        public string Message { get; set; }
        public decimal Consumption { get; set; }
        public DateTime DetectedAt { get; set; }
    }
}
