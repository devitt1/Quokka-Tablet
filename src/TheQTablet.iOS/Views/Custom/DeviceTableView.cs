using System;

using Foundation;

using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;

using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class DeviceCell : MvxTableViewCell
    {
        public static readonly NSString CellId = new NSString("DeviceCell");

        private UILabel _name;

        DeviceCell(IntPtr handle) : base(handle)
        {
            ContentView.BackgroundColor = UIColor.Clear;
            ContentView.DirectionalLayoutMargins = new NSDirectionalEdgeInsets
            {
                Top = 10,
                Bottom = 10,
                Leading = 10,
                Trailing = 10,
            };

            _name = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = FontGenerator.GenerateFont(24, UIFontWeight.Regular),
                TextColor = ColorPalette.PrimaryText,
            };
            ContentView.AddSubview(_name);

            _name.TopAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TopAnchor).Active = true;
            _name.LeadingAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.LeadingAnchor).Active = true;
            _name.TrailingAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TrailingAnchor).Active = true;
            _name.BottomAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.BottomAnchor).Active = true;

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<DeviceCell, string>();
                set.Bind(_name).For(v => v.Text).To(vm => vm);
                set.Apply();
            });
        }
    }

    public class DeviceSource : MvxTableViewSource
    {
        public DeviceSource(UITableView tableView)
            : base(tableView)
        {
            tableView.RegisterClassForCellReuse(typeof(DeviceCell), DeviceCell.CellId);
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            var cell = tableView.DequeueReusableCell(DeviceCell.CellId, indexPath);
            cell.BackgroundColor = UIColor.Clear;
            return cell;
        }
    }
}
