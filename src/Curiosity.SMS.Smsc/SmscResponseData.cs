namespace Curiosity.SMS.Smsc
{
    internal class SmscResponseData
    {
        public string error { get; set; }
        public int? error_code { get; set; }
        
        public string id { get; set; }
        
        public int? cnt { get; set; }
        
        public decimal? cost { get; set; }
        public decimal? balance { get; set; }
    }
}
