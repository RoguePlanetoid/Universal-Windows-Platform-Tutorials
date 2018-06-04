using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

public class Item
{
    public string Id { get; set; }
    public string Address { get; set; }
    public string DisplayName { get; set; }
}

public class Library
{
    private const string app_title = "Contacts App";

    private async Task<ContactStore> Store()
    {
        return await ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);
    }

    private InputScope Scope(InputScopeNameValue value)
    {
        InputScope scope = new InputScope();
        InputScopeName name = new InputScopeName()
        {
            NameValue = value
        };
        scope.Names.Add(name);
        return scope;
    }

    private async Task<Contact> Dialog(Contact contact)
    {
        Thickness margin = new Thickness(5);
        TextBox firstname = new TextBox()
        {
            Text = contact.FirstName,
            Margin = margin,
            PlaceholderText = "First Name"
        };
        TextBox lastname = new TextBox()
        {
            Text = contact.LastName,
            Margin = margin,
            PlaceholderText = "Last Name"
        };
        TextBox email = new TextBox()
        {
            Text = contact?.Emails?.FirstOrDefault()?.Address ?? string.Empty,
            Margin = margin,
            PlaceholderText = "Email Address",
            InputScope = Scope(InputScopeNameValue.EmailSmtpAddress)
        };
        StackPanel panel = new StackPanel()
        {
            Orientation = Orientation.Vertical
        };
        panel.Children.Add(firstname);
        panel.Children.Add(lastname);
        panel.Children.Add(email);
        ContentDialog dialog = new ContentDialog()
        {
            Title = "Contact",
            PrimaryButtonText = "Save",
            CloseButtonText = "Cancel",
            Content = panel
        };
        ContentDialogResult result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            contact.FirstName = firstname.Text;
            contact.LastName = lastname.Text;
            contact.Emails.Add(new ContactEmail() { Address = email.Text });
            return contact;
        }
        return null;
    }

    private async Task<ContactList> GetAsync()
    {
        ContactList result = null;
        ContactStore store = await Store();
        if (store != null)
        {
            IReadOnlyList<ContactList> list = await store.FindContactListsAsync();
            if (list.Count == 0)
            {
                result = await store.CreateContactListAsync(app_title);
            }
            else
            {
                result = list.FirstOrDefault(s => s.DisplayName == app_title);
            }
        }
        return result;
    }

    private async Task<IReadOnlyList<Contact>> ListContactsAsync()
    {
        ContactStore store = await Store();
        if (store != null)
        {
            ContactList list = await GetAsync();
            if (list != null)
            {
                ContactReader reader = list.GetContactReader();
                if (reader != null)
                {
                    ContactBatch batch = await reader.ReadBatchAsync();
                    if (batch != null)
                    {
                        return batch.Contacts;
                    }
                }
            }
        }
        return null;
    }

    private async Task<Contact> GetContactAsync(string id)
    {
        ContactList list = await GetAsync();
        if (list != null)
        {
            return await list.GetContactAsync(id);
        }
        return null;
    }

    public async Task<List<Item>> ListAsync()
    {
        List<Item> results = new List<Item>();
        IReadOnlyList<Contact> contacts = await ListContactsAsync();
        foreach (Contact contact in contacts)
        {
            results.Add(new Item
            {
                Id = contact.Id,
                DisplayName = contact.DisplayName,
                Address = contact?.Emails?.FirstOrDefault()?.Address
            });
        }
        return results;
    }

    public async Task<bool> AddAsync(AppBarButton button)
    {
        Contact contact = await Dialog(new Contact());
        if (contact != null)
        {
            ContactList list = await GetAsync();
            if (list != null)
            {
                await list.SaveContactAsync(contact);
                return true;
            }
        }
        return false;
    }

    public async Task<bool> EditAsync(AppBarButton button)
    {
        Item item = (Item)button.Tag;
        Contact contact = await GetContactAsync(item.Id);
        if (contact != null)
        {
            contact = await Dialog(contact);
            if (contact != null)
            {
                ContactList list = await GetAsync();
                if (list != null)
                {
                    await list.SaveContactAsync(contact);
                    return true;
                }
            }
        }
        return false;
    }

    public async Task<bool> DeleteAsync(AppBarButton button)
    {
        Item item = (Item)button.Tag;
        Contact contact = await GetContactAsync(item.Id);
        if (contact != null)
        {
            ContactList list = await GetAsync();
            if (list != null)
            {
                await list.DeleteContactAsync(contact);
                return true;
            }
        }
        return false;
    }
}