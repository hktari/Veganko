using System.Collections.Generic;
using Xamarin.Essentials;

namespace Veganko.Services.Storage
{
    public interface IPreferences
    {
        string Get(string key, string defaultValue);
        void Remove(string key);
        void Set(string key, string value);
    }

    public class XamarinPreferences : IPreferences
    {
        public string Get(string key, string defaultValue)
        {
            return Preferences.Get(key, defaultValue);
        }

        public void Set(string key, string value)
        {
            Preferences.Set(key, value);
        }

        public void Remove(string key)
        {
            Preferences.Remove(key);
        }
    }

    public class MockPreferences : IPreferences
    {
        private Dictionary<string, string> cache = new Dictionary<string, string>();

        public string Get(string key, string defaultValue)
        {
            if (!cache.TryGetValue(key, out var val))
            {
                return defaultValue;
            }
            return val;
        }

        public void Set(string key, string value)
        {
            cache[key] = value;
        }

        public void Remove(string key)
        {
            if (cache.ContainsKey(key))
            {
                cache.Remove(key);
            }
        }
    }
}
