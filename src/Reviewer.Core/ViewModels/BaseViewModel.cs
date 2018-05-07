using System;
using Reviewer.SharedModels;

namespace Reviewer.Core
{
    public class BaseViewModel : ObservableObject
    {
        string title;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                SetProperty(ref isBusy, value);
                IsNotBusy = !IsBusy;
            }
        }

        bool isNotBusy = true;
        public bool IsNotBusy
        {
            get => isNotBusy;
            private set => SetProperty(ref isNotBusy, value);
        }

    }
}
