using Application.Common.Exceptions;

namespace Application.Common.Untils
{
    public static class ConvertHelper
    {
        public static TEnum ToEnum<TEnum>(string? str) where TEnum : struct
        {
            if (!string.IsNullOrEmpty(str) && Enum.TryParse<TEnum>(str, out TEnum enumData))
            {
                return enumData;
            }
            else
            {
                throw new Exception();
            }
        }
    }
}