using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlankBlog.Areas.AdminDashboard.Controllers
{
    public class adminHomeController : Controller
    {
        // GET: AdminDashboard/adminHome
        public ActionResult Index()
        { 
            return View();
        }
    }
}