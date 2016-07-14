using System;
using System.Windows.Forms;
using Autofac;
using System.Threading;
using log4net.Config;
using System.IO;
using System.IO.IsolatedStorage;
using CommandLine;
using CommandLine.Text;

namespace Pass4Win
{
    internal static class Program
    {
        public static ILifetimeScope Scope { get; private set; }
        // logging
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // parsing command line
            string[] args = Environment.GetCommandLineArgs();
            string PersonalFolder = Path.GetTempPath() + "Pass4Win.log";
            /*
            // Set logfile location to personal directory
            if (args.Length > 1 && args[1].Equals("debug"))
            {
                PersonalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Pass4Win.log";
            }
            // Delete config file
            if (args.Length > 1 && args[1].Equals("resetconfig"))
            {
                IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
                isoStore.DeleteFile("Pass4Win.json");
            }
            */

            var options = new CmdLineOptions();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                // consume Options instance properties
                if (options.Debug)
                {
                    PersonalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Pass4Win.log";
                }
                if (options.Reset)
                {
                    IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
                    isoStore.DeleteFile("Pass4Win.json");
                }
                if (!options.UseGit)
                {
                    // do stuff
                }
            }

            // setting up logging

            Environment.SetEnvironmentVariable("log4netFileName", PersonalFolder + "\\Pass4Win.log");

            XmlConfigurator.Configure();
            log.Debug("Application started");

            ThreadExceptionHandler handler = new ThreadExceptionHandler();

            Application.ThreadException +=
                new ThreadExceptionEventHandler(
                    handler.Application_ThreadException);
            RegisterTypes();
            bool createdNew = true;
            log.Debug("Checking if not already loaded");
            using (Mutex mutex = new Mutex(true, "Pass4Win", out createdNew))
            {
                if (createdNew)
                {

                    if (Environment.OSVersion.Version.Major >= 6)
                    {
                        NativeMethods.SetProcessDPIAware();
                    }
                    log.Debug("Load main form");
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    try {
                        
                        Application.Run(Scope.Resolve<FrmMain>());
                    }
                    catch (Exception message)
                    {
                        log.Debug(message);
                    }
                    
                }
                else
                {
                    NativeMethods.PostMessage(
                                    (IntPtr)NativeMethods.HWND_BROADCAST,
                                    NativeMethods.WM_SHOWME,
                                    IntPtr.Zero,
                                    IntPtr.Zero);
                }
            }
        }

        private static void RegisterTypes()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<FrmMain>().InstancePerLifetimeScope().AsSelf();
            builder.RegisterInstance(new ConfigHandling()).AsSelf();
            builder.RegisterType<FrmKeyManager>().AsSelf();
            builder.RegisterType<FrmConfig>().AsSelf();
            builder.RegisterType<KeySelect>().AsSelf();
            builder.RegisterType<FrmAbout>().AsSelf();
            builder.RegisterType<FileSystemInterface>().AsSelf();
            builder.RegisterType<DirectoryProvider>().As<IDirectoryProvider>();
            builder.RegisterType<GitHandling>().AsSelf();

            Scope = builder.Build().BeginLifetimeScope();
        }

    }
    /// 
    /// Handles a thread (unhandled) exception.
    /// 
    internal class ThreadExceptionHandler
    {
        /// 
        /// Handles the thread exception.
        /// 
        public void Application_ThreadException(
            object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                // Exit the program if the user clicks Abort.
                DialogResult result = ShowThreadExceptionDialog(
                    e.Exception);

                if (result == DialogResult.Abort)
                    Application.Exit();
            }
            catch
            {
                // Fatal error, terminate program
                try
                {
                    MessageBox.Show("Fatal Error",
                        "Fatal Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }
        }

        /// 
        /// Creates and displays the error message.
        /// 
        private DialogResult ShowThreadExceptionDialog(Exception ex)
        {
            string errorMessage =
                "Unhandled Exception:\n\n" +
                ex.Message + "\n\n" +
                ex.GetType() +
                "\n\nStack Trace:\n" +
                ex.StackTrace;

            return MessageBox.Show(errorMessage,
                "Application Error",
                MessageBoxButtons.AbortRetryIgnore,
                MessageBoxIcon.Stop);
        }
    } // End ThreadExceptionHandler
}