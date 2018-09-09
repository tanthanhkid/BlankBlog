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
            DateTime createdDate = new DateTime(year, month, date);

            POST content = new POST();
            BlogEntities db = new BlogEntities();
            content = db.POSTs.FirstOrDefault(c => c.SLUG.Equals(slug) && DbFunctions.TruncateTime(c.CREATED_DATE) == createdDate);

            //CAN'T FIND POST, REDIRECT TO 404 PAGE
            if (content == null)
            {
                return RedirectToAction("NotFound");
            }

            //GET POST'S TAG
            List<TAG> tags=db.TAGs.SqlQuery(string.Format( "select tb2.* from POST tb1, TAG tb2, POST_TAG tb3 where tb1.ID=tb3.POST_ID and tb3.TAG_ID=tb2.ID and tb1.ID={0}",content.ID)).ToList<TAG>();

            DetailBlogViewModel viewModel = new DetailBlogViewModel();
            viewModel.Post = content;
            viewModel.Tags = tags;
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


    }
}