using MvvmCross.IoC;
using MvvmCross.ViewModels;
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

            RegisterAppStart<RootViewModel>();
        }
    }
}
