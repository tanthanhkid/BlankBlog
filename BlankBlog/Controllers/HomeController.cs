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
        public ActionResult DetailBlog(string slug)
        {
            DetailBlogViewModel viewModel = new DetailBlogViewModel();
            BlogEntities db = new BlogEntities();

            POST content = new POST();

            content = db.POSTs.FirstOrDefault(c => c.SLUG.Equals(slug) && c.IS_ACTIVE.Equals("1"));

            //CAN'T FIND POST, REDIRECT TO 404 PAGE
            if (content == null)
            {
                return RedirectToAction("NotFound");
            }

            viewModel.Post = content;

            //GET POST'S TAG
            viewModel.Tags = db.TAGs.SqlQuery(string.Format("SELECT TB2.* FROM POST TB1, TAG TB2, POST_TAG TB3 WHERE TB1.ID=TB3.POST_ID AND TB3.TAG_ID=TB2.ID AND TB1.ID={0}", content.ID)).ToList<TAG>();

            //GET POST'S AUTHOR
            viewModel.Author = db.PAGE_USER.FirstOrDefault(c => c.USERNAME.Equals(content.CREATED_USER));

            //GET PREV AND NEXT POSTs
            try
            {
                string sql = string.Format(@"SELECT CONVERT(nvarchar,ISNULL( K.PREV,0))+'#'+CONVERT(nvarchar,ISNULL( K.NEXT,0)) 
                                         FROM(
	                                          SELECT LAG(P.ID) OVER(ORDER BY P.ID) PREV,
			                                         P.ID,
			                                         LEAD(P.ID) OVER(ORDER BY P.ID) NEXT
	                                          FROM	POST P
	                                          WHERE	P.ID<=({0}*2) AND IS_ACTIVE='1' ) K
                                         WHERE ID={1}", content.ID <= 1 ? 2 : content.ID, content.ID);
                string[] nearById = db.Database.SqlQuery<string>(sql).ToList().FirstOrDefault().Split('#');
                //GET PREV_ID#NEXT_ID
                int prevId = Convert.ToInt32(nearById[0]);
                int nextId = Convert.ToInt32(nearById[1]);

                viewModel.PrevPost = db.POSTs.Where(c => c.ID.Equals(prevId)).Select(c => new PostViewModel { TITLE = c.TITLE, IMAGE_COVER = c.IMAGE_COVER, SLUG = c.SLUG }).FirstOrDefault();
                viewModel.NextPost = db.POSTs.Where(c => c.ID.Equals(nextId)).Select(c => new PostViewModel { TITLE = c.TITLE, IMAGE_COVER = c.IMAGE_COVER, SLUG = c.SLUG }).FirstOrDefault();

                if (viewModel.PrevPost == null) { viewModel.PrevPost = new PostViewModel() { SLUG = "/", TITLE = "Trang chủ" }; }
                if (viewModel.NextPost == null) { viewModel.NextPost = new PostViewModel() { SLUG = "/", TITLE = "Trang chủ" }; }

            }
            catch (Exception ex)
            {
                viewModel.PrevPost = new PostViewModel() { SLUG = "/", TITLE = "Trang chủ" };
                viewModel.NextPost = new PostViewModel() { SLUG = "/", TITLE = "Trang chủ" };
            }

            //GET 3 RELATED POST
            if (viewModel.Tags != null)
            {
                viewModel.RelatedPosts = new List<PostViewModel>();
                foreach (TAG tag in viewModel.Tags)
                {
                    PostViewModel pV = db.POSTs.SqlQuery(string.Format("SELECT TB1.* FROM POST TB1, TAG TB2, POST_TAG TB3 WHERE TB1.ID=TB3.POST_ID AND TB3.TAG_ID=TB2.ID AND TB2.ID={0} AND TB1.ID <> {1}", tag.ID, content.ID)).Select(c => new PostViewModel { TITLE = c.TITLE, IMAGE_COVER = c.IMAGE_COVER, SLUG = c.SLUG, CREATED_DATE = c.CREATED_DATE, CREATED_USER = c.CREATED_USER }).FirstOrDefault();
                    if (pV != null)
                    {
                        pV.MAIN_TAG = tag.NAME;
                        viewModel.RelatedPosts.Add(pV);
                        if (viewModel.RelatedPosts.Count >= 3) { break; }
                    }
                }
            }


            //SETTING VIEWBAG 
            ViewBag.Title = content.TITLE;
            return View(viewModel);
        }

        /// <summary>
        /// RETURN 404 PAGE
        /// </summary>
        /// <returns></returns>
        [Route(Name = "404")]
        public ActionResult NotFound()
        {

            return View();
        }

        [ChildActionOnly]
        public ActionResult RightColumn()
        {
            BlogEntities db = new BlogEntities();
            RightColumnViewModel viewModel = new RightColumnViewModel();

            //GET CATEGORIES COUNT
            string sql = "SELECT NAME+'$'+CONVERT(NVARCHAR, COUNT(*)) FROM TAG GROUP BY NAME";
            viewModel.TagCounts = db.Database.SqlQuery<string>(sql).ToList();

            //GET POPULAR POST
            viewModel.PopularPosts = db.POSTs.SqlQuery("SELECT TOP 4 * FROM POST ORDER BY VIEW_COUNT DESC").Select(c => new PostViewModel { TITLE = c.TITLE, IMAGE_COVER = c.IMAGE_COVER, SLUG = c.SLUG, CREATED_DATE = c.CREATED_DATE, CREATED_USER = c.CREATED_USER }).ToList();

            return PartialView(viewModel);
        }


    }
}