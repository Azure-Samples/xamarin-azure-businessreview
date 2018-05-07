using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Reviewer.SharedModels;

namespace Reviewer.Core
{
    public partial class ReviewDetailPage : ContentPage
    {
        ReviewDetailViewModel vm;
        public ReviewDetailPage(Review review, Business business)
        {
            InitializeComponent();

            vm = new ReviewDetailViewModel(review, business);
            vm.Title = "Review Details";

            BindingContext = vm;
        }

        public ReviewDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            videoList.SelectedItemChanged += VideoList_SelectedItemChanged;
            photoList.SelectedItemChanged += PhotoList_SelectedItemChanged;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            videoList.SelectedItemChanged -= VideoList_SelectedItemChanged;
            photoList.SelectedItemChanged -= PhotoList_SelectedItemChanged;
        }

        async void VideoList_SelectedItemChanged(object sender, EventArgs e)
        {
            if (!(sender is HorizontalList horizontalList))
                return;

            if (!(horizontalList.SelectedItem is Video video))
                return;

            var videoPlayer = new NavigationPage(new VideoPlayerPage(video));

            await Navigation.PushModalAsync(videoPlayer);
        }

        async void PhotoList_SelectedItemChanged(object sender, EventArgs e)
        {
            if (!(sender is HorizontalList horizontalList))
                return;

            if (!(horizontalList.SelectedItem is string photoUrl))
                return;

            var photoViewer = new NavigationPage(new PhotoViewerPage(photoUrl));

            await Navigation.PushModalAsync(photoViewer);
        }

        async void Handle_EditClicked(object sender, EventArgs e)
        {
            var editPage = new EditReviewPage(vm.Review);

            await Navigation.PushAsync(editPage);
        }
    }
}
