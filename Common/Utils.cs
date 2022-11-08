using System.ComponentModel;

namespace Common
{
    public static class Utils
    {
        public static T? Convert<T>(this string input)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));

                if (converter != null && converter.ConvertFromString(input) is T result)
                    return result;

                return default;
            }
            catch (NotSupportedException)
            {
                return default;
            }
        }
    }
}
