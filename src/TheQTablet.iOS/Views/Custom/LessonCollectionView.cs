using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

using CoreGraphics;

using Foundation;

using UIKit;

namespace TheQTablet.iOS.Views.Custom
{

    public class LessonSource : UICollectionViewSource
    {
        public List<string> Lessons { get; private set; }

        public LessonSource()
        {
            Lessons = new List<string>
            {
                "Lesson 1",
                "Lesson 2",
                "Lesson 3",
                "Lesson 4",
                "Lesson 5",
                "Lesson 6",
                "Lesson 7",
                "Lesson 8",
                "Lesson 9",
            };
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (LessonCell)collectionView.DequeueReusableCell(LessonCell.CellId, indexPath);

            cell.Text = Lessons[indexPath.Row];

            cell.Layer.CornerRadius = 20;

            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return Lessons.Count;
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

    public class LessonCell : UICollectionViewCell
    {
        public static readonly NSString CellId = new NSString("LessonCell");

        private UILabel _titleLabel;

        private string _titleText;
        public string Text
        {
            get => _titleText;
            set
            {
                _titleText = value;
                _titleLabel.Text = _titleText;
            }
        }

        [Export("initWithFrame:")]
        LessonCell(RectangleF frame) : base(frame)
        {
            BackgroundColor = UIColor.Yellow;

            _titleLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = _titleText,
            };
            ContentView.AddSubview(_titleLabel);

            _titleLabel.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
            _titleLabel.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
        }
    }
}
