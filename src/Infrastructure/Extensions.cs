namespace Media.Infrastructure;

internal static class Extensions
{
    public static string ChopOffBefore(this string s, string Before)
    {
        int end = s.IndexOf(Before, StringComparison.OrdinalIgnoreCase);
        if (end > -1)
        {
            return s[(end + Before.Length)..];
        }
        return s;
    }


    public static string ChopOffAfter(this string s, string After)
    {
        int end = s.IndexOf(After, StringComparison.OrdinalIgnoreCase);
        if (end > -1)
        {
            return s[..end];
        }
        return s;
    }

}
