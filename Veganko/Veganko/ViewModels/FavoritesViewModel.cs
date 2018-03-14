using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Veganko.ViewModels
{
    public class TodoItem
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }

    public class FavoritesViewModel : BaseViewModel
    {
        private ObservableCollection<TodoItem> items;
        public ObservableCollection<TodoItem> Items
        {
            get { return items; }
            set { SetProperty(ref items, value); }
        }

        public FavoritesViewModel()
        {
            
        }
    }
}
