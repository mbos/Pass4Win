

namespace Pass4WinTests
{
    using System.IO;
    using Autofac;
    using Moq;
    using Pass4Win;
    using SharpConfig;

    static class Setup
    {
        static internal ILifetimeScope Scope { get; set; }

        static internal void InitializeContainer()
        {
            var directoryProviderMock = new Mock<IDirectoryProvider>();

            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterInstance(directoryProviderMock).As<Mock<IDirectoryProvider>>();
            builder.RegisterInstance(directoryProviderMock.Object).As<IDirectoryProvider>();
            builder.RegisterInstance(new Config("Pass4Win", false, true)).AsSelf();
            builder.RegisterType<FrmKeyManager>().AsSelf();
            builder.RegisterType<FileSystemInterface>().AsSelf();
            Scope = builder.Build().BeginLifetimeScope();
        }
    }
}
