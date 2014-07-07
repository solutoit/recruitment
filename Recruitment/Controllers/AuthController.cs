using Recruitment.DataAccess;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Recruitment.Controllers
{
    public class AuthController : ApiController
    {
        ITokenRepository mTokenRepository;

        public AuthController(ITokenRepository tokenRepository)
        {
            mTokenRepository = tokenRepository;
        }

        [HttpGet]
        public async Task<string> Get(string apiKey)
        {
            var token = new Token { Key = Guid.NewGuid().ToString(), Name = apiKey };

            await mTokenRepository.CreateToken(token);

            return token.Key;
        }
    }

}
