using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ProjectA.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Geolocator;

namespace ProjectA
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomVision : ContentPage
    {
        double max = 0;

        public CustomVision()
        {
            InitializeComponent();
        }

        private async void loadCamera(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });

            await postLocationAsync();
            await MakePredictionRequest(file);
        }

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", "8338490e8ac440088290e1edadb0cbfb");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/ddfef3d4-1eec-4c0f-aa25-74332d7339e0/image?iterationId=d8f806c6-866e-482e-9a36-3b87622bfaaa";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    EvaluationModel responseModel = JsonConvert.DeserializeObject<EvaluationModel>(responseString);

                    max = responseModel.Predictions.Max(m => m.Probability);

                    TagLabel.Text = (max >= 0.5) ? "Dog" : "Not dog";

                }

                //Get rid of file once we have finished using it
                file.Dispose();
            }
        }

        async Task postLocationAsync()
        {
            String value;
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));

            if (max >= 0.5)
            {
                value = "Dog";
            }
            else
            {
                value = "Not dog";
            }

            ProjectAModel model = new ProjectAModel()
            {
                Longitude = (float)position.Longitude,
                Latitude = (float)position.Latitude,
                Value = value
            };

            await AzureManager.AzureManagerInstance.PostInformation(model);
        }

    }
}