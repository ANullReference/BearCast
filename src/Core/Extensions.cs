using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace Core;

public static class Extensions
{
    /// <summary>
    /// turning #EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=2149280,CODECS=\"mp4a.40.2,avc1.64001f\",RESOLUTION=1280x720,NAME=\"720\"
    /// to key value pair
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string RemoveComasWithinDoubleQuotes(this string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        //loop through......hit double quote.....replace if encounter coma....continue.....
        char[] cleanChars = new char[s.Length];
        bool openingQuote = false;

        for (int i = 0; i < s.Length; i++)
        {
            if (openingQuote == false && s[i] == '"')
            {
                openingQuote = true;
                cleanChars[i] = s[i];
                continue;
            }

            if (openingQuote && s[i] == '"')
            {
                openingQuote = false;
                cleanChars[i] = s[i];
                continue;
            }

            if (openingQuote && s[i] == ',')
            {
                cleanChars[i] = '|';//replace , with |
                continue;
            }

            cleanChars[i] = s[i];
        }

        return new string(cleanChars);
    }
}
