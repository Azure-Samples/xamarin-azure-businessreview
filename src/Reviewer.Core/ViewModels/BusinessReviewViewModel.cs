using System;
using Reviewer.SharedModels;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Reviewer.Core
{
    public class BusinessReviewViewModel : BaseViewModel
    {
        IAPIService apiService;

        Business business;
        public Business Business { get => business; set => SetProperty(ref business, value); }

        List<Review> reviews;
        public List<Review> Reviews { get => reviews; set => SetProperty(ref reviews, value); }

        bool isLoggedIn = false;
        public bool IsLoggedIn { get => isLoggedIn; set => SetProperty(ref isLoggedIn, value); }

        public ICommand RefreshCommand { get; }

        public BusinessReviewViewModel(Business business)
        {
            apiService = DependencyService.Get<IAPIService>();
            Business = business;

            Reviews = new List<Review>();

            RefreshCommand = new Command(async () => await ExecuteRefreshCommand());

            Title = Business.Name;

            Task.Run(async () => await CheckLoginStatus());
        }

        public async Task CheckLoginStatus()
        {
            var idService = DependencyService.Get<IIdentityService>();

            var authResult = await idService.GetCachedSignInToken();

            IsLoggedIn = authResult?.User != null;
        }

        async Task ExecuteRefreshCommand()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                Reviews = await apiService.GetReviewsForBusiness(Business.Id);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
