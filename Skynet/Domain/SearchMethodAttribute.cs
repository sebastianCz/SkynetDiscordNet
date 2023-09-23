using Skynet.Domain.Enum;

[AttributeUsage(AttributeTargets.All)]
public class SearchMethodAttribute : Attribute
{
    // Private fields.
    public SearchType SearchType { get; set; }
    public SearchMethodAttribute(SearchType searchType)
    {
        this.SearchType = searchType;
    }
       
}