using BlankBlog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using BlankBlog.Helper;

namespace BlankBlog.Controllers
{
    [RoutePrefix("trang-chu")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        #region trang chu
        /// <summary>
        /// Most viewed post 
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult PopularPosts()
        {
            List<PostViewModel> viewModel = new List<PostViewModel>();
            BlogEntities db = new BlogEntities();

            viewModel = db.POSTs.SqlQuery("SELECT TOP 3 * FROM POST ORDER BY VIEW_COUNT DESC").Select(c => new PostViewModel { TITLE = c.TITLE, IMAGE_COVER = c.IMAGE_COVER, SLUG = c.SLUG, CREATED_DATE = c.CREATED_DATE, CREATED_USER = c.CREATED_USER, MAIN_TAG = c.MAIN_TAG }).ToList();

            return PartialView(viewModel);
        }

        /// <summary>
        /// Recent post
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult RecentPosts()
        {
            List<PostViewModel> viewModel = new List<PostViewModel>();
            BlogEntities db = new BlogEntities();

            viewModel = db.POSTs.SqlQuery("SELECT TOP 4 * FROM POST ORDER BY CREATED_DATE ASC").Select(c => new PostViewModel { TITLE = c.TITLE, IMAGE_COVER = c.IMAGE_COVER, SLUG = c.SLUG, CREATED_DATE = c.CREATED_DATE, CREATED_USER = c.CREATED_USER, MAIN_TAG = c.MAIN_TAG }).ToList();

            return PartialView(viewModel);
        }

        /// <summary>
        /// 3 most viewed Post by tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult PostsByTag(string tag)
        {
            PostsByTagViewModel viewModel = new PostsByTagViewModel();
            viewModel.list = new List<List<PostViewModel>>();
            BlogEntities db = new BlogEntities();

            List<string> tagsCount = db.Database.SqlQuery<string>("select MAIN_TAG+'#'+CONVERT(char,count(*)) from POST group by MAIN_TAG").ToList();
            if (tagsCount != null)
            {
                foreach (string item in tagsCount)
                {
                    List<PostViewModel> tmp = db.POSTs.SqlQuery(string.Format("select top 3 * from POST where MAIN_TAG='{0}' order by CREATED_DATE desc", item.Split('#')[0])).Select(c => new PostViewModel { TITLE = c.TITLE, IMAGE_COVER = c.IMAGE_COVER, SLUG = c.SLUG, CREATED_DATE = c.CREATED_DATE, CREATED_USER = c.CREATED_USER, MAIN_TAG = c.MAIN_TAG }).ToList();
                    if (tmp != null) { viewModel.list.Add(tmp); }
                }
            }

            return PartialView(viewModel);
        }

        //[ValidateAntiForgeryToken]
        public ActionResult AllPost()
        {
            int? from = 1;
            int? to = 5;
            BlogEntities db = new BlogEntities();
            List<PostViewModel> viewModel = new List<PostViewModel>();

            string sql = string.Format(@"SELECT * FROM ( 
                             SELECT 
                                  ROW_NUMBER() OVER (ORDER BY id desc) AS row, p.*
                             FROM POST p
                        ) AS a WHERE row BETWEEN {0} AND {1}", from == null ? 1 : from, to == null ? 5 : to);
            viewModel = db.POSTs.SqlQuery(sql).Select(c => new PostViewModel { TITLE = c.TITLE, IMAGE_COVER = c.IMAGE_COVER, SLUG = c.SLUG, CREATED_DATE = c.CREATED_DATE, CREATED_USER = c.CREATED_USER, MAIN_TAG = c.MAIN_TAG, META_DESC = c.META_DESC }).ToList();

            return PartialView(viewModel);
        }

        #endregion

        #region trang chi tiet
        /// <summary>
        /// GET POST CONTENT BY SLUG
        /// </summary>
        /// <param name="year">CREATED YEAR</param>
        /// <param name="month">CREATED MONTH</param>
        /// <param name="date">CREATED DATE</param>
        /// <param name="slug">SLUG VALUE</param>
        /// <returns></returns>
        [OutputCache(Duration = 30 * 60000)]
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

        #endregion

        #region trang danh sach post
        [Route("danh-sach-bai-viet")]
        public ActionResult ListPost(int? page, string category )
        {

            BlogEntities db = new BlogEntities();
            List<PostViewModel> viewModel = new List<PostViewModel>();
            string sql = "";

            if (category == null || category=="" )
            {
                sql = string.Format(@"SELECT * FROM POST ORDER BY CREATED_DATE DESC");
                //Session["notag"] = "1";
            }
            else
            {
                //if (Session["notag"].ToString() == "1")
                //    page = 1;


                //Session["notag"] = 0;
                List<TAG> tags = StaticCache.GetTags();
                if (tags.Any(c => c.NAME.ToUpper() == category.ToUpper()))
                    sql = string.Format(@"SELECT * FROM POST WHERE UPPER(MAIN_TAG)='{0}' ORDER BY CREATED_DATE DESC ", category.ToUpper());
                else
                    sql = string.Format(@"SELECT * FROM POST ORDER BY CREATED_DATE DESC");
            }

            viewModel = db.POSTs.SqlQuery(sql).Select(c => new PostViewModel { TITLE = c.TITLE, IMAGE_COVER = c.IMAGE_COVER, SLUG = c.SLUG, CREATED_DATE = c.CREATED_DATE, CREATED_USER = c.CREATED_USER, MAIN_TAG = c.MAIN_TAG, META_DESC = c.META_DESC }).ToList();

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ViewBag.tag = category;
            return View(viewModel.ToPagedList(pageNumber, pageSize));
        }
        #endregion

        #region partial view xai chung
        /// <summary>
        /// RETURN 404 PAGE
        /// </summary>
        /// <returns></returns>
        [Route(Name = "404")]
        [OutputCache(Duration = 30 * 60000)]
        public ActionResult NotFound()
        {
            return View();
        }

        [ChildActionOnly]
        [OutputCache(Duration = 30 * 60000)]
        public ActionResult RightColumn()
        {
            BlogEntities db = new BlogEntities();
            RightColumnViewModel viewModel = new RightColumnViewModel();

            //GET CATEGORIES COUNT
            string sql = "SELECT MAIN_TAG+'$'+CONVERT(NVARCHAR, COUNT(*)) FROM POST GROUP BY MAIN_TAG";
            viewModel.TagCounts = db.Database.SqlQuery<string>(sql).ToList();

            //GET POPULAR POST
            viewModel.PopularPosts = db.POSTs.SqlQuery("SELECT TOP 4 * FROM POST ORDER BY VIEW_COUNT DESC").Select(c => new PostViewModel { TITLE = c.TITLE, IMAGE_COVER = c.IMAGE_COVER, SLUG = c.SLUG, CREATED_DATE = c.CREATED_DATE, CREATED_USER = c.CREATED_USER, MAIN_TAG = c.MAIN_TAG }).ToList();

            return PartialView(viewModel);
        }
        #endregion





    }
}