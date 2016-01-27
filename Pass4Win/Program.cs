using System;
using System.Windows.Forms;
using Autofac;
using System.Threading;
using log4net.Config;

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
                    try {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                    }
                    catch (Exception message)
                    {
                        log.Debug(message);
                    }
                    Application.Run(Scope.Resolve<FrmMain>());
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