using BlankBlog.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BlankBlog.Helper
{
    [System.ComponentModel.DataObject]
    public class StaticCache
    {
        public static void LoadStaticCache()
        {
            // Get all tag
            BlogEntities db = new BlogEntities();
            List<TAG> tags = new List<TAG>();
            HttpContext.Current.Application["tags"] = db.TAGs.ToList();
        }
        [DataObjectMethodAttribute(DataObjectMethodType.Select, true)]
        public static List<TAG> GetTags()
        {
            return HttpContext.Current.Application["tags"] as List<TAG>;
        }
    }
}