using System;
using System.Globalization;
using System.Runtime.CompilerServices;

using CoreGraphics;

using Foundation;

using MvvmCross.Binding.BindingContext;
using MvvmCross.Converters;
using MvvmCross.Platforms.Ios.Binding.Views;

using TheQTablet.Core.ViewModels.Main;

using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class LessonSource : MvxCollectionViewSource
    {
        public LessonSource(UICollectionView collectionView)
            : base(collectionView, LessonCell.CellId)
        {
            collectionView.RegisterClassForCell(typeof(LessonCell), LessonCell.CellId);
            ReloadOnAllItemsSourceSets = true;
        }

        [Export("collectionView:layout:sizeForItemAtIndexPath:"), CompilerGenerated]
        public virtual CGSize GetSizeForItem(UICollectionView collectionView, FixedRowCountFlowLayout layout, NSIndexPath indexPath)
        {
            var fullWidth = (
                collectionView.Frame.Width
                - layout.SectionInset.Left
                - layout.SectionInset.Right
                - (layout.MinimumInteritemSpacing * (layout.CountPerRow - 1.0f))
            );
            var size = fullWidth / (float)layout.CountPerRow;
            return new CGSize(size, size);
        }
    }

    public class FixedRowCountFlowLayout : UICollectionViewFlowLayout
    {
        public int CountPerRow;
    }

    public class LessonNumberConverter : MvxValueConverter<int, string>
    {
        protected override string Convert(int value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return "LESSON " + value;
        }
    }
    
    public class LessonImageConverter : MvxValueConverter<int, UIImage>
    {
        protected override UIImage Convert(int value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return UIImage.FromBundle("lesson" + value.ToString("00") + "_preview");
        }
    }

    public class LessonCell : MvxCollectionViewCell
    {
        public static readonly NSString CellId = new NSString("LessonCell");

        private UIView _clipView;

        private UIImageView _background;

        private UIView _textContainer;
        private UILabel _numberLabel;
        private UILabel _titleLabel;

        LessonCell(IntPtr handle) : base(handle)
        {
            ContentView.Layer.ShadowRadius = 5;
            ContentView.Layer.ShadowOffset = new CGSize(5, 5);
            ContentView.Layer.ShadowColor = UIColor.Black.CGColor;
            ContentView.Layer.ShadowOpacity = 0.5f;

            _clipView = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            _clipView.Layer.CornerRadius = 20;
            _clipView.BackgroundColor = ColorPalette.BackgroundLight;
            _clipView.ClipsToBounds = true;
            ContentView.AddSubview(_clipView);

            _background = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ContentMode = UIViewContentMode.ScaleAspectFill,
            };
            _clipView.AddSubview(_background);

            _textContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = ColorPalette.BackgroundDark,
                DirectionalLayoutMargins = new NSDirectionalEdgeInsets
                {
                    Leading = 20,
                    Trailing = 20,
                    Top = 20,
                    Bottom = 20,
                }
            };
            _clipView.AddSubview(_textContainer);

            _numberLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = FontGenerator.GenerateFont(24, UIFontWeight.Regular),
                TextColor = ColorPalette.SecondaryText,
            };
            _textContainer.AddSubview(_numberLabel);

            _titleLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = FontGenerator.GenerateFont(24, UIFontWeight.Regular),
                TextColor = ColorPalette.PrimaryText,
            };
            _textContainer.AddSubview(_titleLabel);

            _clipView.WidthAnchor.ConstraintEqualTo(ContentView.WidthAnchor).Active = true;
            _clipView.HeightAnchor.ConstraintEqualTo(ContentView.HeightAnchor).Active = true;

            _background.WidthAnchor.ConstraintEqualTo(_clipView.WidthAnchor).Active = true;
            _background.HeightAnchor.ConstraintEqualTo(_clipView.HeightAnchor).Active = true;

            _textContainer.WidthAnchor.ConstraintEqualTo(_clipView.WidthAnchor).Active = true;
            _textContainer.BottomAnchor.ConstraintEqualTo(_clipView.BottomAnchor).Active = true;

            _numberLabel.LeftAnchor.ConstraintEqualTo(_textContainer.LayoutMarginsGuide.LeftAnchor).Active = true;
            _numberLabel.TopAnchor.ConstraintEqualTo(_textContainer.LayoutMarginsGuide.TopAnchor).Active = true;

            _titleLabel.TopAnchor.ConstraintEqualTo(_numberLabel.BottomAnchor).Active = true;
            _titleLabel.LeftAnchor.ConstraintEqualTo(_textContainer.LayoutMarginsGuide.LeftAnchor).Active = true;
            _titleLabel.BottomAnchor.ConstraintEqualTo(_textContainer.LayoutMarginsGuide.BottomAnchor).Active = true;

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<LessonCell, Lesson>();
                set.Bind(_numberLabel).For(v => v.Text).To(vm => vm.Number).WithConversion<LessonNumberConverter>();
                set.Bind(_titleLabel).For(v => v.Text).To(vm => vm.Title);
                set.Bind(_background).For(v => v.Image).To(vm => vm.Number).WithConversion<LessonImageConverter>();
                set.Apply();
            });
        }
    }
}
