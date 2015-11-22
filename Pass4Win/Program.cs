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

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [return: MarshalAs(UnmanagedType.Bool)]

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            RegisterTypes();
            bool createdNew = true;
            using (Mutex mutex = new Mutex(true, "Pass4Win", out createdNew))
            {
                if (createdNew)
                {

                    if (Environment.OSVersion.Version.Major >= 6)
                    {
                        SetProcessDPIAware();
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
            Scope = builder.Build().BeginLifetimeScope();
        }
    }
}