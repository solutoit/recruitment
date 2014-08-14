using Recruitment.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Recruitment.DataAccess
{
    public interface ITokenRepository
    {
        Task<Token> GetOrCreateToken(string name);
        Task UpdateTokenUsage(Token token);
        Task<Token> GetToken(string name);
        Task<IEnumerable<string>> ListTokens();
    }

    public class TokenRepository : ITokenRepository
    {
        readonly IStorageProvider mStorageProvider;

        public TokenRepository(IStorageProvider storageProvider)
        {
            mStorageProvider = storageProvider;
        }

        public async Task<Token> GetOrCreateToken(string name)
        {
            var token = await mStorageProvider.TryRead<Token>("tokens", name);
            if (token == null)
            {
                token = new Token { Key = Guid.NewGuid().ToString(), Name = name };
                await mStorageProvider.Create("tokens", name, token);
                return token;
            }

            return token;
        }

        public async Task UpdateTokenUsage(Token token)
        {
            token.Usages.Add(DateTime.UtcNow);
            await mStorageProvider.Update("tokens", token.Name, token);
        }

        public async Task<Token> GetToken(string name)
        {
            var token = await mStorageProvider.TryRead<Token>("tokens", name);
            return token;
        }

        public async Task<IEnumerable<string>> ListTokens()
        {
            return (await mStorageProvider.List("tokens")).Select(x => x.Key);
        }
    }

    public class Token
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public DateTime CreationTime { get; set; }
        public List<DateTime> Usages { get; set; }

        public Token()
        {
            CreationTime = DateTime.UtcNow;
            Usages = new List<DateTime>();
        }
    }
}