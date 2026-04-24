using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Management;
using System.Windows.Forms;

namespace frmnetx86
{
    public partial class Form1 : Form
    {
        // Color constants for logging
        private static readonly Color ColorInfo = Color.White;   // System info
        private static readonly Color ColorProcess = Color.Yellow;  // Process/Activity
        private static readonly Color ColorSuccess = Color.Lime;    // Success (Code 0)
        private static readonly Color ColorWarning = Color.Orange;  // Warning (Code 3010)
        private static readonly Color ColorError = Color.Red;     // Critical error

        private bool lanInstalled = false;
        private BackgroundWorker installWorker = null;
        private string currentInstallingDriver = "";

        public Form1()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
        }

        private void InitializeBackgroundWorker()
        {
            installWorker = new BackgroundWorker();
            installWorker.DoWork += InstallWorker_DoWork;
            installWorker.RunWorkerCompleted += InstallWorker_RunWorkerCompleted;
            installWorker.ProgressChanged += InstallWorker_ProgressChanged;
            installWorker.WorkerReportsProgress = true;
        }

        private void InstallWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = Math.Min(e.ProgressPercentage, 100);
            if (e.UserState != null)
                lblnettus.Text = e.UserState.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            // Center lblos on form
            lblos.Left = (this.ClientSize.Width - lblos.Width) / 2;

            // Remove from startup if exists (after successful .NET installation)
            RemoveFromStartup();

            LogMessage("> === Network Driver Installer Started ===", ColorInfo);

