using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;

public class Library
{
    private async Task<Dictionary<string, string>> GetProperties(StorageFile file)
    {
        Dictionary<string, string> results = new Dictionary<string, string>();
        ImageProperties properties = await file.Properties.GetImagePropertiesAsync();
        results.Add("Name", file.Name);
        foreach (PropertyInfo property in properties.GetType().GetProperties())
        {
            results.Add(property.Name, property.GetValue(properties)?.ToString());
        }
        results.Remove("PeopleNames");
        results.Remove("Keywords");
        return results;
    }

    public async Task<Dictionary<string, string>> OpenAsync()
    {
        try
        {
            FileOpenPicker picker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            StorageFile open = await picker.PickSingleFileAsync();
            if (open != null)
            {
                return await GetProperties(open);
            }
        }
        finally
        {

        }
        return null;
    }
}