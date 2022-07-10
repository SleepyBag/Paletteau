using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Paletteau.Plugin.BrowserBookmark
{
    public class EdgeBookmarks
    {
        private List<Bookmark> bookmarks = new List<Bookmark>();

        public List<Bookmark> GetBookmarks()
        {
            LoadEdgeBookmarks();
            return bookmarks;
        }

        private IEnumerable<JObject> GetNestedChildren(JObject jo)
        {
            List<JObject> nested = new List<JObject>();
            JArray children = (JArray) jo["children"];
            foreach(JObject c in children)
            {
                var type = c["type"].ToString();
                if (type == "folder")
                {
                    var nc = GetNestedChildren(c);
                    nested.AddRange(nc);
                } else if (type == "url")
                {
                    nested.Add(c);
                }
            }
            return nested;
        }

        private static string getBookmarkName(JObject obj)
        {
            JContainer cur = obj;
            string name = (string)cur["name"];
            while (cur != null)
            {
                if (cur is JObject)
                    name = (string)cur["name"] + "/" + name;
                cur = cur.Parent;
            }
            return name;
        }

        private void ParseEdgeBookmarks(String path, string source)
        {
            if (!File.Exists(path)) return;
            string all = File.ReadAllText(path);
            JObject json = JObject.Parse(all);
            var items = (JObject) json["roots"]["bookmark_bar"];
            var flatterned = GetNestedChildren(items);
            var bs = from item in flatterned select new Bookmark()
                     {
                         Name = getBookmarkName(item),
                         Url = (string)item["url"],
                         Source = source
                     };
            var filtered = bs.Where(b =>
            {
                var c = !b.Url.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase) &&
                        !b.Url.StartsWith("vbscript:", StringComparison.OrdinalIgnoreCase);
                return c;
            });
            bookmarks.AddRange(filtered);
        }

        private void LoadEdgeBookmarks(string path, string name)
        {
            if (!Directory.Exists(path)) return;
            var paths = Directory.GetDirectories(path);

            foreach (var profile in paths)
            {
                
                if (File.Exists(Path.Combine(profile, "Bookmarks")))
                    ParseEdgeBookmarks(Path.Combine(profile, "Bookmarks"), name + (Path.GetFileName(profile) == "Default" ? "" : (" (" + Path.GetFileName(profile) + ")")));
            }
        }

        private void LoadEdgeBookmarks()
        {
            String platformPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            LoadEdgeBookmarks(Path.Combine(platformPath, @"Microsoft\Edge\User Data"), "Google Chrome");
        }

    }
}

