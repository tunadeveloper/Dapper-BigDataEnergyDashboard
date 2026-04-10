namespace Energy.WebAPI.DTOs.MeterDTOs
{
    public class MeterOverviewDto
    {
        public int TotalMeterCount { get; set; }
        public int ActiveMeterCount { get; set; }
        public int PassiveMeterCount { get; set; }
        public int NewThisMonthCount { get; set; }
        public decimal ActiveRate { get; set; }
        public List<MeterDistributionItemDto> RegionDistribution { get; set; } = new();
        public List<MeterDistributionItemDto> TariffDistribution { get; set; } = new();
    }

    public class MeterDistributionItemDto
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
