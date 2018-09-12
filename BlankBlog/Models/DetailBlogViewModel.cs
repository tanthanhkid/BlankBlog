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
        public PostViewModel PrevPost { get; set; }
        public PostViewModel NextPost { get; set; }
        public List<PostViewModel> RelatedPosts { get; set; }
    }
}