namespace Application.Common.Extensions
{
    public static class TypeEx
    {
        public static Dictionary<string, object> ToDictionary(this Type type, object obj)
        {
            var keyValuePairs = new Dictionary<string, object>();
            type.GetProperties()
                .ToList()
                .ForEach(property =>
                {
                    var key = property.Name;
                    var value = property.GetValue(obj, null);
                    if (value != null)
                    {
                        keyValuePairs.Add(key, value);
                    }
                });

            return keyValuePairs;
        }
    }
}