using System.Web.Script.Serialization;

namespace FPAdminBot
{
    public static class Parser
    {
        private static JavaScriptSerializer json;
        private static JavaScriptSerializer JSON
        {
            get
            {
                return json ?? (json = new JavaScriptSerializer());
            }
        }
        
        public static T ParseJSON<T>(this string @this) where T : class
        {
            return JSON.Deserialize<T>(@this.Trim());
        }
    }
}
