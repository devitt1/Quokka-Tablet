using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.ViewModels;

using TheQTablet.Core.Service.Implementations;
using TheQTablet.Core.Service.Interfaces;
using TheQTablet.Core.Rest.Implementations;
using TheQTablet.Core.Rest.Interfaces;
using TheQTablet.Core.ViewModels.Main;
using Acr.UserDialogs;

namespace TheQTablet.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            CreatableTypes()
                .EndingWith("Client")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            Mvx.IoCProvider.RegisterType<ISimulatorService, SimulatorService>();
            Mvx.IoCProvider.RegisterType<IRestClient, RestClient>();
            Mvx.IoCProvider.RegisterSingleton<IUserDialogs>(() => UserDialogs.Instance);

            RegisterAppStart<RootViewModel>();
        }
    }
}
