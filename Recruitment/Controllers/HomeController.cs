using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Recruitment.DataAccess;

namespace Recruitment.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserActionsRepository mUserActionsRepository;

        public HomeController(IUserActionsRepository userActionsRepository)
        {
            mUserActionsRepository = userActionsRepository;
        }

        public ActionResult Index(string candidateName)
        {
            var name = string.IsNullOrEmpty(candidateName) ? "JohnDoe" : candidateName;

            ViewBag.CandidateName = name;
            mUserActionsRepository.Pageview(name, "rest-test");
            return View();
        }
    }
}