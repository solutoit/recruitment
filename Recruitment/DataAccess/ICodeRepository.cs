using Recruitment.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Recruitment.DataAccess
{
   public interface ICodeRepository
    {
        Task UpdateCode(string name, string code);
    }

   public class CodeRepository : ICodeRepository
   {
       IStorageProvider mStorageProvider;

       public CodeRepository(IStorageProvider storageProvider)
       {
           mStorageProvider = storageProvider;
       }

       public async Task UpdateCode(string name, string code)
       {
           await mStorageProvider.CreateOrUpdate("code", name, code);
       }
   }
}