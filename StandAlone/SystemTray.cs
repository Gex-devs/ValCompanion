using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ValRestServer
{
    internal static class SystemTray
    {
        private static NotifyIcon notifyIcon;
        private static ContextMenuStrip contextMenu;
        private static Thread restServerThread;
        private static bool isRunning;

        [STAThread]
        static void Main()
        {
            if(!CheckIfGameIsRunning())
            {
                MessageBox.Show("Valorant is Not Running", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create and configure the notify icon
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new Icon("Assets/TrayIcon.ico");
            notifyIcon.Text = "Valo Companion";
            notifyIcon.Visible = true;

            // Create the context menu
            contextMenu = CreateContextMenu();
            notifyIcon.ContextMenuStrip = contextMenu;

            isRunning = true;

            restServerThread = new Thread(RunRestServer);
            restServerThread.Start();

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

            // Add later if you have a settings window
            //contextMenu.Items.Add(settingsMenuItem);
            contextMenu.Items.Add(exitMenuItem);

            return contextMenu;
        }

        private static void RunRestServer()
        {
            RestServer.RunAsync().Wait();
        }

        private static void OnSettingsClicked(object sender, EventArgs e)
        {
            // Handle the settings menu item click
        }

        private static void OnExitClicked(object sender, EventArgs e)
        {
            // Clean up resources
            notifyIcon.Dispose();
            isRunning = false;

            // Wait for the thread to complete gracefully
            restServerThread.Join();

            // Exit the application
            Environment.Exit(0);
        }

        private static bool CheckIfGameIsRunning()
        {
            Process[] process = Process.GetProcessesByName("VALORANT");

            if(process.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

  
}
