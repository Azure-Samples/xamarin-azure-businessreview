using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Reviewer.SharedModels;

namespace Reviewer.Core
{
    public partial class AccountPage : ContentPage
    {

        AccountViewModel vm;
        public AccountPage()
        {
            InitializeComponent();

            vm = new AccountViewModel();

            BindingContext = vm;

            authorReviewList.ItemTapped += (sender, e) => authorReviewList.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            vm.UnsuccessfulSignIn += UnSuccessfulSignIn;
            authorReviewList.ItemSelected += listItemSelected;

            vm.RefreshCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            vm.UnsuccessfulSignIn -= UnSuccessfulSignIn;
            authorReviewList.ItemSelected -= listItemSelected;
        }

        protected async void listItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var review = args.SelectedItem as Review;

            if (review == null)
                return;

            await Navigation.PushAsync(new EditReviewPage(review));
        }

        async void UnSuccessfulSignIn(object sender, EventArgs e) => await DisplayAlert("Error", "Couldn't log in, try again!", "OK");
    }
}
