namespace Energy.WebUI.DTOs.DashboardDTOs
{
    public class EnergyDashboardDto
    {
        public decimal TotalConsumption { get; set; }
        public int ReadingCount { get; set; }
        public int ActiveMeterCount { get; set; }
        public decimal AverageVoltage { get; set; }
        public int RegionCount { get; set; }
    }
}
