using System;

using MvvmCross.Platforms.Ios.Binding.Views;

using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class ConnectivityTableView<SourceType> : UIView where SourceType : MvxTableViewSource
    {
        private UILabel _title;
        private UITableView _list;
        private SourceType _source;

        public SourceType Source => _source;

        public ConnectivityTableView(string title)
        {
            TranslatesAutoresizingMaskIntoConstraints = false;

            _title = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = ColorPalette.SecondaryText,
                Font = FontGenerator.GenerateFont(32, UIFontWeight.Regular),
                Text = title,
            };
            AddSubview(_title);

            _list = new UITableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear,
                TableFooterView = new UIView(),
                SeparatorColor = ColorPalette.Border,
            };
            _source = (SourceType)Activator.CreateInstance(typeof(SourceType), _list);
            _list.Source = _source;
            AddSubview(_list);

            _title.TopAnchor.ConstraintEqualTo(_title.Superview.TopAnchor).Active = true;
            _title.LeftAnchor.ConstraintEqualTo(_title.Superview.LeftAnchor).Active = true;
            _title.RightAnchor.ConstraintEqualTo(_title.Superview.RightAnchor).Active = true;

            _list.LeftAnchor.ConstraintEqualTo(_list.Superview.LeftAnchor).Active = true;
            _list.RightAnchor.ConstraintEqualTo(_list.Superview.RightAnchor).Active = true;
            _list.TopAnchor.ConstraintEqualTo(_title.BottomAnchor).Active = true;
            _list.BottomAnchor.ConstraintEqualTo(_list.Superview.BottomAnchor).Active = true;
        }
    }
}
