using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Veganko.ViewModels;
using Xamarin.Forms;
using Veganko.Services;
using Autofac.Core;
using Autofac;
using Veganko.Models.User;

namespace Veganko.Views.Product.DTS
{
    public class CommentDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EditableCommentTemplate { get; set; }

        public DataTemplate DefaultCommentTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            UserPublicInfo user = App.IoC.Resolve<IUserService>().CurrentUser;

            CommentViewModel commentVM = item as CommentViewModel ?? throw new ArgumentException(nameof(item));

            if (commentVM.UserId == user.Id || user.IsManager())
            {
                return EditableCommentTemplate;
            }
            else
            {
                return DefaultCommentTemplate;
            }
        }
    }
}
