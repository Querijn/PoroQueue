using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace PoroQueueWindow
{
    static class Program
    {
        public const string AppName = "PoroQueue";
        static NotifyIcon TrayIcon;
        static RegistryKey BootKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        static MenuItem StartOnBootMenuItem;
        static Settings SettingsMenu;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                using (new SingleGlobalInstance(500)) // Wait 500 seconds max for other programs to stop
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    SettingsMenu = new Settings();

                    TrayIcon = new NotifyIcon()
                    {
                        Icon = Properties.Resources.PoroIcon,
                        Visible = true,
                        BalloonTipTitle = "Poro Queue"
                    };
                    UpdateMenuItems();

                    Application.Run();
                }
            }
            catch (TimeoutException)
            {
                MessageBox.Show("PoroQueue is already running!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private static void ShowSettings()
        {
            if (SettingsMenu.Visible)
            {
                SettingsMenu.BringToFront();
                SettingsMenu.Activate();
                SettingsMenu.Focus();
            }
            else SettingsMenu.Show();
        }

        private static void UpdateMenuItems()
        {
            StartOnBootMenuItem = new MenuItem("Start with Windows", (s, e) => ToggleStartOnBoot());
            StartOnBootMenuItem.Checked = WillStartOnBoot;

            TrayIcon.DoubleClick += (s, e) => ShowSettings();
            var SettingsMenuItem = new MenuItem("Settings", (s, e) => ShowSettings());

            var QuitMenuItem = new MenuItem("Quit", (a, b) =>
            {
                TrayIcon.Dispose();
                Application.Exit();
                Environment.Exit(1);
            });
            TrayIcon.ContextMenu = new ContextMenu(new MenuItem[] { SettingsMenuItem, StartOnBootMenuItem, QuitMenuItem });
        }

        private static bool WillStartOnBoot { get { return BootKey.GetValue(AppName) != null; } }

        private static void ToggleStartOnBoot()
        {
            if (!WillStartOnBoot)
                BootKey.SetValue(AppName, Application.ExecutablePath);
            else BootKey.DeleteValue(AppName, false);

            TrayIcon.BalloonTipText = $"{AppName} {(WillStartOnBoot ? "will now" : "won't")} start with Windows from now on.";
            TrayIcon.ShowBalloonTip(1000);

            // Update menu state
            StartOnBootMenuItem.Checked = WillStartOnBoot;
        }
    }
}
