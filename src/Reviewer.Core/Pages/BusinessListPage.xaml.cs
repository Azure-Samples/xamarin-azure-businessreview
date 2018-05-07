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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            allBusList.ItemSelected += listItemSelected;
            vm.RefreshCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            allBusList.ItemSelected -= listItemSelected;
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
