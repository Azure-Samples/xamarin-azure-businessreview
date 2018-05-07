using System;
using Reviewer.SharedModels;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using Plugin.Media;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Data.Common;

namespace Reviewer.Core
{
    public class EditReviewViewModel : BaseViewModel
    {
        Review review;
        public Review Review { get => review; set => SetProperty(ref review, value); }

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

        bool isNotNew;
        public bool IsNotNew { get => isNotNew; set => SetProperty(ref isNotNew, value); }

        List<ImageSource> photos;
        public List<ImageSource> Photos { get => photos; set => SetProperty(ref photos, value); }

        List<ImageSource> videos;
        public List<ImageSource> Videos { get => videos; set => SetProperty(ref videos, value); }

        public ICommand SaveCommand { get; }

        public event EventHandler SaveComplete;

        public ICommand TakePhotoCommand { get; }

        IIdentityService idService;

        public EditReviewViewModel(Review theReview)
        {
            Photos = new List<ImageSource>();

            Review = theReview;

            SaveCommand = new Command(async () => await ExecuteSaveCommand());
            TakePhotoCommand = new Command(async () => await ExecuteTakePhotoCommand());

            Title = "A Review";

            IsNew = false;

            idService = DependencyService.Get<IIdentityService>();

            Review.Author = idService.DisplayName;

            var thePhotos = new List<ImageSource>();

            if (Review.Photos != null)
            {
                foreach (var photo in Review.Photos)
                {
                    thePhotos.Add(ImageSource.FromUri(new Uri(photo)));
                }

                Photos = thePhotos;
            }
            else
            {
                Review.Photos = new List<string>();
            }

            Photos.Insert(0, ImageSource.FromFile("ic_camera_enhance_black"));

            var theVideos = new List<ImageSource>();
            if (Review.Videos != null)
            {
                foreach (var video in Review.Videos)
                {
                    theVideos.Add(ImageSource.FromUri(new Uri(video.ThumbnailUrl)));
                }

                Videos = theVideos;
            }
            else
            {
                Review.Videos = new List<Video>();
            }

            Videos.Insert(0, ImageSource.FromFile("ic_movie_black"));
        }

        public EditReviewViewModel(string businessId, string businessName) :
            this(new Review { Id = Guid.NewGuid().ToString(), BusinessId = businessId, BusinessName = businessName })
        {
            IsNew = true;
        }

        async Task ExecuteSaveCommand()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var authResult = await idService.GetCachedSignInToken();

                var webAPI = DependencyService.Get<IAPIService>();

                if (IsNew)
                {
                    Review.AuthorId = authResult.UniqueId;
                    await webAPI.InsertReview(Review, authResult.AccessToken);
                }
                else
                    await webAPI.UpdateReview(Review, authResult.AccessToken);
            }
            finally
            {
                IsBusy = false;
            }

            SaveComplete?.Invoke(this, new EventArgs());
        }

        async Task ExecuteTakePhotoCommand()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var actions = new List<string>();

                if (CrossMedia.Current.IsTakePhotoSupported && CrossMedia.Current.IsCameraAvailable)
                    actions.Add("Take Photo");

                if (CrossMedia.Current.IsTakeVideoSupported && CrossMedia.Current.IsCameraAvailable)
                    actions.Add("Take Video");

                if (CrossMedia.Current.IsPickPhotoSupported)
                    actions.Add("Pick Photo");


                var result = await Application.Current.MainPage.DisplayActionSheet("Take or Pick Photo", "Cancel", null, actions.ToArray());


                bool isVideo = false;
                MediaFile mediaFile = null;
                if (result == "Take Photo")
                {
                    var options = new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.Medium,
                        DefaultCamera = CameraDevice.Rear
                    };

                    mediaFile = await CrossMedia.Current.TakePhotoAsync(options);
                }
                else if (result == "Take Video")
                {
                    var options = new StoreVideoOptions
                    {
                        CompressionQuality = 50,
                        CustomPhotoSize = 50,
                        DefaultCamera = CameraDevice.Rear,
                        Quality = VideoQuality.Medium
                    };

                    mediaFile = await CrossMedia.Current.TakeVideoAsync(options);
                    isVideo = true;
                }
                else if (result == "Pick Photo")
                {
                    mediaFile = await CrossMedia.Current.PickPhotoAsync();
                }

                if (mediaFile == null)
                    return;

                if (isVideo)
                    await UploadVideo(mediaFile);
                else
                    await UploadPhoto(mediaFile);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task UploadPhoto(MediaFile mediaFile)
        {
            UploadProgress progressUpdater = new UploadProgress();

            using (var mediaStream = mediaFile.GetStream())
            {
                var storageService = DependencyService.Get<IStorageService>();

                var blobAddress = await storageService.UploadBlob(mediaStream, false, Review.Id, progressUpdater);

                if (blobAddress == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Upload Error", "There was an error uploading your photo, please try again.", "OK");
                    return;
                }

                var thePhotos = new List<ImageSource>();
                thePhotos.AddRange(Photos);
                thePhotos.Add(ImageSource.FromUri(blobAddress));

                Photos = thePhotos;

                Review.Photos.Add(blobAddress.AbsoluteUri);

                if (!IsNew)
                {
                    var functionApi = DependencyService.Get<IAPIService>();
                    await functionApi.WritePhotoInfoToQueue(Review.Id, blobAddress.AbsoluteUri);
                }
            }
        }

        async Task UploadVideo(MediaFile mediaFile)
        {
            using (var mediaStream = await ConvertToMP4(mediaFile))
            {
                UploadProgress progressUpdater = new UploadProgress();

                var storageService = DependencyService.Get<IStorageService>();
                var blobAddress = await storageService.UploadBlob(mediaStream, true, Review.Id, progressUpdater);

                await Application.Current.MainPage.DisplayAlert("Video Upload", "We're processing your video! It'll be visible here as soon as we're done!", "OK");
            }
        }

        async Task<Stream> ConvertToMP4(MediaFile file)
        {
            if (file.Path.EndsWith("mp4", StringComparison.OrdinalIgnoreCase))
                return file.GetStream();

            var videoConverter = DependencyService.Get<IVideoConversion>();
            if (videoConverter == null)
                return null;

            return await videoConverter.ConvertToMP4(file.Path);
        }
    }
}
