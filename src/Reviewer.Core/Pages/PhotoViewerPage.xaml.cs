using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Reviewer.Core
{
    public partial class PhotoViewerPage : ContentPage
    {
        PhotoViewerViewModel viewModel;
        public PhotoViewerPage(string photoUrl)
        {
            InitializeComponent();

            viewModel = new PhotoViewerViewModel(photoUrl);

            BindingContext = viewModel;
        }

        async void Done_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }
    }
}
