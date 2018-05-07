using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Reviewer.SharedModels;
using System.Threading.Tasks;
using System.Linq;

namespace Reviewer.Core
{
    public partial class EditReviewPage : ContentPage
    {
        EditReviewViewModel vm;
        bool isNew;

        public EditReviewPage(string businessId, string businessName)
        {
            InitializeComponent();
            vm = new EditReviewViewModel(businessId, businessName);
            BindingContext = vm;
            isNew = true;
        }

        public EditReviewPage(Review review) : base()
        {
            InitializeComponent();

            vm = new EditReviewViewModel(review);
            BindingContext = vm;
            isNew = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            vm.SaveComplete += SaveComplete;
            videoList.SelectedItemChanged += VideoList_SelectedItemChanged;
            photoList.SelectedItemChanged += PhotoList_SelectedItemChanged;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            vm.SaveComplete -= SaveComplete;
            videoList.SelectedItemChanged -= VideoList_SelectedItemChanged;
            photoList.SelectedItemChanged -= PhotoList_SelectedItemChanged;
        }

        async void VideoList_SelectedItemChanged(object sender, EventArgs e)
        {
            if (!(sender is HorizontalList horizontalList))
                return;

            if (horizontalList.SelectedItem is FileImageSource fileIS)
            {
                vm.TakePhotoCommand.Execute(null);
                return;
            }

            if (!(horizontalList.SelectedItem is UriImageSource videoUrl))
                return;

            var foundVideo = vm.Review.Videos.FirstOrDefault(vid => vid.ThumbnailUrl == videoUrl.Uri.AbsoluteUri);

            //if (!(horizontalList.SelectedItem is Video video))
            //return;

            var playerPage = new NavigationPage(new VideoPlayerPage(foundVideo));

            await Navigation.PushModalAsync(playerPage, true);
        }

        async void PhotoList_SelectedItemChanged(object sender, EventArgs e)
        {
            if (!(sender is HorizontalList horizontalList))
                return;

            if (horizontalList.SelectedItem is FileImageSource fileImgSrc)
            {
                vm.TakePhotoCommand.Execute(null);
                return;
            }

            if (!(horizontalList.SelectedItem is UriImageSource photoUrl))
                return;

            var photoPage = new NavigationPage(new PhotoViewerPage(photoUrl.Uri.AbsoluteUri));

            await Navigation.PushModalAsync(photoPage, true);
        }

        async void SaveComplete(object sender, EventArgs args)
        {
            await CloseWindow();
        }

        async void Cancel_Clicked(object sender, EventArgs args)
        {
            await CloseWindow();
        }

        async Task CloseWindow()
        {
            if (isNew)
                await Navigation.PopModalAsync(true);
            else
                await Navigation.PopAsync(true);
        }
    }

}
