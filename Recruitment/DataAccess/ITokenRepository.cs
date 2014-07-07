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
        Task CreateToken(Token token);
        Task IncrementTokenUsageCount(Token token);
        Task<Token> GetToken(string name, string key);
    }

    public class TokenRepository : ITokenRepository
    {
        IStorageProvider mStorageProvider;

        public TokenRepository(IStorageProvider storageProvider)
        {
            mStorageProvider = storageProvider;
        }

        public async Task CreateToken(Token token)
        {
            await mStorageProvider.Create("tokens", GetTokenDescription(token.Key, token.Name), token);
        }

        public async Task IncrementTokenUsageCount(Token token)
        {
            token.UsageCount++;
            await mStorageProvider.Update("tokens", GetTokenDescription(token.Key, token.Name), token);
        }

        public async Task<Token> GetToken(string name, string key)
        {
            var token = await mStorageProvider.TryRead<Token>("tokens", GetTokenDescription(key, name));
            return token;
        }

        private string GetTokenDescription(string key, string identifier)
        {
            return string.Format("{0}_{1}", key, identifier);
        }

    }

    public class Token
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public DateTime CreationTime { get; set; }
        public int UsageCount { get; set; }

        public Token()
        {
            CreationTime = DateTime.UtcNow;
        }
    }
}