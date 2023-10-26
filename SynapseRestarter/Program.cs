using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
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
        RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Razer Synapse");

        if (key?.GetValue("DisplayIcon") is not string iconFilePath)
        {
            return null;
        }

        string? synapsePath = Path.GetDirectoryName(Path.GetDirectoryName(iconFilePath));

        return synapsePath == null ? null : Path.Join(synapsePath, "WPFUI", "Framework", "Razer Synapse 3 Host", "Razer Synapse 3.exe");
    }
    
    /// <summary>
    /// The main entry point.
    /// </summary>
    public static void Main()
    {
        string? executable = GetExecutable();

        if (executable == null)
        {
            Console.Error.WriteLine("Razer Synapse 3 does not appears to be installed!");
            return;
        }

        if (!File.Exists(executable))
        {
            Console.Error.WriteLine("Razer Synapse 3 executable does not exists!");
            return;
        }
        
        ServiceController synapseController = new ServiceController("Razer Synapse Service");
        ServiceController gameManagerController = new ServiceController("Razer Game Manager Service");
        ServiceController chromaStreamController = new ServiceController("Razer Chroma Stream Server");
        ServiceController chromaSdlController = new ServiceController("Razer Chroma SDL Server");
        ServiceController chromaSdkController = new ServiceController("Razer Chroma SDK Server");
        ServiceController centralController = new ServiceController("RzActionSvc");

        try
        {
            synapseController.Stop();
            gameManagerController.Stop();
            chromaStreamController.Stop();
            chromaSdlController.Stop();
            chromaSdkController.Stop();
            centralController.Stop();
        }
        catch (InvalidOperationException)
        {
            Console.Error.WriteLine("Access denied to stop the Razer Synapse 3 services");
            return;
        }

        Process.Start(executable);
    }
}
