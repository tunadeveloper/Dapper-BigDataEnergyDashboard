namespace Energy.WebAPI.DTOs.RegionDTOs
{
    public class RegionOverviewDto
    {
        public int TotalRegionCount { get; set; }
        public string TopConsumptionRegionName { get; set; }
        public decimal TopConsumptionValue { get; set; }
        public string TopMeterRegionName { get; set; }
        public int TopMeterRegionCount { get; set; }
        public decimal AverageVoltage { get; set; }
        public List<RegionOverviewItemDto> Regions { get; set; } = new();
    }

    public class RegionOverviewItemDto
    {
        public string RegionName { get; set; }
        public decimal TotalConsumption { get; set; }
        public int MeterCount { get; set; }
        public decimal AverageVoltage { get; set; }
        public decimal SharePercent { get; set; }
    }
}
