// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace TheQTablet.iOS
{
	[Register ("StoryContainerView")]
	partial class StoryContainerView
	{
		[Outlet]
		UIKit.UIButton nextButton { get; set; }

		[Outlet]
		UIKit.UILabel storyLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (storyLabel != null) {
				storyLabel.Dispose ();
				storyLabel = null;
			}

			if (nextButton != null) {
				nextButton.Dispose ();
				nextButton = null;
			}
		}
	}
}
