using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Filters;
using AlloyTemplates.Business;
using AlloyTemplates.Models.Blocks;
using AlloyTemplates.Models.ViewModels;
using EPiServer.Web.Mvc;
using EPiServer;
using Microsoft.AspNetCore.Mvc;
using EPiServer.Cms.AspNetCore.Mvc;

namespace AlloyTemplates.Controllers
{
    public class PageListBlockViewComponent : BlockComponent<PageListBlock>
    {
        private ContentLocator contentLocator;
        private IContentLoader contentLoader;
        public PageListBlockViewComponent(ContentLocator contentLocator, IContentLoader contentLoader)
        {
            this.contentLocator = contentLocator;
            this.contentLoader = contentLoader;
        }

        public override IViewComponentResult Invoke(PageListBlock currentBlock)
        {
            var pages = FindPages(currentBlock);

            pages = Sort(pages, currentBlock.SortOrder);
            
            if(currentBlock.Count > 0)
            {
                pages = pages.Take(currentBlock.Count);
            }

            var model = new PageListModel(currentBlock)
            {
                Pages = pages.Cast<PageData>()
            };

            ViewData.GetEditHints<PageListModel, PageListBlock>()
                .AddConnection(x => x.Heading, x => x.Heading);

            return View(model);
        }

        private IEnumerable<IContent> FindPages(PageListBlock currentBlock)
        {
            IEnumerable<PageData> pages;
            var listRoot = currentBlock.Root;
            if (currentBlock.Recursive)
            {
                if (currentBlock.PageTypeFilter != null)
                {
                    pages = contentLocator.FindPagesByPageType(listRoot, true, currentBlock.PageTypeFilter.ID);
                }
                else
                {
                    pages = contentLocator.GetAll<PageData>(listRoot);
                }
            }
            else
            {
                if (currentBlock.PageTypeFilter != null)
                {
                    pages = contentLoader.GetChildren<PageData>(listRoot)
                        .Where(p => p.ContentTypeID == currentBlock.PageTypeFilter.ID);
                }
                else
                {
                    pages = contentLoader.GetChildren<PageData>(listRoot);
                }
            }

            if (currentBlock.CategoryFilter != null && currentBlock.CategoryFilter.Any())
            {
                pages = pages.Where(x => x.Category.Intersect(currentBlock.CategoryFilter).Any());
            }
            return pages;
        }

        private IEnumerable<IContent> Sort(IEnumerable<IContent> pages, FilterSortOrder sortOrder)
        {
            // ToDo: review what shoud we do impolements only in Alloy or remove obsolete?
#pragma warning disable CS0618 // Type or member is obsolete
            var sortFilter = new FilterSort(sortOrder);
            sortFilter.Sort(new PageDataCollection(pages.ToList()));
#pragma warning restore CS0618 // Type or member is obsolete
            return pages;
        }
    }
}
