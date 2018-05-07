using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Reviewer.SharedModels;

namespace Reviewer.Core
{
    public partial class VideoPlayerPage : ContentPage
    {
        VideoPlayerViewModel vm;

        public VideoPlayerPage(Video video)
        {
            InitializeComponent();

            vm = new VideoPlayerViewModel(video);

            BindingContext = vm;
        }

        async void Done_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }
    }
}
