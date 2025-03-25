using System.Globalization;

namespace Amethyst.Text;

public static class TextUtility
{
    const int Minutes = 60;
    const int Hours = 60 * 60;
    const int Days = 60 * 60 * 24;
    const int Months = 60 * 60 * 24 * 30;
    const int Years = 60 * 60 * 24 * 365;

    public static int ParseToSeconds(string from)
    {
        int time = 0;

        string numbers = "";
        foreach (char c in from)
        {
            if (char.IsDigit(c))
            {
                numbers += c;
                continue;
            }

            switch (c)
            {
                case 's': 
                case 'с': 
                    time += Convert.ToInt32(numbers, CultureInfo.InvariantCulture); 
                    break;
                case 'm': 
                case 'м': 
                    time += Convert.ToInt32(numbers, CultureInfo.InvariantCulture) * Minutes; 
                    break;
                case 'h': 
                case 'ч': 
                    time += Convert.ToInt32(numbers, CultureInfo.InvariantCulture) * Hours; 
                    break;
                case 'd': 
                case 'д': 
                    time += Convert.ToInt32(numbers, CultureInfo.InvariantCulture) * Days; 
                    break;
                case 'M': 
                case 'М': 
                    time += Convert.ToInt32(numbers, CultureInfo.InvariantCulture) * Months; 
                    break;
                case 'y': 
                case 'г': 
                    time += Convert.ToInt32(numbers, CultureInfo.InvariantCulture) * Years; 
                    break;
            }
            
            numbers = ""; 
        }

        return time;
    }

    public static List<string> SplitArguments(string text)
    {
        List<string> args = new List<string>();
        args.Add("");
        int index = 0;

        bool blockSpace = false;
        bool ignoreFormat = false;
        foreach (char c in text)
        {
            if (c == '"' && !ignoreFormat)
            {
                blockSpace = !blockSpace;
                ignoreFormat = false;
            }
            else if (c == ' ' && !ignoreFormat && !blockSpace)
            {
                args.Add("");
                index++;
                ignoreFormat = false;
            }
            else if (c == '\\' && !ignoreFormat) ignoreFormat = true;
            else
            {
                args[index] += c;
            }
        }

        return args;
    }
}