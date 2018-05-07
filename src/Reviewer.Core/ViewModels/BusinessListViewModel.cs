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

        public ICommand RefreshCommand { get; }

        public BusinessListViewModel()
        {
            Businesses = new List<Business>();

            RefreshCommand = new Command(async () => await ExecuteRefreshCommand());

            dataService = DependencyService.Get<IDataService>();
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
