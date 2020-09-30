using MvvmCross.Platforms.Ios.Core;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Plugin.Json;

using TheQTablet.Core;

namespace TheQTablet.iOS
{
    public class Setup : MvxIosSetup<App>
    {
        protected override void InitializeFirstChance()
        {
            base.InitializeFirstChance();
            Mvx.IoCProvider.RegisterType<IMvxJsonConverter, MvxJsonConverter>();
        }

    }
}
