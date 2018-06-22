using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Reviewer.Core
{
    public partial class EditBusinessPage : ContentPage
    {
        bool isNew;
        EditBusinessViewModel viewModel;

        public EditBusinessPage()
        {
            InitializeComponent();

            viewModel = new EditBusinessViewModel();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.SaveComplete += SaveComplete;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            viewModel.SaveComplete -= SaveComplete;
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
            if (viewModel.IsNew)
                await Navigation.PopModalAsync(true);
            else
                await Navigation.PopAsync(true);
        }
    }
}
