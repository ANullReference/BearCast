namespace Core;

public static class Extensions
{
    /// <summary>
    /// Remove comma's that are within double quotes. 
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
