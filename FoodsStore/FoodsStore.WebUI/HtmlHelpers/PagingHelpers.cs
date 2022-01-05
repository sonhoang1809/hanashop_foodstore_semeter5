using FoodsStore.WebUI.Models;
using System;
using System.Text;
using System.Web.Mvc;

namespace FoodsStore.WebUI.HtmlHelpers {
    public static class PagingHelpers {
        
        public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo pagingInfo, string selectedPageCssClass, string defaultPageCssClass, string cssPageLinks, Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 1; i <= pagingInfo.TotalPages; i++)
            {
                TagBuilder tagLi = new TagBuilder("li");
                string a = "<a class=" + cssPageLinks + " href=" + pageUrl(i) + ">" + i + "</a>";                          
                if (i == pagingInfo.CurrentPage)
                {
                    tagLi.AddCssClass(selectedPageCssClass);
                }
                tagLi.AddCssClass(defaultPageCssClass);
                tagLi.InnerHtml = a;
                result.Append(tagLi.ToString());
            }
            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString PageLinksJson(this HtmlHelper html, PagingInfo pagingInfo, string selectedPageCssClass, string defaultPageCssClass, string cssPageLinks)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 1; i <= pagingInfo.TotalPages; i++)
            {
                TagBuilder tagLi = new TagBuilder("li");
                string a = "<a class=" + cssPageLinks + " href='#0' onclick='scrollPage(); searchProduct(" + i + ")';>" + i + "</a>";
                if (i == pagingInfo.CurrentPage)
                {
                    tagLi.AddCssClass(selectedPageCssClass);
                }
                tagLi.AddCssClass(defaultPageCssClass);
                tagLi.InnerHtml = a;
                result.Append(tagLi.ToString());
            }
            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString PageLinks2(this HtmlHelper html, PagingInfo pagingInfo, string selectedPageCssClass, string defaultPageCssClass, Func<int, string> pageUrl) {
            StringBuilder result = new StringBuilder();
            for (int i = 1; i <= pagingInfo.TotalPages; i++) {
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(i));
                tag.SetInnerText(i + "");
                if (i == pagingInfo.CurrentPage) {
                    tag.AddCssClass(selectedPageCssClass);
                }
                tag.AddCssClass(defaultPageCssClass);
                result.Append(tag.ToString());
            }
            return MvcHtmlString.Create(result.ToString());
        }
    }
}