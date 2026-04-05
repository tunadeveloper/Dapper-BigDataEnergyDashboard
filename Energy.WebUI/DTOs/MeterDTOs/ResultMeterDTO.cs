namespace Energy.WebUI.DTOs.MeterDTOs
{
    public class ResultMeterDTO
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public int RegionId { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public bool IsActive { get; set; }
    }
}
