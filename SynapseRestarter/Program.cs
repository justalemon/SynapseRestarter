using System;
using System.Collections.Generic;
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

        List<ServiceController> services = new List<ServiceController>()
        {
            new ServiceController("Razer Synapse Service"),
            new ServiceController("Razer Game Manager Service"),
            new ServiceController("Razer Chroma Stream Server"),
            new ServiceController("Razer Chroma SDK Service"),
            new ServiceController("Razer Chroma SDK Server"),
            new ServiceController("RzActionSvc")
        };

        foreach (ServiceController service in services)
        {
            try
            {
                if (service.Status != ServiceControllerStatus.Stopped)
                {
                    Console.WriteLine($"Stopping {service.DisplayName} ({service.ServiceName})");
                    service.Stop();
                }
                else
                {
                    Console.WriteLine($"Process {service.DisplayName} ({service.ServiceName}) is already stopped");
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"Unable to stop {service.DisplayName} ({service.ServiceName}): {e}");
                return;
            }
        }

        Process.Start(executable);
    }
}
