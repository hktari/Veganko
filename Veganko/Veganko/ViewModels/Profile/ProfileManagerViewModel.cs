using System.Collections.Generic;
using Xamarin.Forms;
using Veganko.Views.Management;

namespace Veganko.ViewModels.Profile
{
    public class ProfileManagerViewModel : ProfileViewModel
    {
        public class ListItem
        {
            public string Id { get; set; }
            public string Title { get; set; }
        }

        public List<ListItem> Items { get; } = new List<ListItem>
        {
            new ListItem{ Id = "prod-mods-reqs", Title = "Nepotrjeni produkti"},
            new ListItem{ Id = "user-management", Title = "Upravljanje uporabnikov"}
        };

        public Command<ListItem> ItemSelectedCommand => new Command<ListItem>(
            async selected => 
            {
                switch (selected.Id)
                {
                    case "prod-mods-reqs":
                        await App.Navigation.PushAsync(new ProductRequestsPage());
                        break;
                    case "user-management":
                        await App.Navigation.PushAsync(new ManageUsersPage());
                        break;
                    default:
                        break;
                }
            });
    }
}
