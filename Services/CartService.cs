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
            if (item.IsCustomBuild && item.Pc != null)
            {
                var exists = Items
                    .Where(i => i.IsCustomBuild && i.Pc != null)
                    .Any(i =>
                        i.Pc.Components.Count == item.Pc.Components.Count &&
                        !i.Pc.Components.Except(item.Pc.Components).Any());

                if (exists)
                {
                    return;
                }
            }
            else
            {
                var existingItem = Items.FirstOrDefault(i =>
                    (i.Pc != null && i.Pc.Id == item.Pc?.Id) ||
                    (i.Component != null && i.Component.Id == item.Component?.Id));

                if (existingItem != null)
                {
                    existingItem.Quantity += item.Quantity;
                    CartChanged?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
            item.Id = Items.Count > 0 ? Items.Max(i => i.Id) + 1 : 1;
            Items.Add(item);
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