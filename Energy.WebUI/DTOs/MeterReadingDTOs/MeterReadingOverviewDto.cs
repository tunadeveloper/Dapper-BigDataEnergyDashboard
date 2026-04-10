namespace Energy.WebUI.DTOs.MeterReadingDTOs
{
    public class MeterReadingOverviewDto
    {
        public int TodayReadingCount { get; set; }
        public decimal TotalConsumption { get; set; }
        public decimal AverageVoltage { get; set; }
        public int AnomalyCount { get; set; }
        public List<ResultMeterReadingWithRegionDTO> LatestReadings { get; set; } = new();
    }
}
