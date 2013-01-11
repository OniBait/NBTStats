using System.Collections.Generic;

namespace NBTStats
{
    public static class Extensions
    {
        public static TResult Get<T, TResult>(this Dictionary<T, TResult> dict, T key)
        {
            TResult result;
            return dict.TryGetValue(key, out result) ? result : default(TResult);
        }
    }
}