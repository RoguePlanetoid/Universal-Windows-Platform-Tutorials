using System;
using System.Collections.ObjectModel;
using System.Linq;

public class Item
{
    public Guid Id { get; set; }
    public string Value { get; set; }
}

public class Library
{
    public ObservableCollection<Item> Items { get; set; } = new ObservableCollection<Item>();

    public void Add(string value)
    {
        Items.Add(new Item
        {
            Id = Guid.NewGuid(),
            Value = value
        });
    }

    public void Remove(Guid id)
    {
        Item result = Items.FirstOrDefault(item => item.Id == id);
        if (result != null)
        {
            Items.Remove(result);
        }
    }
}