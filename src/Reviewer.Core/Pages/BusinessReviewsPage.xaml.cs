using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Reviewer.SharedModels;

namespace Reviewer.Core
{
    public partial class BusinessReviewsPage : ContentPage
    {
        BusinessReviewViewModel vm;

        public BusinessReviewsPage(Business business)
        {
            InitializeComponent();

            vm = new BusinessReviewViewModel(business);

            BindingContext = vm;

            reviewList.ItemTapped += (sender, args) => reviewList.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            reviewList.ItemSelected += ReviewList_ItemSelected;
            addNewReview.Clicked += AddNewReview_Clicked;

            vm.RefreshCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            reviewList.ItemSelected -= ReviewList_ItemSelected;
            addNewReview.Clicked -= AddNewReview_Clicked;
        }

        async void AddNewReview_Clicked(object sender, EventArgs e)
        {
            var editPage = new EditReviewPage(vm.Business.Id, vm.Business.Name);

            await Navigation.PushModalAsync(new NavigationPage(editPage));
        }

        async void ReviewList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var review = e.SelectedItem as Review;
            if (review == null)
                return;

            await Navigation.PushAsync(new ReviewDetailPage(review, vm.Business));
        }
    }
}
