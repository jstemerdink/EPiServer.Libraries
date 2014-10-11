namespace EPiServer.Libraries.SEO.Models
{
    public class AlchemyResponse
    {
        public string status { get; set; }
        public string url { get; set; }
        public string language { get; set; }
        public string text { get; set; }
        public Keyword[] keywords { get; set; }
    }
}