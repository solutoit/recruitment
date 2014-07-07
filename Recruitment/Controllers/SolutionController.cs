using System.Threading.Tasks;
using System.Web;
using Recruitment.DataAccess;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Recruitment.Controllers
{
    public class SolutionController : ApiController
    {
        readonly ITokenRepository mTokenRepository;
        readonly ICodeRepository mCodeRepository;

        public SolutionController(ITokenRepository tokenRepository, ICodeRepository codeRepository)
        {
            mTokenRepository = tokenRepository;
            mCodeRepository = codeRepository;
        }

        [HttpPost]
        public async Task Post(string apiKey)
        {
            // not using [FromBody] to allow them to upload without content-type and content-length

            var authToken = HttpContext.Current.Request.Headers["Token"];
            var code = await Request.Content.ReadAsStringAsync();

            var token = await mTokenRepository.GetToken(apiKey, authToken);
            if (token == null)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Invalid token" };
                throw new HttpResponseException(resp);
            }

            if (token.UsageCount > 0)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Token can only be used once" };
                throw new HttpResponseException(resp);
            }

            if (token.CreationTime.AddMinutes(30) < DateTime.UtcNow)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Token expired" };
                throw new HttpResponseException(resp);
            }

            await mTokenRepository.IncrementTokenUsageCount(token);
            await mCodeRepository.UpdateCode(apiKey, code);
        }
    }
}
