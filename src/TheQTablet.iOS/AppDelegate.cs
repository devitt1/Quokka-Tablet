using Foundation;
using MvvmCross.Platforms.Ios.Core;
using TheQTablet.Core;

namespace TheQTablet.iOS
{
    [Register(nameof(AppDelegate))]
    public class AppDelegate : MvxApplicationDelegate<Setup, App>
    {
    }
}
