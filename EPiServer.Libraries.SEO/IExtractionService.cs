using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EPiServer.Libraries.SEO
{
    public interface IExtractionService
    {
        ReadOnlyCollection<string> GetKeywords(string text);
    }

}
