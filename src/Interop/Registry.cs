using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace Media.Interop;

internal static class SystemRegistry
{
    public static void AddFolderToPath(string folderPath, ILogger logger)
    {
        const string keyName = "SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment";
        const string valueName = "Path";

        try
        {
            using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(keyName, true))
            {
                if (key != null)
                {
                    string currentPath = key.GetValue(valueName, string.Empty).ToString() ?? string.Empty;
                    if (!currentPath.Split(';').Contains(folderPath, StringComparer.OrdinalIgnoreCase))
                    {
                        string newPath = currentPath + ";" + folderPath;
                        key.SetValue(valueName, newPath, RegistryValueKind.ExpandString);
                        logger.LogInformation("Folder added to system PATH.");
                    }
                    else
                    {
                        logger.LogInformation("Folder is already in the system PATH.");
                    }
                }
                else
                {
                    logger.LogInformation("Failed to open registry key.");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Error: {message}", ex.Message);
        }
    }
}
