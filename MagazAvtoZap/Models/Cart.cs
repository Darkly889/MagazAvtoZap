using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MagazAvtoZap.Models
{
    public class Cart : INotifyPropertyChanged
    {
        private List<CartItem> _items = new List<CartItem>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public List<CartItem> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public void AddItem(CartItem item)
        {
            var existingItem = _items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                _items.Add(item);
            }
            OnPropertyChanged(nameof(Items));
        }

        public void RemoveItem(CartItem item)
        {
            _items.Remove(item);
            OnPropertyChanged(nameof(Items));
        }

        public decimal GetTotalPrice()
        {
            return _items.Sum(item => item.Price * item.Quantity);
        }

        public void Clear()
        {
            _items.Clear();
            OnPropertyChanged(nameof(Items));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
