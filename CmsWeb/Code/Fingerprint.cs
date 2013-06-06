﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Xml.Linq;
using UtilityExtensions;

public class Fingerprint
{
    public static string Tag(string path)
    {
        if (HttpRuntime.Cache[path] == null)
        {
            string absolute = HostingEnvironment.MapPath("~" + path);
            var ext = Path.GetExtension(absolute);
            var f = Path.GetFileNameWithoutExtension(absolute);
            var d = path.Remove(path.LastIndexOf('/'));
            DateTime dt = File.GetLastWriteTime(absolute);
#if DEBUG
            const string min = "";
#else
            const string min = ".min";
#endif
            string tag = "?v=" + dt.Ticks;
            string result = "{0}/{1}{2}{3}{4}".Fmt(d, f, min, ext, tag);
            HttpRuntime.Cache.Insert(path, result, new CacheDependency(absolute));
        }

        return HttpRuntime.Cache[path] as string;
    }
    public static HtmlString Script(string path)
    {
        if (HttpRuntime.Cache[path] == null)
        {
            string absolute = HostingEnvironment.MapPath("~" + path);
            var result = new StringBuilder();
#if DEBUG
            const string min = "";
            var bundle = absolute + ".bundle";
            if (File.Exists(bundle))
            {
                var xd = XDocument.Load(bundle);
                foreach (var i in xd.Descendants("file"))
                {
                    string a = HostingEnvironment.MapPath("~" + i.Value);
                    var fd = File.GetLastWriteTime(a);
                    string t = i.Value + "?v=" + fd.Ticks;
                    result.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>\n", t);
                }
            }
            else
            {
                Debug.Assert(absolute != null, "absolute != null");
                var fd = File.GetLastWriteTime(absolute);
                string t = path + "?v=" + fd.Ticks;
                result.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>\n", t);
            }
#else
            const string min = ".min";
            var ext = Path.GetExtension(absolute);
            var f = Path.GetFileNameWithoutExtension(absolute);
            var d = path.Remove(path.LastIndexOf('/'));
            DateTime dt = File.GetLastWriteTime(absolute);
            string tag = "?v=" + dt.Ticks;
            result.AppendFormat("<script type=\"text/javascript\" src=\"{0}/{1}{2}{3}{4}\"></script>\n", d, f, min, ext, tag);
#endif
            HttpRuntime.Cache.Insert(path, result.ToString(), new CacheDependency(absolute));
        }
        return new HtmlString(HttpRuntime.Cache[path] as string);
    }
    public static HtmlString Css(string path)
    {
        if (HttpRuntime.Cache[path] == null)
        {
            string absolute = HostingEnvironment.MapPath("~" + path);
            var result = new StringBuilder();
#if DEBUG
            const string min = "";
            var bundle = absolute + ".bundle";
            if (File.Exists(bundle))
            {
                var xd = XDocument.Load(bundle);
                foreach (var i in xd.Descendants("file"))
                {
                    string a = HostingEnvironment.MapPath("~" + i.Value);
                    var fd = File.GetLastWriteTime(a);
                    string t = i.Value + "?v=" + fd.Ticks;
                    result.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" />\n", t);
                }
            }
            else
            {
                Debug.Assert(absolute != null, "absolute != null");
                var fd = File.GetLastWriteTime(absolute);
                string t = path + "?v=" + fd.Ticks;
                result.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" />\n", t);
            }
#else
            const string min = ".min";
            var ext = Path.GetExtension(absolute);
            var f = Path.GetFileNameWithoutExtension(absolute);
            var d = path.Remove(path.LastIndexOf('/'));
            DateTime dt = File.GetLastWriteTime(absolute);
            string tag = "?v=" + dt.Ticks;
            result.AppendFormat("<link href=\"{0}/{1}{2}{3}{4}\" rel=\"stylesheet\" />\n", d, f, min, ext, tag);
#endif
            HttpRuntime.Cache.Insert(path, result.ToString(), new CacheDependency(absolute));
        }
        return new HtmlString(HttpRuntime.Cache[path] as string);
    }
}