namespace Energy.WebAPI.DTOs.MeterReadingDTOs
{
    public class CreateMeterReadingDTO
    {
        public int MeterId { get; set; }
        public decimal Consumption { get; set; }
        public int Voltage { get; set; }
        public DateTime ReadingDate { get; set; }
        public string TariffType { get; set; }
    }
}
