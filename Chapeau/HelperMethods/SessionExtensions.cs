using Newtonsoft.Json;

namespace Chapeau.HelperMethods
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            // Convert the object to a JSON string
            string json = JsonConvert.SerializeObject(value);

            // Store the string in session under the given key
            session.SetString(key, json);
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            // Read the string from session
            string json = session.GetString(key);

            // If not found, return default (null or 0)
            if (json == null)
            {
                return default(T);
            }

            // Convert the JSON string back to an object of type T
            T result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
    }
}
