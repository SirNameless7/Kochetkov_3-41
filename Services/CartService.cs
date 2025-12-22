using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using KPO_Cursovoy.Models;

namespace KPO_Cursovoy.Services
{
    public class CartService
    {
        public ObservableCollection<CartItem> Items { get; } = new();

        public event EventHandler CartChanged;

        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i =>
                (i.Pc != null && i.Pc.Id == item.Pc?.Id) ||
                (i.Component != null && i.Component.Id == item.Component?.Id));

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                item.Id = Items.Count > 0 ? Items.Max(i => i.Id) + 1 : 1;
                Items.Add(item);
            }

            CartChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveItem(CartItem item)
        {
            Items.Remove(item);
            CartChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Clear()
        {
            Items.Clear();
            CartChanged?.Invoke(this, EventArgs.Empty);
        }

        public decimal GetTotalPrice()
        {
            return Items.Sum(i => i.TotalPrice);
        }

        public int GetTotalItemCount()
        {
            return Items.Sum(i => i.Quantity);
        }
    }
}