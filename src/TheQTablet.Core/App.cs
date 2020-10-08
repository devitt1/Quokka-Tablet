using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.ViewModels;

using TheQTablet.Core.Service.Implementations;
using TheQTablet.Core.Service.Interfaces;
using TheQTablet.Core.Rest.Implementations;
using TheQTablet.Core.Rest.Interfaces;
using TheQTablet.Core.ViewModels.Main;

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
            Mvx.IoCProvider.RegisterType<IResultAccumulatorService, ResultAccumulatorService>();
            Mvx.IoCProvider.RegisterType<IRestClient, RestClient>();

            //RegisterAppStart<RootViewModel>();
            RegisterAppStart<PolarisationExperimentViewModel>();
        }
    }
}
