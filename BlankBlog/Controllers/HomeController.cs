using BlankBlog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlankBlog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //BlogEntities db = new BlogEntities();
            //for (int i = 7; i < 1000; i++)
            //{
            //    POST post = new POST()
            //    {
            //        SLUG= "bai-viet-test-so-"+i,
            //        CONTENT=" sad asdsad sadsad sad as as d",
            //        CREATED_USER="admin",
            //        META_KEYWORD="sda sad asd",
            //        META_DESC="asd adsa dsad",
            //        TITLE= "bai viet so "+i,
            //        VIEW_COUNT=0,
            //        IMAGE_COVER= "Template/img/header-1.jpg",
            //        MAIN_TAG= "lifestyle"
            //    };
            //    db.POSTs.Add(post);
            //}
            //db.SaveChanges();
            return View();
        }

        /// <summary>
        /// GET POST CONTENT BY SLUG
        /// </summary>
        /// <param name="year">CREATED YEAR</param>
        /// <param name="month">CREATED MONTH</param>
        /// <param name="date">CREATED DATE</param>
        /// <param name="slug">SLUG VALUE</param>
        /// <returns></returns>
        public ActionResult DetailBlog(int year, int month, int date, string slug)
        {
            DetailBlogViewModel viewModel = new DetailBlogViewModel();
            BlogEntities db = new BlogEntities();
            DateTime createdDate = new DateTime(year, month, date);

            POST content = new POST();
            
            content = db.POSTs.FirstOrDefault(c => c.SLUG.Equals(slug) && DbFunctions.TruncateTime(c.CREATED_DATE) == createdDate && c.IS_ACTIVE.Equals("1"));

            //CAN'T FIND POST, REDIRECT TO 404 PAGE
            if (content == null)
            {
                return RedirectToAction("NotFound");
            }

            viewModel.Post = content;

            //GET POST'S TAG
            viewModel.Tags = db.TAGs.SqlQuery(string.Format( "select tb2.* from POST tb1, TAG tb2, POST_TAG tb3 where tb1.ID=tb3.POST_ID and tb3.TAG_ID=tb2.ID and tb1.ID={0}",content.ID)).ToList<TAG>();

            //GET POST'S AUTHOR
            viewModel.Author = db.PAGE_USER.FirstOrDefault(c => c.USERNAME.Equals(content.CREATED_USER));

            
            //SETTING VIEWBAG 
            ViewBag.Title = content.TITLE; 
            return View(viewModel);
        }

        /// <summary>
        /// RETURN 404 PAGE
        /// </summary>
        /// <returns></returns>
        [Route(Name ="404")]
        public ActionResult NotFound( )
        {
             
            return View();
        }

        [ChildActionOnly]
        public ActionResult RightColumn()
        {
            return PartialView();
        }


    }
}