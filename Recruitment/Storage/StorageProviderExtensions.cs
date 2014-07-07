using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Recruitment.Storage
{
    public static class StorageProviderExtensions
    {
        public static async Task<T> TryRead<T>(this IStorageProvider aProvider, string aContainer, string aKey) where T : class
        {
            T item = null;
            try
            {
                item = await aProvider.Read<T>(aContainer, aKey);
            }
            catch (ItemDoesNotExitException) { }

            return item;
        }
    }
}