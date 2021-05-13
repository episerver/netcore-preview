using EPiServer.ServiceLocation;
using Mediachase.Commerce.Website.Search;
using Mediachase.Search;
using Mediachase.Search.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using AppContext = Mediachase.Commerce.Core.AppContext;
using StringCollection = System.Collections.Specialized.StringCollection;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Facades
{
    [ServiceConfiguration(typeof(SearchFacade), Lifecycle = ServiceInstanceScope.Singleton)]
    public class SearchFacade
    {
        public enum SearchProviderType
        {
            Lucene,
            Unknown
        }

        private SearchManager _searchManager;
        private SearchProviderType _searchProviderType;
        private bool _initialized;
        private readonly IOptions<SearchOptions> _searchOptions;
        private readonly IServiceProvider _serviceProvider;
        private readonly IndexBuilder _indexBuilder;

        public SearchFacade(IOptions<SearchOptions> searchOptions,
            IServiceProvider serviceProvider, 
            IndexBuilder indexBuilder)
        {
            _searchOptions = searchOptions;
            _serviceProvider = serviceProvider;
            _indexBuilder = indexBuilder;
        }

        public virtual ISearchResults Search(CatalogEntrySearchCriteria criteria)
        {
            Initialize();
            return _searchManager.Search(criteria);
        }

        public virtual SearchProviderType GetSearchProvider()
        {
            Initialize();
            return _searchProviderType;
        }

        public virtual SearchFilter[] SearchFilters => new SearchFilter[0];

        public virtual StringCollection GetOutlinesForNode(string code)
        {
            return SearchFilterHelper.GetOutlinesForNode(code);
        }

        private void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            _searchManager = new SearchManager(AppContext.Current.ApplicationName, _searchOptions, _serviceProvider, _indexBuilder);
            _searchProviderType = LoadSearchProvider();
            _initialized = true;
        }

        private SearchProviderType LoadSearchProvider()
        {
            var element = _searchOptions.Value.SearchProviders;
            if (element == null ||
                string.IsNullOrEmpty(_searchOptions.Value.DefaultSearchProvider) ||
                string.IsNullOrEmpty(_searchOptions.Value.SearchProviders.Single(x => x.Name.Equals(_searchOptions.Value.DefaultSearchProvider, StringComparison.OrdinalIgnoreCase)).Type))
            {
                return SearchProviderType.Unknown;
            }

            var providerType = Type.GetType(_searchOptions.Value.SearchProviders.Single(x => x.Name.Equals(_searchOptions.Value.DefaultSearchProvider, StringComparison.OrdinalIgnoreCase)).Type);
            var baseType = Type.GetType("Mediachase.Search.Providers.Lucene.LuceneSearchProvider, Mediachase.Search.LuceneSearchProvider");
            if (providerType == null || baseType == null)
            {
                return SearchProviderType.Unknown;
            }
            if (providerType == baseType || providerType.IsSubclassOf(baseType))
            {
                return SearchProviderType.Lucene;
            }

            return SearchProviderType.Unknown;
        }
    }
}