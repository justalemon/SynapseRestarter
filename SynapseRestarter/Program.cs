using System.IO;
using Microsoft.Win32;

namespace SynapseRestarter;

/// <summary>
/// The core of SynapseRestarter.
/// </summary>
public static class Program
{
    /// <summary>
    /// Gets the executable of Razer Synapse 3.
    /// </summary>
    /// <returns>The path to the main Razer Synapse 3 executable.</returns>
    public static string? GetExecutable()
    {
        RegistryKey? key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Razer Synapse");

        if (key == null)
        {
            return null;
        }

        object? icon = key.GetValue("DisplayIcon");

        if (icon is not string iconFilePath)
        {
            return null;
        }

        string? synapsePath = Path.GetDirectoryName(Path.GetDirectoryName(iconFilePath));

        if (synapsePath == null)
        {
            return null;
        }

        return Path.Join(synapsePath, "WPFUI", "Framework", "Razer Synapse 3 Host", "Razer Synapse 3.exe");
    }
    
    /// <summary>
    /// The main entry point.
    /// </summary>
    public static void Main()
    {
    }
}
