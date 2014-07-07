using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure;
using System.Text;
using System.Net;
using Recruitment.Helpers;

namespace Recruitment.Storage
{
    public interface IStorageProvider
    {
        Task Create(string aContainer, string aKey, object aItem);
        Task<T> Read<T>(string aContainer, string aKey);
        Task Update(string aContainer, string aKey, object aItem);
        Task CreateOrUpdate(string aContainer, string aKey, object aItem);
        Task Delete(string aContainer, string aKey);
        Task<IEnumerable<IItemDescriptor>> List(string aContainer);
    }

    public class BlobStorageProvider : IStorageProvider
    {
        CloudStorageAccount mStorageAccount;
        CloudBlobClient mBlobClient;
        ISerializer mSerializer;

        public BlobStorageProvider(ISerializer aSerializer, IConfigurationRetriever aConfigurationRetriever)
        {
            mStorageAccount = CloudStorageAccount.Parse(aConfigurationRetriever.GetSetting("StorageConnectionString"));
            mBlobClient = mStorageAccount.CreateCloudBlobClient();
            mSerializer = aSerializer;
        }

        public async Task Create(string aContainer, string aKey, object aItem)
        {
            var container = await GetContainer(aContainer);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(aKey);

            var bytes = mSerializer.SerializeObject(aItem);

            try
            {
                await blockBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length, AccessCondition.GenerateIfNoneMatchCondition("*"), null, new OperationContext());
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.ExtendedErrorInformation.ErrorCode == "BlobAlreadyExists")
                {
                    throw new ItemAlreadyExistsException();
                }

                throw;
            }
        }

        public async Task<T> Read<T>(string aContainer, string aKey)
        {
            try
            {
                var container = await GetContainer(aContainer);
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(aKey);

                var content = await blockBlob.DownloadTextAsync();
                return mSerializer.DeserializeObject<T>(Encoding.UTF8.GetBytes(content));

            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == 404)
                {
                    throw new ItemDoesNotExitException();
                }

                throw new Exception();
            }
        }

        public async Task Update(string aContainer, string aKey, object aItem)
        {
            var container = await GetContainer(aContainer);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(aKey);

            var bytes = mSerializer.SerializeObject(aItem);

            try
            {
                await blockBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length, AccessCondition.GenerateIfMatchCondition("*"), null, new OperationContext());
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.ExtendedErrorInformation.ErrorCode == "ConditionNotMet")
                {
                    throw new ItemDoesNotExitException();
                }

                throw;
            }

        }

        public async Task CreateOrUpdate(string aContainer, string aKey, object aItem)
        {
            var container = await GetContainer(aContainer);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(aKey);

            var bytes = mSerializer.SerializeObject(aItem);
            await blockBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
        }

        public async Task Delete(string aContainer, string aKey)
        {
            var container = await GetContainer(aContainer);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(aKey);

            await blockBlob.DeleteAsync();
        }

        public async Task<IEnumerable<IItemDescriptor>> List(string aContainer)
        {
            var container = await GetContainer(aContainer);

            var results = new List<IListBlobItem>();
            BlobContinuationToken continuationToken = null;
            BlobResultSegment segment;
            do
            {
                segment = await container.ListBlobsSegmentedAsync(continuationToken);
                results.AddRange(segment.Results);
                continuationToken = segment.ContinuationToken;
            } while (continuationToken != null);

            return results.Select(x => new ItemDescriptor(aContainer, GetBlobName(x.Uri)));
        }

        private string GetBlobName(Uri aUri)
        {
            return HttpUtility.UrlDecode(aUri.Segments.Last());
        }

        private object syncRoot = new object();
        private Dictionary<string, CloudBlobContainer> mCachedContainers;
        public Dictionary<string, CloudBlobContainer> CachedContainers
        {
            get
            {
                if (mCachedContainers == null)
                {
                    lock (syncRoot)
                    {
                        if (mCachedContainers == null)
                        {
                            mCachedContainers = new Dictionary<string, CloudBlobContainer>();
                        }
                    }
                }

                return mCachedContainers;
            }
        }

        private async Task<CloudBlobContainer> GetContainer(string aContainer)
        {
            CloudBlobContainer container;
            if (!CachedContainers.TryGetValue(aContainer, out container))
            {
                CachedContainers[aContainer] = container = mBlobClient.GetContainerReference(aContainer.ToLower());
                await container.CreateIfNotExistsAsync();
            }

            return container;
        }
    }
}