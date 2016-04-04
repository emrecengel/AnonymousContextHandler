using Newtonsoft.Json;

namespace AnonymousContextHandler.Extensions
{
    internal static class StringExtensions
    {
        public static string ToJson<T>(this T selectedObject)
        {
            return JsonConvert.SerializeObject(selectedObject);
        }

        public static T ParseJson<T>(this string jsonString) where T : class
        {
            return JsonConvert.DeserializeObject<T>(jsonString.IfNullReturnEmpty()
                .Trim());
        }

        public static string IfNullReturnEmpty(this string value)
        {
            if (value == null)
                return string.Empty;

            return value;
        }
    }
}