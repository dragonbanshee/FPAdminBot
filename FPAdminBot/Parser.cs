using Newtonsoft.Json;

namespace FPAdminBot
{
    public static class Parser
    {
        public static T ParseJSON<T>(this string @this) where T : class
        {
            return JsonConvert.DeserializeObject<T>(@this.Trim());
        }
    }
}
