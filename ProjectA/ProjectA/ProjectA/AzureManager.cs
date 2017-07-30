using Microsoft.WindowsAzure.MobileServices;
using ProjectA.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectA
{
    class AzureManager
    {
        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<ProjectAModel> ProjectATable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://projectab.azurewebsites.net");
            this.ProjectATable = this.client.GetTable<ProjectAModel>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task<List<ProjectAModel>> GetData()
        {
            return await this.ProjectATable.ToListAsync();
        }

        public async Task PostInformation(ProjectAModel projectModel)
        {
            await this.ProjectATable.InsertAsync(projectModel);
        }
    }
}