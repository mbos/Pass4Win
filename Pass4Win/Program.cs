using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Autofac;
using System.Threading;
using System.Diagnostics;

namespace Pass4Win
{
    internal static class Program
    {
        public static ILifetimeScope Scope { get; private set; }

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            ThreadExceptionHandler handler =
            new ThreadExceptionHandler();

            Application.ThreadException +=
                new ThreadExceptionEventHandler(
                    handler.Application_ThreadException);
            RegisterTypes();
            bool createdNew = true;
            using (Mutex mutex = new Mutex(true, "Pass4Win", out createdNew))
            {
                if (createdNew)
                {

                    if (Environment.OSVersion.Version.Major >= 6)
                    {
                        NativeMethods.SetProcessDPIAware();
                    }
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
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