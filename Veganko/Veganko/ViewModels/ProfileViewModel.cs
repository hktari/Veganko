using Autofac;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Veganko.Common.Models.Products;
using Veganko.Models.User;
using Veganko.Other;
using Veganko.Services;
using Veganko.Services.Comments;
using Veganko.ViewModels.Products.ModRequests.Partial;
using Veganko.ViewModels.Profile;
using Veganko.Views;
using Xamarin.Forms;
using Veganko.Services.Products.ProductModRequests;
using Veganko.Services.Http;
using Veganko.Extensions;
using System.Linq;
using Veganko.Views.Product.ModRequests;
using Veganko.ViewModels.Products.ModRequests;

namespace Veganko.ViewModels
{
    public class ProfileViewModel : BaseUserProfileViewModel
    {
        public const string ProfileChangedMsg = "ProfileChangedMsg";

        public Command ReloadProductModReqsCommand => new Command(
            async () => await ReloadProductModReqs());

        public Command HelpCommand => new Command(
            async () => await App.Navigation.PushModalAsync(new NavigationPage(new HelpPage())));

        private bool isDirty;
        public bool IsDirty
        {
            get => isDirty;
            set => SetProperty(ref isDirty, value);
        }

        private ICommentsService commentDataStore;
        private IProductService productDataStore;
        private readonly IProductModRequestService productModReqService;
        private readonly IAccountService accountService;
        private readonly IUserService userService;
        private Command focusEditorCommand;

        public ProfileViewModel()
            : base(true)
        {
            accountService = App.IoC.Resolve<IAccountService>();
            userService = App.IoC.Resolve<IUserService>();
            Title = "Profile";
            User = userService.CurrentUser;
            CanDeleteProducts = User.Role != UserRole.Member;
            HandleNewData();
            MessagingCenter.Subscribe<BackgroundImageViewModel, string>(this, BackgroundImageViewModel.SaveMsg, OnBackgroundImageChanged);
            MessagingCenter.Subscribe<SelectAvatarViewModel, string>(this, SelectAvatarViewModel.SaveMsg, OnAvatarImageChanged);
            commentDataStore = App.IoC.Resolve<ICommentsService>();
            productDataStore = App.IoC.Resolve<IProductService>();
            productModReqService = App.IoC.Resolve<IProductModRequestService>();
        }

        private bool anyProdModRequestsExist;
        public bool AnyProdModRequestsExist
        {
            get => anyProdModRequestsExist;
            set => SetProperty(ref anyProdModRequestsExist, value);
        }

        public bool CanDeleteProducts { get; }

        public Command StartEditingDescriptionCommand => new Command(
            () =>
            {
                IsEditingDescription = true;
                FocusEditorCommand?.Execute(null);
                ShouldShowDescriptionPlaceholder = false;
            });

        public Command StopEditingDescriptionCommand => new Command(
            () =>
            {
                IsEditingDescription = false;
                UpdateDescPlaceholderVisibility();
                UpdateIsDirty();
            });

        public Command FocusEditorCommand
        {
            get => focusEditorCommand;
            set => SetProperty(ref focusEditorCommand, value);
        }

        public Command<ProductModRequestViewModel> ProductSelectedCommand => new Command<ProductModRequestViewModel>(
            async selection =>
            {
                await App.Navigation.PushAsync(
                    new ProductModRequestDetailPage(
                        new ProductModRequestDetailViewModel(selection)))
                .ConfigureAwait(false);
            });

        public Command<ProductModRequestViewModel> DeleteProdModReqCommand => new Command<ProductModRequestViewModel>(
            async pmr =>
            {
                try
                {
                    string result = await App.CurrentPage.DisplayActionSheet("Prosim potrdi da želiš izbrisati ta produkt.", "Prekliči", "Izbriši");
                    if (result == "Izbriši")
                    {
                        IsBusy = true;
                        ProductModRequestDTO model = pmr.GetModel();
                        await productModReqService.DeleteAsync(model);
                        ProductModRequests?.Remove(pmr);
                    }
                }
                catch (ServiceException ex)
                {
                    await App.CurrentPage.Err("Brisanje ni uspelo", ex);
                }
                finally
                {
                    IsBusy = false;
                }
            });

        private ObservableCollection<ProductModRequestViewModel> productModRequests;
        public ObservableCollection<ProductModRequestViewModel> ProductModRequests
        {
            get => productModRequests;
            set => SetProperty(ref productModRequests, value);
        }

        public override async void OnPageAppearing()
        {
            await ReloadProductModReqs();
        }

        private async Task ReloadProductModReqs()
        {
            try
            {
                IsBusy = true;
                // TODO : -1 == get all
                var page = await productModReqService.AllAsync(pageSize: 100, userId: User.Id);
                ProductModRequests = new ObservableCollection<ProductModRequestViewModel>(
                    page.Items
                    .OrderByDescending(p => p.Timestamp)
                    .Select(dto => new ProductModRequestViewModel(dto)));
                AnyProdModRequestsExist = page.Items.Count() > 0;
            }
            catch (ServiceException ex)
            {
                await App.CurrentPage.Err("Nalaganje produktov ni uspelo.", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task SaveProfile()
        {
            UserPublicInfo updatedUser = new UserPublicInfo(userService.CurrentUser)
            {
                Label = UserLabel,
                Description = UserDescription,
                AvatarId = Images.GetProfileAvatarId(AvatarImage),
                ProfileBackgroundId = Images.GetProfileBackgroundImageId(BackgroundImage),
            };

            User = await userService.Edit(updatedUser);
            HandleNewData();
            UpdateIsDirty();
            MessagingCenter.Send(this, ProfileChangedMsg, updatedUser);
        }

        protected override void HandleNewData()
        {
            base.HandleNewData();
            UpdateDescPlaceholderVisibility();
        }

        private void UpdateDescPlaceholderVisibility()
        {
            ShouldShowDescriptionPlaceholder = string.IsNullOrWhiteSpace(UserDescription);
        }

        private void OnBackgroundImageChanged(BackgroundImageViewModel arg1, string newBackgroundId)
        {
            BackgroundImage = Images.GetProfileBackgroundImageById(newBackgroundId);
            UpdateIsDirty();
        }

        private void UpdateIsDirty()
        {
            UserPublicInfo user = userService.CurrentUser;
            IsDirty =
                Images.GetProfileBackgroundImageId(BackgroundImage) != user.ProfileBackgroundId ||
                Images.GetProfileAvatarId(AvatarImage) != user.AvatarId ||
                UserDescription != user.Description ||
                UserLabel != user.Label;
        }

        private void OnAvatarImageChanged(SelectAvatarViewModel sender, string newAvatarId)
        {
            AvatarImage = Images.GetProfileAvatarById(newAvatarId);
            UpdateIsDirty();
        }
    }
}
