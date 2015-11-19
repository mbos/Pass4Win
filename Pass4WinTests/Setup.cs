

namespace Pass4WinTests
{
    using System.IO;
    using Autofac;
    using Moq;
    using Pass4Win;

    static class Setup
    {
        static internal ILifetimeScope Scope { get; set; }

        static internal void InitializeContainer()
        {
            var directoryProviderMock = new Mock<IDirectoryProvider>();

            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterInstance(directoryProviderMock).As<Mock<IDirectoryProvider>>();
            builder.RegisterInstance(directoryProviderMock.Object).As<IDirectoryProvider>();
            builder.RegisterInstance(new ConfigHandling()).AsSelf();
            builder.RegisterType<FrmKeyManager>().AsSelf();
            builder.RegisterType<FileSystemInterface>().AsSelf();
            Scope = builder.Build().BeginLifetimeScope();
        }
    }
}
