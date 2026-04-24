using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace frmnetx86
{
    internal static class Program
    {
        private const string NET_INSTALLER = "net452.exe";
        private const string RESTART_FLAG = "RestartAfterNetInstall";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Check if we're returning from a restart after .NET installation
            bool returningFromRestart = CheckRestartFlag();

            // Check .NET Framework version
            if (!CheckDotNetFramework())
            {
                if (returningFromRestart)
                {
                    // We already tried to install .NET and restarted, but still not found
                    MessageBox.Show(
                        ".NET Framework 4.5.2 installation may have failed.\n\n" +
                        "Please install it manually from:\n" +
                        "https://dotnet.microsoft.com/download/dotnet-framework/net452\n\n" +
                        "Then restart your computer and try again.",
                        "Installation Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    ClearRestartFlag();
                    return;
                }

                // First time - try to install .NET Framework
                InstallDotNetFramework();
                return;
            }

            // .NET Framework is installed - clear restart flag if exists
            ClearRestartFlag();

            // Start the main application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        /// <summary>
        /// Check if .NET Framework 4.5.2 or higher is installed
        /// </summary>
        private static bool CheckDotNetFramework()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"))
                {
                    if (key == null)
                        return false;

                    object release = key.GetValue("Release");
                    if (release == null)
                        return false;

                    int releaseKey = (int)release;

                    // .NET Framework 4.5.2 = 379893
                    return releaseKey >= 379893;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Install .NET Framework 4.5.2 from bundled installer
        /// </summary>
        private static void InstallDotNetFramework()
        {
            try
            {
                // Extract net452.exe from embedded resources to temp
                string tempDir = Path.GetTempPath();
                string installerPath = Path.Combine(tempDir, NET_INSTALLER);

                // Check if embedded resource exists
                byte[] installerData = Properties.Resources.net452;
                if (installerData == null || installerData.Length == 0)
                {
                    // Fallback: Try to find net452.exe in same directory
                    string appDir = AppDomain.CurrentDomain.BaseDirectory;
                    string externalPath = Path.Combine(appDir, NET_INSTALLER);

                    if (File.Exists(externalPath))
                    {
                        installerPath = externalPath;
                    }
                    else
                    {
                        MessageBox.Show(
                            $".NET Framework 4.5.2 is required but not installed.\n\n" +
                            $"The installer was not found (embedded or external).\n\n" +
                            "Please download .NET Framework 4.5.2 from:\n" +
                            "https://dotnet.microsoft.com/download/dotnet-framework/net452",
                            "Missing Installer",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    // Write embedded installer to temp folder
                    File.WriteAllBytes(installerPath, installerData);
                }

                // Ask user for permission to install
                DialogResult result = MessageBox.Show(
                    ".NET Framework 4.5.2 is required to run this application.\n\n" +
                    "The installer will now run and your computer will need to restart.\n\n" +
                    "After restart, this application will automatically start again.\n\n" +
                    "Do you want to continue?",
                    "Install .NET Framework 4.5.2",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    // Clean up temp file if created
                    try { if (File.Exists(installerPath)) File.Delete(installerPath); } catch { }
                    return;
                }

                // Set restart flag
                SetRestartFlag();

                // Start .NET installer
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = installerPath,
                    Arguments = "/passive /norestart",
                    Verb = "runas",
                    UseShellExecute = true
                };

                using (Process process = Process.Start(psi))
                {
                    if (process != null)
                    {
                        MessageBox.Show(
                            ".NET Framework installation is in progress...\n\n" +
                            "Please wait for the installation to complete.\n" +
                            "This may take 2-5 minutes.\n\n" +
                            "Click OK after installation window closes.",
                            "Installing...",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        process.WaitForExit();
                        int exitCode = process.ExitCode;

                        // Clean up temp installer
                        try { if (File.Exists(installerPath) && installerPath.StartsWith(tempDir)) File.Delete(installerPath); } catch { }

                        if (exitCode == 0 || exitCode == 3010)
                        {
                            DialogResult restartResult = MessageBox.Show(
                                ".NET Framework 4.5.2 has been installed successfully!\n\n" +
                                "Your computer needs to restart to complete the installation.\n\n" +
                                "Click OK to restart now.",
                                "Restart Required",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Information);

                            if (restartResult == DialogResult.OK)
                            {
                                AddToStartup();
                                Process.Start("shutdown", "/r /t 10 /c \"Restarting to complete .NET Framework installation\"");
                            }
                        }
                        else if (exitCode == 1602)
                        {
                            ClearRestartFlag();
                            MessageBox.Show(
                                "Installation was cancelled.\n\n" +
                                "The application cannot run without .NET Framework 4.5.2.",
                                "Installation Cancelled",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                        else
                        {
                            ClearRestartFlag();
                            MessageBox.Show(
                                $"Installation failed with exit code: {exitCode}\n\n" +
                                "Please try installing .NET Framework 4.5.2 manually.",
                                "Installation Failed",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ClearRestartFlag();
                MessageBox.Show(
                    $"Error starting .NET Framework installer:\n\n{ex.Message}\n\n" +
                    "Please install .NET Framework 4.5.2 manually.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Set flag to indicate we're waiting for restart after .NET install
        /// </summary>
        private static void SetRestartFlag()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\NetworkDriverInstaller"))
                {
                    key.SetValue(RESTART_FLAG, "1");
                }
            }
            catch { }
        }

        /// <summary>
        /// Check if restart flag is set
        /// </summary>
        private static bool CheckRestartFlag()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\NetworkDriverInstaller"))
                {
                    if (key == null)
                        return false;

                    object value = key.GetValue(RESTART_FLAG);
                    return value != null && value.ToString() == "1";
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Clear restart flag
        /// </summary>
        private static void ClearRestartFlag()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\NetworkDriverInstaller", true))
                {
                    if (key != null)
                    {
                        key.DeleteValue(RESTART_FLAG, false);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Add application to startup for auto-launch after restart
        /// </summary>
        private static void AddToStartup()
        {
            try
            {
                string appPath = Process.GetCurrentProcess().MainModule.FileName;
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    key.SetValue("NetworkDriverInstaller", $"\"{appPath}\"");
                }
            }
            catch { }
        }

        /// <summary>
        /// Remove application from startup after successful launch
        /// </summary>
        private static void RemoveFromStartup()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("NetworkDriverInstaller", false);
                    }
                }
            }
            catch { }
        }
    }
}
