using System;
using System.Drawing;
using System.Windows.Forms;

namespace ValRestServer
{
    internal static class SystemTray
    {
        private static NotifyIcon notifyIcon;
        private static ContextMenuStrip contextMenu;
        private static Thread RestServerThread;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create and configure the notify icon
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon("Assets/TrayIcon.ico");
            notifyIcon.Text = "Valo Companion";
            notifyIcon.Visible = true;
            // Create the context menu
            contextMenu = CreateContextMenu();
            notifyIcon.ContextMenuStrip = contextMenu;

            RestServerThread = new Thread(() =>
            {
                RestServer.RunAsync();

            });

            RestServerThread.Start();

            notifyIcon.ShowBalloonTip(1000, "Notice", "Valo Companion is running in Background", ToolTipIcon.Info);

            Application.Run();
        }

        private static ContextMenuStrip CreateContextMenu()
        {
            var contextMenu = new ContextMenuStrip();

            var settingsMenuItem = new ToolStripMenuItem();
            settingsMenuItem.Text = "Settings";
            settingsMenuItem.Click += OnSettingsClicked;

            var exitMenuItem = new ToolStripMenuItem();
            exitMenuItem.Text = "Exit";
            exitMenuItem.Click += OnExitClicked;

            contextMenu.Items.Add(settingsMenuItem);
            contextMenu.Items.Add(exitMenuItem);

            return contextMenu;
        }

        private static void OnSettingsClicked(object sender, EventArgs e)
        {
            // Handle the settings menu item click
        }

        private static void OnExitClicked(object sender, EventArgs e)
        {
            // Clean up resources
            notifyIcon.Dispose();
            RestServerThread.Abort();
            // Exit the application
            Application.Exit();
        }
    }
}
