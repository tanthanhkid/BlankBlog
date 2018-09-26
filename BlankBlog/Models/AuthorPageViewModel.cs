using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankBlog.Models
{
    public class AuthorPageViewModel
    {
        public IPagedList<PostViewModel> ListPost { get; set; }
        public PAGE_USER User { get; set; }
    }
}