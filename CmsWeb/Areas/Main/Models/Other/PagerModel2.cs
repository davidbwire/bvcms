/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UtilityExtensions;
using System.Text;
using CmsData;

namespace CmsWeb.Models
{
	public class PagerModel2
	{
		public PagerModel2(CountDelegate count) : this()
		{
			GetCount = count;
		}

		public PagerModel2()
		{
			ShowPageSize = true;
		}

		public string URL { get; set; }
		public string Sort { get; set; }
		public string Direction { get; set; }

		public string SortExpression
		{
			get
			{
				if (Direction == "asc")
					return Sort;
				return Sort + " " + Direction;
			}
		}

		public delegate int CountDelegate();
		public CountDelegate GetCount;
		private int? _count;

		private int count
		{
			get
			{
				if (!_count.HasValue)
				{
					_count = GetCount();
					if (StartRow >= _count)
						_Page = null;
				}
				return _count.Value;
			}
		}

		public void setCountDelegate(CountDelegate count)
		{
			GetCount = new CountDelegate(count);
		}

		public bool ShowPageSize { get; set; }
		public int? pagesize;
		private readonly int[] pagesizes = { 10, 25, 50, 100, 200 };

		public int PageSize
		{
			get
			{
				if (pagesize.HasValue)
					return pagesize.Value;
				return DbUtil.Db.UserPreference("PageSize", "10").ToInt();
			}
			set
			{
                if(pagesizes.Contains(value))
    				DbUtil.Db.SetUserPreference("PageSize", value);
				pagesize = value;
			}
		}
		private int? _Page;
		public int? Page
		{
			get { return _Page ?? 1; }
			set { _Page = value; }
		}
		public int LastPage()
		{
			return (int)Math.Ceiling(count / (double)PageSize);
		}
		public int StartRow
		{
			get { return (Page.Value - 1) * PageSize; }
		}
		public IEnumerable<SelectListItem> PageSizeList()
		{
			return pagesizes.Select(i => new SelectListItem { Text = i.ToString(), Selected = PageSize == i });
		}
		public IEnumerable<int> PageList()
		{
			for (var i = 1; i <= LastPage(); i++)
			{
				if (i > 1 && i < Page - 2)
				{
					i = Page.Value - 3;
					yield return 0;
				}
				else if (i < LastPage() && i > Page + 2)
				{
					i = LastPage() - 1;
					yield return 0;
				}
				else
					yield return i;
			}
		}

        internal void Set(string url, int? page, int? size, string sort, string dir)
        {
            URL = url;
            if(page.HasValue)
                Page = page.Value;
            if(size.HasValue)
                PageSize = size.Value;
            if (sort.HasValue())
                Sort = sort;
            if (dir.HasValue())
                Direction = dir;
        }
        public HtmlString SortLink(string label)
        {
            var active = "";
            var asc = " asc";
            if (label == Sort)
            {
                active = " active";
                if(Direction == "asc")
                    asc = "";
            }
            return new HtmlString("<a href='{0}/{1}/{2}/{3}/{4}' class='ajax{5}{6}'>{3}</a>"
                .Fmt(URL, Page, PageSize, label, Direction == "asc" ? "desc" : "asc", active, asc));
        }
        public HtmlString PageLink(string label, int? page, int? size = null)
        {
            return new HtmlString("<a href='{0}/{1}/{2}/{3}/{4}' class='ajax'>{5}</a>"
                .Fmt(URL, page ?? 1, size ?? PageSize, Sort ?? "na", Direction == "asc" ? "desc" : "asc", label));
        }
        public HtmlString PageSizeItem(string label, int? page, int? size = null, bool? disable = null)
        {
            var disabled = "";
            if (disable == true)
                disabled = " class='disabled'";
            return new HtmlString("<li{6}><a href='{0}/{1}/{2}/{3}/{4}' class='ajax'>{5}</a></li>"
                .Fmt(URL, page ?? 1, size ?? PageSize, Sort ?? "na", Direction == "asc" ? "desc" : "asc", label, disabled));
        }
        public string ShowCount()
        {
            var cnt = 0;
            var n = GetCount();
            if (n > PageSize)
                cnt = n - StartRow;
            if (cnt > PageSize)
                cnt = PageSize;
            return "Showing {0} of {1} records".Fmt(cnt, n.ToString("N0"));
        }
    }
}
