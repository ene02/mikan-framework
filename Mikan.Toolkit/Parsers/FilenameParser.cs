using System.Text;

namespace Mikan.Toolkit.Parsers;

public static class FilenameParser
{
    /// <summary>
    /// Gets the actual filename from a file path.
    /// </summary>
    public static string Get(string filePath, bool canIncludeExtension = false)
    {
        StringBuilder noExtParsedName = new();
        StringBuilder fullParsedName = new(); // Used as a fallback in case the filename has no extension;

        bool isExtensionDotFound = false;

        char leftBar = '\\';

        char rightBar = '/';

        for (int i = filePath.Length - 1; i >= 0; i--)
        {
            if (filePath[i] == leftBar || filePath[i] == rightBar)
            {
                string fpn = fullParsedName.ToString();
                string nxn = noExtParsedName.ToString();

                if (canIncludeExtension || nxn == string.Empty)
                {
                    return new string(fpn.Reverse().ToArray());
                }
                else
                {
                    return new string(nxn.Reverse().ToArray());
                }
            }

            if (isExtensionDotFound)
            {
                noExtParsedName.Append(filePath[i]);
            }

            if (filePath[i] == '.')
            {
                isExtensionDotFound = true;
            }

            fullParsedName.Append(filePath[i]);
        }

        return string.Empty;
    }
}


