using System;
using System.Windows.Input;
using Reviewer.SharedModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Reviewer.Services;
namespace Reviewer.Core
{
    public class EditBusinessViewModel : BaseViewModel
    {
        Business business;
        public Business Business { get => business; set => SetProperty(ref business, value); }

        bool isNotNew;
        public bool IsNotNew { get => isNotNew; set => SetProperty(ref isNotNew, value); }

        bool isNew;
        public bool IsNew
        {
            get => isNew;
            set
            {
                SetProperty(ref isNew, value);
                IsNotNew = !IsNew;
            }
        }

        public ICommand SaveCommand { get; }

        public event EventHandler SaveComplete;

        public EditBusinessViewModel()
        {
            IsNew = true;
            Business = new Business() { Id = Guid.NewGuid().ToString() };
            Business.Address = new Address() { Id = Guid.NewGuid().ToString() };

            SaveCommand = new Command(async () => await ExecuteSaveCommand());
        }

        public EditBusinessViewModel(Business theBusiness)
        {
            Business = theBusiness;
            IsNew = false;

            SaveCommand = new Command(async () => await ExecuteSaveCommand());
        }

        async Task ExecuteSaveCommand()
        {
            if (IsBusy)
                return;

            try
            {
                var dataService = DependencyService.Get<IDataService>();

                if (IsNew)
                    await dataService.InsertBusiness(Business);
                else
                    await dataService.UpdateBusiness(Business);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR: {ex}");
            }
            finally
            {
                IsBusy = false;
            }

            SaveComplete?.Invoke(this, new EventArgs());
        }
    }
}
