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
        private const int TokenExpirationInMinutes = 300;

        public SolutionController(ITokenRepository tokenRepository, ICodeRepository codeRepository)
        {
            mTokenRepository = tokenRepository;
            mCodeRepository = codeRepository;
        }

        [HttpGet]
        public async Task<string> Get(string apiKey)
        {
            var code = await mCodeRepository.GetCode(apiKey);
            if (string.IsNullOrEmpty(code))
            {
                return "You didn't upload anything yet";
            }

            return code;
        }

        [HttpPost]
        public async Task Post(string apiKey)
        {
            // not using [FromBody] to allow them to upload without content-type and content-length

            var authToken = HttpContext.Current.Request.Headers["Token"];
            var code = await Request.Content.ReadAsStringAsync();

            var token = await mTokenRepository.GetToken(apiKey);
            if (token == null)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Token does not exist" };
                throw new HttpResponseException(resp);
            }

            if (token.Key != authToken)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Invalid token" };
                throw new HttpResponseException(resp);
            }

            if (token.CreationTime.AddMinutes(TokenExpirationInMinutes) < DateTime.UtcNow)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Token expired" };
                throw new HttpResponseException(resp);
            }

            await mTokenRepository.IncrementTokenUsageCount(token);
            await mCodeRepository.UpdateCode(apiKey, code);
        }
    }
}
