namespace BoseCineMatePulse
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			//make sure only one instance of the application can be running at the same time
			using var mutex = new Mutex(false, "BoseCineMatePulse");
			if (!mutex.WaitOne(0, false))
			{
				MessageBox.Show("BoseCineMatePulse is already running.", "BoseCineMatePulse", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// we don't need a form, so we can just run the ApplicationContext directly
			Application.Run(new MyCustomApplicationContext());
		}

		public class MyCustomApplicationContext : ApplicationContext
		{
			private NotifyIcon notifyIcon;
			private Igniter igniter;

			public MyCustomApplicationContext()
			{
				notifyIcon = new();
				igniter = new();

				SetupNotifyIcon();
			}

			private void SetupNotifyIcon()
			{
				notifyIcon!.Text = "Bose CineMate Pulse";
				notifyIcon.Icon = new("BoseCineMatePulse.ico");
				notifyIcon.ContextMenuStrip = new();
				notifyIcon.ContextMenuStrip.Items.Add("Pulse", null, async (s, e) => await Ignite_ClickAsync(s, e));
				notifyIcon.ContextMenuStrip.Items.Add("About", null, ShowAboutMessageBox_Click);
				notifyIcon.ContextMenuStrip.Items.Add("Exit", null, ExitApplication_Click);

				notifyIcon!.Click += async (s, e) =>
				{
					if (e is MouseEventArgs me && me.Button == MouseButtons.Right)
					{
						return;
					}

					await igniter.IgniteAsync();
				};

				notifyIcon.Visible = true;
			}

			async Task Ignite_ClickAsync(object? sender, EventArgs? e)
			{
				await igniter!.IgniteAsync();
			}

			void ShowAboutMessageBox_Click(object? sender, EventArgs? e)
			{
				var packageVersion = typeof(Program).Assembly.GetName().Version;
				MessageBox.Show($"BoseCineMatePulse v{packageVersion}", "BoseCineMatePulse", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}

			private void ExitApplication_Click(object? sender, EventArgs e)
			{
				Environment.Exit(0);
			}
		}
	}
}