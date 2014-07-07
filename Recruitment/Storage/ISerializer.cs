using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recruitment.Storage
{
    public interface ISerializer
    {
        byte[] SerializeObject(object aObject);
        T DeserializeObject<T>(byte[] aData);
    }
}
