using Microsoft.WindowsAzure.MobileServices;
using ProjectA.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProjectA
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AzureTable : ContentPage
    {
        MobileServiceClient client = AzureManager.AzureManagerInstance.AzureClient;

        public AzureTable()
        {
            InitializeComponent();
        }

        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            List<ProjectAModel> data = await AzureManager.AzureManagerInstance.GetData();

            infoList.ItemsSource = data;
        }

    }
}