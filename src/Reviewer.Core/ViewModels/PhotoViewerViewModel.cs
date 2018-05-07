using System;
namespace Reviewer.Core
{
    public class PhotoViewerViewModel : BaseViewModel
    {
        string photoUrl;
        public string PhotoUrl { get => photoUrl; set => SetProperty(ref photoUrl, value); }

        public PhotoViewerViewModel(string url)
        {
            PhotoUrl = url;
        }
    }
}
