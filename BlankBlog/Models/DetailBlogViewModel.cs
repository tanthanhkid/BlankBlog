using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankBlog.Models
{
    public class DetailBlogViewModel
    {
        public POST Post { get; set; }
        public List<TAG> Tags { get; set; }
        public PAGE_USER Author { get; set; }
        public POST PrevPost { get; set; }
        public POST NextPost { get; set; }
    }
}