namespace EPiServer.Libraries.SEO.Models
{
    public class Keyword
    {
        public string text { get; set; }
        public float relevance { get; set; }
        public Sentiment sentiment { get; set; }
    }
}