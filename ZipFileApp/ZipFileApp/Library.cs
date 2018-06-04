using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

public class ZipItem
{
    public string Name { get; set; }
    public string ActualBytes { get; set; }
    public string CompressedBytes { get; set; }
}

public class Library
{
    public const string extension_all = "*";
    public const string extension_zip = ".zip";
    public const string desc_archive = "Archive";
    public const string desc_file = "File";

    private string _token = string.Empty;
    StorageItemAccessList _access = StorageApplicationPermissions.FutureAccessList;

    public string Name { get; set; }

    private async Task<byte[]> GetByteFromFile(StorageFile storageFile)
    {
        using (IRandomAccessStream stream = await storageFile.OpenReadAsync())
        {
            using (DataReader dataReader = new DataReader(stream))
            {
                byte[] bytes = new byte[stream.Size];
                await dataReader.LoadAsync((uint)stream.Size);
                dataReader.ReadBytes(bytes);
                return bytes;
            }
        }
    }

    private async Task<StorageFile> OpenFileAsync(string extension)
    {
        try
        {
            FileOpenPicker picker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            picker.FileTypeFilter.Add(extension);
            StorageFile open = await picker.PickSingleFileAsync();
            if (open != null)
            {
                return open;
            }
        }
        finally
        {
        }
        return null;
    }

    private async Task<StorageFile> SaveFileAsync(string filename, string extension, string description)
    {
        try
        {
            FileSavePicker picker = new FileSavePicker()
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                DefaultFileExtension = extension,
                SuggestedFileName = filename
            };
            picker.FileTypeChoices.Add(description, new List<string>() { extension });
            StorageFile save = await picker.PickSaveFileAsync();
            if (save != null)
            {
                return save;
            }
        }
        finally
        {
        }
        return null;
    }

    public async Task<List<ZipItem>> List()
    {
        List<ZipItem> results = null;
        StorageFile file = await _access.GetFileAsync(_token);
        if (file != null)
        {
            using (Stream stream = await file.OpenStreamForReadAsync())
            {
                using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    results = new List<ZipItem>();
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.Name != string.Empty)
                        {
                            results.Add(new ZipItem()
                            {
                                Name = entry.Name,
                                CompressedBytes = $"Compressed Size {entry.CompressedLength}",
                                ActualBytes = $"Actual Size {entry.Length}"
                            });
                        }
                    }
                }
            }
        }
        return results;
    }

    public async Task<bool> NewAsync()
    {
        StorageFile file = await SaveFileAsync(desc_archive, extension_zip, desc_archive);
        if (file != null)
        {
            _token = _access.Add(file);
            Name = file.Name;
            using (Stream stream = await file.OpenStreamForWriteAsync())
            {
                new ZipArchive(stream, ZipArchiveMode.Create);
            }
            return true;
        }
        return false;
    }

    public async Task<List<ZipItem>> OpenAsync()
    {
        StorageFile file = await OpenFileAsync(extension_zip);
        if (file != null)
        {
            Name = file.Name;
            _token = _access.Add(file);
        }
        return await List();
    }

    public async Task<List<ZipItem>> AddAsync()
    {
        if (!string.IsNullOrEmpty(_token))
        {
            StorageFile file = await _access.GetFileAsync(_token);
            if (file != null)
            {
                StorageFile source = await OpenFileAsync(extension_all);
                if (source != null)
                {
                    using (Stream stream = await file.OpenStreamForWriteAsync())
                    {
                        using (ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Update))
                        {
                            ZipArchiveEntry existing = zipArchive.GetEntry(source.Name);
                            if (existing != null)
                            {
                                existing.Delete();
                            }
                            ZipArchiveEntry entry = zipArchive.CreateEntry(source.Name);
                            using (Stream entryStream = entry.Open())
                            {
                                byte[] data = await GetByteFromFile(source);
                                entryStream.Write(data, 0, data.Length);
                            }
                        }
                    }
                }
            }
            return await List();
        }
        return null;
    }

    public async void Extract(ZipItem item)
    {
        if (!string.IsNullOrEmpty(_token))
        {
            StorageFile file = await _access.GetFileAsync(_token);
            if (file != null)
            {
                using (Stream stream = await file.OpenStreamForReadAsync())
                {
                    using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
                    {
                        ZipArchiveEntry entry = archive.GetEntry(item.Name);
                        if (entry != null)
                        {
                            StorageFile source = await SaveFileAsync(entry.Name, Path.GetExtension(entry.Name), desc_file);
                            if (source != null)
                            {
                                using (Stream output = await source.OpenStreamForWriteAsync())
                                {
                                    await entry.Open().CopyToAsync(output);
                                    await output.FlushAsync();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public async Task<List<ZipItem>> RemoveAsync(ZipItem item)
    {
        if (!string.IsNullOrEmpty(_token))
        {
            StorageFile file = await _access.GetFileAsync(_token);
            if (file != null)
            {
                using (Stream stream = await file.OpenStreamForWriteAsync())
                {
                    using (ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Update))
                    {
                        ZipArchiveEntry entry = zipArchive.GetEntry(item.Name);
                        if (entry != null)
                        {
                            entry.Delete();
                        }
                    }
                }
                return await List();
            }
        }
        return null;
    }
}