using System;
using System.Globalization;

using Foundation;

using MvvmCross.Binding.BindingContext;
using MvvmCross.Converters;
using MvvmCross.Platforms.Ios.Binding.Views;

using TheQTablet.Core.Service.Interfaces;

using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class AuthHideLockConverter : MvxValueConverter<AuthType, bool>
    {
        protected override bool Convert(AuthType value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return value == AuthType.None;
        }
    }

    public class NetworkCell : MvxTableViewCell
    {
        public static readonly NSString CellId = new NSString("NetworkCell");

        private UIImageView _lock;
        private UILabel _ssid;

        NetworkCell(IntPtr handle) : base(handle)
        {
            ContentView.BackgroundColor = UIColor.Clear;
            ContentView.DirectionalLayoutMargins = new NSDirectionalEdgeInsets
            {
                Top = 10,
                Bottom = 10,
                Leading = 10,
                Trailing = 10,
            };

            _lock = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("lock"),
            };
            ContentView.AddSubview(_lock);

            _ssid = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = FontGenerator.GenerateFont(24, UIFontWeight.Regular),
                TextColor = ColorPalette.PrimaryText,
                Lines = 0,
            };
            ContentView.AddSubview(_ssid);

            _lock.TopAnchor.ConstraintGreaterThanOrEqualTo(ContentView.LayoutMarginsGuide.TopAnchor).Active = true;
            _lock.LeadingAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.LeadingAnchor).Active = true;
            _lock.BottomAnchor.ConstraintLessThanOrEqualTo(ContentView.LayoutMarginsGuide.BottomAnchor).Active = true;
            _lock.HeightAnchor.ConstraintEqualTo(24).Active = true;
            _lock.AspectRatioConstraint().Active = true;

            _ssid.TopAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TopAnchor).Active = true;
            _ssid.LeadingAnchor.ConstraintEqualTo(_lock.TrailingAnchor, 10).Active = true;
            _ssid.TrailingAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TrailingAnchor).Active = true;
            _ssid.BottomAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.BottomAnchor).Active = true;

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<NetworkCell, WiFiNetwork>();
                set.Bind(_ssid).For(v => v.Text).To(vm => vm.SSID);
                set.Bind(_lock).For(v => v.Hidden).To(vm => vm.Auth).WithConversion<AuthHideLockConverter>();
                set.Apply();
            });
        }
    }

    public class NetworkSource : MvxTableViewSource
    {
        public NetworkSource(UITableView tableView)
            : base(tableView)
        {
            tableView.RegisterClassForCellReuse(typeof(NetworkCell), NetworkCell.CellId);
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            var cell = tableView.DequeueReusableCell(NetworkCell.CellId, indexPath);
            cell.BackgroundColor = UIColor.Clear;
            return cell;
        }
    }
}
