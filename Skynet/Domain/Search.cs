namespace Skynet.Domain
{
    public class Search
    {
        public bool Found { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Result { get; set; }
        public ISearchResult? ResultObject { get; set; }

    }
}
