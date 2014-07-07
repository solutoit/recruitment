using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Recruitment.Helpers
{
    public interface IConfigurationRetriever
    {
        string GetSetting(string aKey);
    }

    public class ConfigurationRetriever : IConfigurationRetriever
    {
        public string GetSetting(string aKey)
        {
            return CloudConfigurationManager.GetSetting(aKey);
        }
    }
}