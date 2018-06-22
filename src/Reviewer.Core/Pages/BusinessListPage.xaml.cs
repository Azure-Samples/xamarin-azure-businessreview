using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Reviewer.SharedModels;

namespace Reviewer.Core
{
    public partial class BusinessListPage : ContentPage
    {
        BusinessListViewModel vm;

        public BusinessListPage()
        {
            InitializeComponent();

            vm = new BusinessListViewModel();
            BindingContext = vm;


            allBusList.ItemTapped += (sender, args) => allBusList.SelectedItem = null;

            vm.Title = "Businesses";
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            addNewReview.Clicked += HandleAddNewClicked;
            allBusList.ItemSelected += listItemSelected;
            vm.RefreshCommand.Execute(null);

            if (!vm.IsLoggedIn)
                await vm.CheckLoginStatus();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            addNewReview.Clicked -= HandleAddNewClicked;
            allBusList.ItemSelected -= listItemSelected;
        }

        async void HandleAddNewClicked(object sender, EventArgs eventArgs)
        {
            var editPage = new NavigationPage(new EditBusinessPage());

            await Navigation.PushModalAsync(editPage);
        }

        protected async void listItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var business = args.SelectedItem as Business;

            if (business == null)
                return;

            await Navigation.PushAsync(new BusinessReviewsPage(business));
        }
    }
}
