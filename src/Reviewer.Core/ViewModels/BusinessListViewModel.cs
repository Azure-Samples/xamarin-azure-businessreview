using System;
using System.Windows.Input;
using System.Collections.Generic;
using Reviewer.SharedModels;
using System.Threading.Tasks;
using Reviewer.Services;
using Xamarin.Forms;

namespace Reviewer.Core
{
    public class BusinessListViewModel : BaseViewModel
    {
        IDataService dataService;

        List<Business> businesses;
        public List<Business> Businesses { get => businesses; set => SetProperty(ref businesses, value); }

        bool isLoggedIn;
        public bool IsLoggedIn { get => isLoggedIn; set => SetProperty(ref isLoggedIn, value); }

        public ICommand RefreshCommand { get; }

        public BusinessListViewModel()
        {
            Businesses = new List<Business>();

            RefreshCommand = new Command(async () => await ExecuteRefreshCommand());

            dataService = DependencyService.Get<IDataService>();

            IsLoggedIn = false;
            Task.Run(async () => CheckLoginStatus());
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
                Businesses = await dataService.GetBusinesses();
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}
