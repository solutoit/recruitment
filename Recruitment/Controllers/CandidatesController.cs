using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Recruitment.DataAccess;
using Recruitment.Helpers;
using Recruitment.Models;

namespace Recruitment.Controllers
{
    public class CandidatesController : Controller
    {
        private readonly IUserActionsRepository mUserActionsRepository;
        private readonly ITokenRepository mTokenRepository;
        private readonly ICodeRepository mCodeRepository;
        private readonly IConfigurationRetriever mConfigurationRetriever;

        public CandidatesController(IUserActionsRepository aUserActionsRepository, ITokenRepository aTokenRepository, ICodeRepository aCodeRepository, IConfigurationRetriever aConfigurationRetriever)
        {
            mUserActionsRepository = aUserActionsRepository;
            mTokenRepository = aTokenRepository;
            mCodeRepository = aCodeRepository;
            mConfigurationRetriever = aConfigurationRetriever;
        }

        //
        // GET: /Candidates/
        public async Task<ActionResult> Solution(string candidateName, string password)
        {
            if (password != mConfigurationRetriever.GetSetting("Password"))
            {
                throw new HttpException(403, "Forbidden");
            }

            var token = await mTokenRepository.GetToken(candidateName);

            var userActions = new List<UserAction>();

            userActions.AddRange(await mUserActionsRepository.GetUserActions(candidateName));
            if (token == null && userActions.Count == 0)
            {
                return Json("Couldn't find anything on this candidate");
            }

            if (token != null)
            {
                userActions.AddRange(token.Usages.Select(x => new UserAction { Action = "API call", DateTime = x }));
                userActions.Add(new UserAction { Action = "API initialization", DateTime = token.CreationTime });
            }

            var solution = await mCodeRepository.GetCode(candidateName);

            var model = new CandidateModel
            {
                Name = candidateName,
                Actions = userActions.OrderBy(x => x.DateTime).ToList(),
                Solution = solution
            };

            return View(model);
        }
	}
}