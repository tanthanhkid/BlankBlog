using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankBlog.Models
{
    public class PostViewModel
    {
        public string SLUG { get; set; }
        public string CONTENT { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string CREATED_USER { get; set; }
        public string IS_ACTIVE { get; set; }
        public string META_KEYWORD { get; set; }
        public string META_DESC { get; set; }
        public string TITLE { get; set; }
        public Nullable<decimal> VIEW_COUNT { get; set; }
        public string IMAGE_COVER { get; set; }
        public int ID { get; set; }
        public string MAIN_TAG { get; set; }
    }
}