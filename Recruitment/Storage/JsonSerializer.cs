using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Recruitment.Storage
{
    public class JsonSerializer : ISerializer
    {
        public byte[] SerializeObject(object aObject)
        {
            var json = JsonConvert.SerializeObject(aObject);
            return Encoding.UTF8.GetBytes(json);
        }

        public T DeserializeObject<T>(byte[] aData)
        {
            var json = Encoding.UTF8.GetString(aData);
            return JsonConvert.DeserializeObject<T>(json);
        }

    }
}