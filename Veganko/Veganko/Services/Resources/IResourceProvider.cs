using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Services.Resources
{
    public interface IResourceProvider
    {
        TResource GetByKey<TResource>(string key);
    }
}
