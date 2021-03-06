using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class AskSize : Ask
	{
		public decimal? Fee { get; set; }
		public string Label { get; set; }
		public bool AllowLastYear { get; set; }
		public List<Size> list { get; set; }
		public AskSize() : base("AskSize")
		{
		    list = new List<Size>();
		}
		public static AskSize Parse(Parser parser)
		{
			var r = new AskSize();
			parser.lineno++;
			r.Label = parser.GetLabel("Size");
			if (parser.curr.kw == Parser.RegKeywords.Fee)
				r.Fee = parser.GetDecimal();
			if (parser.curr.kw == Parser.RegKeywords.AllowLastYear)
				r.AllowLastYear = parser.GetBool();
			r.list = new List<Size>();
			if (parser.curr.indent == 0)
				return r;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				var m = Size.Parse(parser, startindent);
				r.list.Add(m);
			}
			var q = r.list.GroupBy(mi => mi.SmallGroup).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
			if (q.Any())
				throw parser.GetException("Duplicate SmallGroup in Sizes: " + string.Join(",", q));
			return r;
		}
		public override void Output(StringBuilder sb)
		{
			Settings.AddValueNoCk(0, sb, "AskSize", "");
			Settings.AddValueCk(1, sb, "Label", Label ?? "Size");
			Settings.AddValueCk(1, sb, "Fee", Fee);
			Settings.AddValueCk(1, sb, "AllowLastYear", AllowLastYear);
			foreach (var q in list)
				q.Output(sb);
			sb.AppendLine();
		}
        public override List<string> SmallGroups()
        {
            var q = (from i in list
                     select i.SmallGroup).ToList();
            return q;
        }

		public class Size
		{
			public string Name { get; set; }
			public string Description { get; set; }
			public string SmallGroup { get; set; }

			public void Output(StringBuilder sb)
			{
				Settings.AddValueCk(1, sb, Description);
				Settings.AddValueCk(2, sb, "SmallGroup", SmallGroup ?? Description);
			}

			public static Size Parse(Parser parser, int startindent)
			{
				var i = new Size();
				if (parser.curr.kw != Parser.RegKeywords.None)
					throw parser.GetException("unexpected line in Size");
				i.Description = parser.GetLine();
				i.SmallGroup = i.Description;
				if (parser.curr.indent <= startindent)
					return i;
				var ind = parser.curr.indent;
				while (parser.curr.indent == ind)
				{
					if (parser.curr.kw != Parser.RegKeywords.SmallGroup)
						throw parser.GetException("unexpected line in Size");
					i.SmallGroup = parser.GetString(i.Description);
				}
				return i;
			}
		}
		public static List<Size> ParseShirtSizes(Parser parser)
		{
			parser.lineno++;
			var list = new List<Size>();
			if (parser.curr.indent == 0)
				return list;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				var shirtsize = new Size();
				if (parser.curr.kw != Parser.RegKeywords.None)
					throw parser.GetException("unexpected line");
				shirtsize.Description = parser.GetLine();
				shirtsize.SmallGroup = shirtsize.Description;
				if (parser.curr.indent > startindent)
				{
					if (parser.curr.kw != Parser.RegKeywords.SmallGroup)
						throw parser.GetException("expected SmallGroup keyword");
					shirtsize.SmallGroup = parser.GetString(shirtsize.SmallGroup);
				}
				list.Add(shirtsize);
			}
			return list;
		}
	}
}