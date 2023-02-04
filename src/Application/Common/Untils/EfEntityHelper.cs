namespace Application.Common.Untils
{
    public static class EfEntityHelper
    {
        public static void UpdateEfMapperEx<TSource, TDest>(TSource source, TDest dest, bool isOverride = true)
        {
            if (source == null || dest == null) return;

            var sourceType = source.GetType();
            var destType = dest.GetType();

            foreach (var sourcePropertyInfo in sourceType.GetProperties())
            {
                // Source property info
                var sourceValue = sourcePropertyInfo.GetValue(source);
                var sourceSafeType = Nullable.GetUnderlyingType(sourcePropertyInfo.PropertyType) ?? sourcePropertyInfo.PropertyType;
                if (sourceValue == null) continue;

                // Update value to destination object
                var destPropertyInfo = destType.GetProperty(sourcePropertyInfo.Name);
                if (destPropertyInfo != null)
                {
                    // Convert value to non-nullable if dest type is Nullable<>
                    var destSafeType = Nullable.GetUnderlyingType(destPropertyInfo.PropertyType) ?? destPropertyInfo.PropertyType;
                    if (destPropertyInfo.GetValue(dest, null) != null && !isOverride) continue;

                    if (sourceSafeType == destSafeType)
                    {
                        destPropertyInfo.SetValue(dest, Convert.ChangeType(sourceValue, destSafeType), null);
                    }
                }
            }
        }
    }
}