using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Services.Logging;

namespace Veganko.Services.Resources
{
    public class ResourceProvider : IResourceProvider
    {
        private readonly ILogger logger;

        public ResourceProvider(ILogger logger)
        {
            this.logger = logger;
        }

        public TResource GetByKey<TResource>(string key)
        {
            if (!App.Current.Resources.TryGetValue(key, out object resource))
            {
                var ex = new Exception("Resource not found with key: " + key);
                logger.LogException(ex);
                throw ex;
            }

            if (!typeof(TResource).IsAssignableFrom(resource.GetType()))
            {
                throw new Exception($"Unexpected type of resource. Expecting {typeof(TResource)} for key: {key}");
            }

            return (TResource)resource;
        }
    }
}
