using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

public class Item
{
    public int Id { get; set; }
    public string Album { get; set; }
    public string Artist { get; set; }
    public string Genre { get; set; }
}

public class Library
{
    private const string field_id = "@Id";
    private const string field_album = "@Album";
    private const string field_artist = "@Artist";
    private const string field_genre = "@Genre";
    private const string connection_string = "Filename=database.db";
    private const string table_create = "CREATE TABLE IF NOT EXISTS Items " +
        "(Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
        "Album NVARCHAR(255) NULL, Artist NVARCHAR(255) NULL, Genre NVARCHAR(255) NULL)";
    private const string table_insert = "INSERT INTO Items VALUES (NULL, @Album, @Artist, @Genre)";
    private const string table_update = "UPDATE Items SET Album = @Album, " +
        "Artist = @Artist, Genre = @Genre WHERE Id = @Id";
    private const string table_delete = "DELETE FROM Items WHERE Id = @Id";
    private const string table_select = "SELECT Id, Album, Artist, Genre FROM Items";

    private async Task<Item> Dialog(Item item)
    {
        Thickness margin = new Thickness(5);
        TextBlock id = new TextBlock()
        {
            Text = item.Id.ToString(),
            Margin = margin,
        };
        TextBox album = new TextBox()
        {
            Text = item.Album ?? string.Empty,
            Margin = margin,
            PlaceholderText = "Album"
        };
        TextBox artist = new TextBox()
        {
            Text = item.Artist ?? string.Empty,
            Margin = margin,
            PlaceholderText = "Artist"
        };
        TextBox genre = new TextBox()
        {
            Text = item.Genre ?? string.Empty,
            Margin = margin,
            PlaceholderText = "Genre"
        };
        StackPanel panel = new StackPanel()
        {
            Orientation = Orientation.Vertical
        };
        panel.Children.Add(id);
        panel.Children.Add(album);
        panel.Children.Add(artist);
        panel.Children.Add(genre);
        ContentDialog dialog = new ContentDialog()
        {
            Title = "Database",
            PrimaryButtonText = "Save",
            CloseButtonText = "Cancel",
            Content = panel
        };
        ContentDialogResult result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            item.Album = album.Text;
            item.Artist = artist.Text;
            item.Genre = genre.Text;
            return item;
        }
        return null;
    }

    private async Task<bool> AddItemAsync(Item item)
    {
        bool result = false;
        using (SqliteConnection connection = new SqliteConnection(connection_string))
        {
            await connection.OpenAsync();
            SqliteCommand insert = new SqliteCommand()
            {
                Connection = connection,
                CommandText = table_insert
            };
            insert.Parameters.AddWithValue(field_album, item.Album);
            insert.Parameters.AddWithValue(field_artist, item.Artist);
            insert.Parameters.AddWithValue(field_genre, item.Genre);
            try
            {
                await insert.ExecuteScalarAsync();
                result = true;
            }
            catch (SqliteException)
            {
                result = false;
            }
            connection.Close();
        }
        return result;
    }

    private async Task<bool> EditItemAsync(Item item)
    {
        bool result = false;
        using (SqliteConnection connection = new SqliteConnection(connection_string))
        {
            await connection.OpenAsync();
            SqliteCommand insert = new SqliteCommand()
            {
                Connection = connection,
                CommandText = table_update
            };
            insert.Parameters.AddWithValue(field_id, item.Id);
            insert.Parameters.AddWithValue(field_album, item.Album);
            insert.Parameters.AddWithValue(field_artist, item.Artist);
            insert.Parameters.AddWithValue(field_genre, item.Genre);
            try
            {
                await insert.ExecuteScalarAsync();
                result = true;
            }
            catch (SqliteException)
            {
                result = false;
            }
            connection.Close();
        }
        return result;
    }

    private async Task<bool> DeleteItemAsync(Item item)
    {
        bool result = false;
        using (SqliteConnection connection = new SqliteConnection(connection_string))
        {
            await connection.OpenAsync();
            SqliteCommand delete = new SqliteCommand()
            {
                Connection = connection,
                CommandText = table_delete
            };
            delete.Parameters.AddWithValue(field_id, item.Id);
            try
            {
                await delete.ExecuteNonQueryAsync();
                result = true;
            }
            catch (SqliteException)
            {
                result = false;
            }
            connection.Close();
        }
        return result;
    }

    public async Task<bool> CreateAsync()
    {
        bool result = false;
        using (SqliteConnection connection = new SqliteConnection(connection_string))
        {
            await connection.OpenAsync();
            SqliteCommand create = new SqliteCommand(table_create, connection);
            try
            {
                await create.ExecuteNonQueryAsync();
                result = true;
            }
            catch (SqliteException)
            {
                result = false;
            }
            connection.Close();
        }
        return result;
    }

    public async Task<List<Item>> ListAsync()
    {
        List<Item> results = new List<Item>();
        using (SqliteConnection connection = new SqliteConnection(connection_string))
        {
            await connection.OpenAsync();
            SqliteCommand select = new SqliteCommand(table_select, connection);
            try
            {
                SqliteDataReader query = await select.ExecuteReaderAsync();
                while (query.Read())
                {
                    Item item = new Item()
                    {
                        Id = query.GetInt32(0),
                        Album = query.GetString(1),
                        Artist = query.GetString(2),
                        Genre = query.GetString(3)
                    };
                    results.Add(item);
                }
            }
            catch (SqliteException)
            {
                results = null;
            }
            connection.Close();
        }
        return results;
    }

    public async Task<bool> AddAsync()
    {
        Item item = await Dialog(new Item());
        if (item != null)
        {
            return await AddItemAsync(item);
        }
        return false;
    }

    public async Task<bool> EditAsync(AppBarButton button)
    {
        Item item = await Dialog((Item)button.Tag);
        if (item != null)
        {
            return await EditItemAsync(item);
        }
        return false;
    }

    public async Task<bool> DeleteAsync(AppBarButton button)
    {
        Item item = (Item)button.Tag;
        if (item != null)
        {
            return await DeleteItemAsync(item);
        }
        return false;
    }
}