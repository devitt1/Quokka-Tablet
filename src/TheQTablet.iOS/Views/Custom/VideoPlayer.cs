using AVFoundation;
using AVKit;
using Foundation;
using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class VideoPlayer: UIView
    {
        private AVAsset _asset;
        private AVPlayer _player;
        private AVPlayerItem _playerItem;
        private AVPlayerViewController _playerController;

        public VideoPlayer(string filename)
        {
            _asset = AVAsset.FromUrl(NSUrl.FromFilename(filename));
            _playerItem = new AVPlayerItem(_asset);

            _player = new AVPlayer(_playerItem);
            _playerController = new AVPlayerViewController
            {
                Player = _player,
                // Picture in picture does not get destroyed when view does
                AllowsPictureInPicturePlayback = false,
            };
            _playerController.View.TranslatesAutoresizingMaskIntoConstraints = false;
            AddSubview(_playerController.View);

            _playerController.View.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            _playerController.View.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            _playerController.View.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            _playerController.View.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;

            _playerController.View.HeightAnchor.ConstraintEqualTo(_playerController.View.WidthAnchor, _asset.NaturalSize.Height / _asset.NaturalSize.Width).Active = true;
        }
    }
}
