using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Recruitment.Storage;

namespace Recruitment.DataAccess
{
	public interface IUserActionsRepository
	{
	    Task Pageview(string name, string page);
	}

    public class UserActionsRepository : IUserActionsRepository
    {
        IStorageProvider mStorageProvider;

        public UserActionsRepository(IStorageProvider storageProvider)
        {
            mStorageProvider = storageProvider;
        }

        public async Task Pageview(string name, string page)
        {
            var action = new UserAction
            {
                Action = string.Format("Pageview: {0}", page),
                CandidateName = name,
                DateTime = DateTime.UtcNow
            };

            await mStorageProvider.Create("user-actions", string.Format("{0}_{1}", name, Guid.NewGuid()), action);
        }
    }

    public class UserAction
    {
        public string Action { get; set; }
        public string CandidateName { get; set; }
        public DateTime DateTime { get; set; }
    }
}