            // Retrieve operating system information
            try
            {
                LogMessage("> Detecting operating system...", ColorProcess);
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
                {
                    ManagementObjectCollection information = searcher.Get();
                    if (information != null)
                    {
                        foreach (ManagementObject obj in information)
                        {
                            string osCaption = obj["Caption"]?.ToString() ?? "Unknown";
                            string osArch = obj["OSArchitecture"]?.ToString() ?? "Unknown";
                            string osInfo = $"{osCaption} - {osArch}";

                            // Normalize legacy OS names
                            osInfo = osInfo.Replace("NT 5.1.2600", "XP").Replace("NT 5.2.3790", "Server 2003");

                            lblos.Text = osInfo;
                            LogMessage($"> ✓ OS: {osInfo}", ColorSuccess);

                            // Check if x64 is not supported
                            if (osInfo.Contains("x64"))
                            {
                                LogMessage("> ✗ Error: x64 OS detected - not supported!", ColorError);
                                MessageBox.Show("This application does not support x64 operating systems.", "Unsupported OS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Application.Exit();
                            }

                            obj.Dispose();
                        }
                        information.Dispose();
                    }
                }
            }
            catch (ManagementException ex)
            {
                LogMessage($"> ✗ Error: {ex.Message}", ColorError);
                MessageBox.Show($"An error occurred while detecting the operating system: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            catch (UnauthorizedAccessException ex)
            {
                LogMessage($"> ✗ Error: Access denied - {ex.Message}", ColorError);
                MessageBox.Show($"Access denied while detecting the operating system: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            // Check WiFi support
            LogMessage("> Checking WiFi support...", ColorProcess);
            CheckAndEnableWifi();

            // Detect network hardware vendor
            LogMessage("> Detecting network hardware vendor...", ColorProcess);
            string vendor = GetNetworkHardwareID();
            LogMessage($"> ✓ Network Hardware Vendor: {vendor}", ColorSuccess);
        }

        private void CheckAndEnableWifi()
        {
            if (IsWifiSupported())
            {
                // WiFi is supported
                btnlan.Enabled = true;
                btnwifi.Enabled = false;
                lblnettus.Text = "WiFi is supported by the system. Install LAN driver first.";
                LogMessage("> ✓ WiFi adapter detected", ColorSuccess);
            }
            else
            {
                // WiFi is not supported
                btnwifi.Enabled = false;
                btnlan.Enabled = true;
                lblnettus.Text = "WiFi is not supported by the system.";
                LogMessage("> ⚠ WiFi adapter not found", ColorWarning);
            }
        }

        public bool IsWifiSupported()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionStatus = 2"))
                {
                    using (var collection = searcher.Get())
                    {
                        foreach (var device in collection)
                        {
                            string name = device["Name"]?.ToString().ToLower() ?? "";
                            string description = device["Description"]?.ToString().ToLower() ?? "";

                            // Check for wireless keywords in both Name and Description
                            if (IsWirelessDevice(name) || IsWirelessDevice(description))
                            {
                                device.Dispose();
                                return true;
                            }

                            device.Dispose();
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Silent fail - if WMI fails, assume WiFi not available
                return false;
            }
            return false;
        }

        private static bool IsWirelessDevice(string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName))
                return false;

            return deviceName.Contains("wireless")
                || deviceName.Contains("wi-fi")
                || deviceName.Contains("802.11")
                || deviceName.Contains("wlan")
                || deviceName.Contains("wifi");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = "Network Driver Installer (x86) - " + DateTime.Now.ToString();
        }

        private void btnlan_Click(object sender, EventArgs e)
        {
            // Check disk space before extraction
            if (!CheckDiskSpace())
            {
                MessageBox.Show("Insufficient disk space. Need at least 500MB free on C:\\ drive.", "Disk Space Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnlan.Enabled = false;
            lblnettus.Text = "Installing LAN driver...";
            currentInstallingDriver = "lanx86";

            // Run installation in background
            installWorker.RunWorkerAsync("lanx86.zip|lanx86|LAN driver");
        }

        private void btnwifi_Click(object sender, EventArgs e)
        {
            // Check if LAN driver was installed first
            if (!lanInstalled)
            {
                MessageBox.Show("Please install the LAN driver first before installing WiFi driver.", "LAN Driver Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check disk space before extraction
            if (!CheckDiskSpace())
            {
                MessageBox.Show("Insufficient disk space. Need at least 500MB free on C:\\ drive.", "Disk Space Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnwifi.Enabled = false;
            lblnettus.Text = "Installing WiFi driver...";
            currentInstallingDriver = "wifix86";

            // Run installation in background
            installWorker.RunWorkerAsync("wifix86.zip|wifix86|WiFi driver");
        }

        /// <summary>
        /// Check if C: drive has at least 500MB free space
        /// </summary>
        private bool CheckDiskSpace()
        {
            try
            {
                const long minimumSpaceNeeded = 500 * 1024 * 1024; // 500MB
                DriveInfo driveC = new DriveInfo("C");

                if (driveC.AvailableFreeSpace < minimumSpaceNeeded)
                {
                    long availableMB = driveC.AvailableFreeSpace / (1024 * 1024);
                    LogMessage($"> ✗ Low disk space: {availableMB}MB available (need 500MB)", ColorError);
                    return false;
                }

                long availableMB2 = driveC.AvailableFreeSpace / (1024 * 1024);
                LogMessage($"> ✓ Disk space OK: {availableMB2}MB available", ColorSuccess);
                return true;
            }
            catch (Exception ex)
            {
                LogMessage($"> ⚠ Disk check warning: {ex.Message}", ColorWarning);
                return true; // Allow to continue if check fails
            }
        }

        private void InstallWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string args = e.Argument as string;
            string[] parts = args.Split('|');
            string zipFile = parts[0];
            string folder = parts[1];
            string driverName = parts[2];

            try
            {
                // Step 1: Extract driver (20%)
                LogMessage($"> Step 1/4: Extracting {driverName}...", ColorProcess);
                installWorker.ReportProgress(10, $"[1/4] Extracting {driverName}...");

                if (ExtractDriverZip(zipFile, folder, driverName))
                {
                    installWorker.ReportProgress(20, "[1/4] Extraction complete");
                    LogMessage($"> ✓ Extraction complete", ColorSuccess);

                    // Step 2: Install driver (60%)
                    LogMessage($"> Step 2/4: Installing {driverName}...", ColorProcess);
                    installWorker.ReportProgress(30, "[2/4] Installing via PnPUtil...");

                    InstallDriver(folder);

                    installWorker.ReportProgress(60, "[2/4] Installation complete");
                    LogMessage($"> ✓ Installation complete", ColorSuccess);

                    // Step 3: Rescan hardware & cleanup (90%)
                    LogMessage($"> Step 3/4: Scanning hardware and cleaning up...", ColorProcess);
                    installWorker.ReportProgress(70, "[3/4] Rescanning hardware...");

                    RescanHardware();
                    CleanupDriverFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), folder));

                    installWorker.ReportProgress(90, "[3/4] Cleanup complete");
                    LogMessage($"> ✓ Cleanup complete", ColorSuccess);

                    // Step 4: Complete (100%)
                    LogMessage($"> Step 4/4: Installation finished!", ColorSuccess);
                    installWorker.ReportProgress(100, "[4/4] Installation finished!");
                }
                else
                {
                    LogMessage($"> ✗ Installation failed", ColorError);
                    installWorker.ReportProgress(0, "Installation failed");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"> ✗ Error: {ex.Message}", ColorError);
                installWorker.ReportProgress(0, "Error during installation");
            }
        }

        private void InstallWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (currentInstallingDriver == "lanx86")
            {
                lanInstalled = true;
                btnlan.Enabled = false;
                btnwifi.Enabled = true;
                lblnettus.Text = "LAN driver installed successfully. Now install WiFi driver.";
            }
            else if (currentInstallingDriver == "wifix86")
            {
                btnwifi.Enabled = false;
                lblnettus.Text = "Both drivers installed successfully!";
            }
        }
        public string GetNetworkHardwareID()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE ConfigManagerErrorCode = 0 OR ConfigManagerErrorCode = 28"))
                {
                    using (var collection = searcher.Get())
                    {
                        foreach (var device in collection)
                        {
                            try
                            {
                                object hardwareIDObj = device["HardwareID"];
                                if (hardwareIDObj != null)
                                {
                                    string[] hardwareIDs = hardwareIDObj as string[];
                                    if (hardwareIDs != null && hardwareIDs.Length > 0)
                                    {
                                        string hardwareID = hardwareIDs[0].ToUpper();

                                        // Detect manufacturer by Vendor ID
                                        if (hardwareID.Contains("VEN_10EC"))
                                            return "Realtek";
                                        if (hardwareID.Contains("VEN_8086"))
                                            return "Intel";
                                        if (hardwareID.Contains("VEN_168C"))
                                            return "Atheros";
                                        if (hardwareID.Contains("VEN_14E4"))
                                            return "Broadcom";
                                    }
                                }
                            }
                            finally
                            {
                                device?.Dispose();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error detecting network hardware: {ex.Message}", "Detection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return "Unknown";
        }
        private bool ExtractDriverZip(string zipFileName, string folderName, string driverName)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string destinationPath = Path.Combine(desktopPath, folderName);

                // Get ZIP data from embedded resources
                byte[] zipData = null;
                if (zipFileName == "lanx86.zip")
                {
                    zipData = Properties.Resources.lanx86;
                }
                else if (zipFileName == "wifix86.zip")
                {
                    zipData = Properties.Resources.wifix86;
                }

                if (zipData == null || zipData.Length == 0)
                {
                    LogMessage($"> ✗ Embedded resource not found: {zipFileName}", ColorError);
                    MessageBox.Show($"Driver package '{zipFileName}' not found in application resources.", "Resource Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                LogMessage($"> ✓ Loaded {zipFileName} from resources ({zipData.Length / 1024} KB)", ColorInfo);

                // Create destination directory if it doesn't exist
                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }
                else
                {
                    // Clear the directory if it already exists to avoid conflicts
                    try
                    {
                        DirectoryInfo di = new DirectoryInfo(destinationPath);
                        foreach (FileInfo file in di.GetFiles())
                        {
                            file.Delete();
                        }
                        foreach (DirectoryInfo dir in di.GetDirectories())
                        {
                            dir.Delete(true);
                        }
                    }
                    catch (Exception cleanEx)
                    {
                        LogMessage($"> ⚠ Could not clear existing files: {cleanEx.Message}", ColorWarning);
                    }
                }

                // Write ZIP to temp file, extract, then delete temp
                string tempZipPath = Path.Combine(Path.GetTempPath(), zipFileName);
                File.WriteAllBytes(tempZipPath, zipData);

                // Extract the zip file
                ZipFile.ExtractToDirectory(tempZipPath, destinationPath);

                // Delete temp ZIP
                File.Delete(tempZipPath);

                LogMessage($"> ✓ Extracted to: {destinationPath}", ColorSuccess);
                return true;
            }
            catch (IOException ioEx)
            {
                LogMessage($"> ✗ IO error during extraction: {ioEx.Message}", ColorError);
                MessageBox.Show($"An I/O error occurred while extracting the driver: {ioEx.Message}", "IO Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (InvalidOperationException zipEx)
            {
                LogMessage($"> ✗ Invalid zip data: {zipEx.Message}", ColorError);
                MessageBox.Show($"The driver package is invalid or corrupted: {zipEx.Message}", "Zip Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                LogMessage($"> ✗ Extraction error: {ex.Message}", ColorError);
                MessageBox.Show($"An error occurred while extracting the driver: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void InstallDriver(string folderName)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string driverPath = Path.Combine(desktopPath, folderName);

                // Check if driver folder exists
                if (!Directory.Exists(driverPath))
                {
                    LogMessage($"> ✗ Driver folder not found: {driverPath}", ColorError);
                    MessageBox.Show($"Driver folder not found: {driverPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check if .inf files exist
                string[] infFiles = Directory.GetFiles(driverPath, "*.inf", SearchOption.AllDirectories);
                if (infFiles.Length == 0)
                {
                    LogMessage($"> ✗ No .inf files found in: {driverPath}", ColorError);
                    MessageBox.Show($"No .inf driver files found in: {driverPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LogMessage($"> Installing {folderName}... ({infFiles.Length} .inf files)", ColorProcess);

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "pnputil.exe",
                    Arguments = $"/add-driver \"{driverPath}\\*.inf\" /install /subdirs /force",
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                };

                using (Process p = Process.Start(psi))
                {
                    if (p != null)
                    {
                        p.WaitForExit();
                        int exitCode = p.ExitCode;

                        // Handle different exit codes
                        if (exitCode == 0)
                        {
                            LogMessage($"> ✓ {folderName}: Installation successful (Code 0)", ColorSuccess);
                            MessageBox.Show($"Driver '{folderName}' installed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (exitCode == 3010)
                        {
                            LogMessage($"> ⚠ {folderName}: Success, restart required (Code 3010)", ColorWarning);
                            MessageBox.Show($"Driver '{folderName}' installed successfully!\n\nPlease restart your computer to apply the changes.", "Restart Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            LogMessage($"> ⚠ {folderName}: Exit code {exitCode}", ColorWarning);
                            MessageBox.Show($"Driver installation completed (Exit code: {exitCode})\n\nPlease check Device Manager for status.", "Installation Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                LogMessage($"> ✗ Admin required: {ex.Message}", ColorError);
                MessageBox.Show("This action requires Administrator privileges.\nPlease run the application as Administrator.", "Admin Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                LogMessage($"> ✗ Installation error: {ex.Message}", ColorError);
                MessageBox.Show($"Error during driver installation: {ex.Message}", "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Rescan hardware to force Windows to recognize newly installed drivers
        /// Tries multiple methods: devcon.exe, pnputil rescan, or suggests restart
        /// </summary>
        private void RescanHardware()
        {
            try
            {
                LogMessage("> Rescanning hardware...", ColorProcess);
                if (TryDevconRescan())
                {
                    LogMessage("> ✓ Hardware rescan completed", ColorSuccess);
                    return;
                }
                if (TryPnputilRescan())
                {
                    LogMessage("> ✓ Hardware rescan completed (via pnputil)", ColorSuccess);
                    return;
                }
                LogMessage("> ⚠ Consider restarting for full recognition", ColorWarning);
                MessageBox.Show("For optimal driver recognition, consider restarting your computer.", "Restart Suggested", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                LogMessage($"> ⚠ Rescan warning: {ex.Message}", ColorWarning);
            }
        }

        private bool TryDevconRescan()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "devcon.exe",
                    Arguments = "rescan",
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                };

                using (Process p = Process.Start(psi))
                {
                    if (p != null)
                    {
                        if (p.WaitForExit(5000))
                        {
                            return p.ExitCode == 0;
                        }
                    }
                }
            }
            catch
            {
                // devcon.exe not available
            }
            return false;
        }

        private bool TryPnputilRescan()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "pnputil.exe",
                    Arguments = "/scan-devices",
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                };

                using (Process p = Process.Start(psi))
                {
                    if (p != null)
                    {
                        if (p.WaitForExit(5000))
                        {
                            return true; // pnputil always returns success
                        }
                    }
                }
            }
            catch
            {
                // pnputil scan might not be available
            }
            return false;
        }

        private void CleanupDriverFolder(string folderPath)
        {
            try
            {
                LogMessage("> Cleaning up temporary files...", ColorProcess);
                foreach (string file in Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories))
                {
                    try { File.Delete(file); } catch { }
                }
                foreach (string dir in Directory.GetDirectories(folderPath))
                {
                    try
                    {
                        if (Directory.GetFileSystemEntries(dir).Length == 0)
                            Directory.Delete(dir);
                    }
                    catch { }
                }
                LogMessage("> ✓ Cleanup completed", ColorSuccess);
            }
            catch (Exception ex)
            {
                LogMessage($"> ⚠ Cleanup warning: {ex.Message}", ColorWarning);
            }
        }

        private void LogMessage(string message, Color? color = null)
        {
            try
            {
                if (rtbLog.InvokeRequired)
                    rtbLog.Invoke(new Action(() => AppendColoredText(rtbLog, message, color ?? ColorInfo)));
                else
                    AppendColoredText(rtbLog, message, color ?? ColorInfo);
            }
            catch { }
        }

        private void AppendColoredText(RichTextBox rtb, string text, Color color)
        {
            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;
            rtb.SelectionColor = color;
            rtb.AppendText(text + Environment.NewLine);
            rtb.SelectionColor = rtb.ForeColor;
            rtb.ScrollToCaret();
        }

        public void OpenDeviceManager()
        {
            try
            {
                LogMessage("> Opening Device Manager...", ColorProcess);
                Process.Start("devmgmt.msc");
                LogMessage("> ✓ Device Manager opened", ColorSuccess);
            }
            catch (Exception ex)
            {
                LogMessage($"> ✗ Device Manager error: {ex.Message}", ColorError);
                MessageBox.Show($"Could not open Device Manager: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDevMgr_Click(object sender, EventArgs e)
        {
            OpenDeviceManager();
        }

        /// <summary>
        /// Remove application from Windows startup
        /// </summary>
        private void RemoveFromStartup()
        {
            try
            {
                using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("NetworkDriverInstaller", false);
                    }
                }
            }
            catch { }
        }

        private void llbthanks_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Thank you for your support! Do you want to visit the developer's website?",
                "Thanks!",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                try
                {
                    Process.Start("https://anhdv.com");
                }
                catch (Exception ex)
                {
                    LogMessage($"> ✗ Could not open website: {ex.Message}", ColorError);
                    MessageBox.Show($"Could not open website: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Thank you for using this tool! If you have any feedback, please send it to me.", "Thank You!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }
        }

        private void llbdonate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form2 donateForm = new Form2();
            donateForm.ShowDialog();
        }
    }
}
