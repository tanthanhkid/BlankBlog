using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankBlog.Models
{
    public class RightColumnViewModel
    {
        public List<string> TagCounts { get; set; }
        public List<PostViewModel> PopularPosts { get; set; }
    }
}