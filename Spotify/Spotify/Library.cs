using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

[DataContract]
public class Image
{
    [DataMember(Name = "url")]
    public string Url { get; set; }
}

[DataContract]
public class ExternalUrls
{
    [DataMember(Name = "spotify")]
    public string Spotify { get; set; }
}

[DataContract]
public class Item
{
    [DataMember(Name = "external_urls")]
    public ExternalUrls External { get; set; }
    [DataMember(Name = "images")]
    public List<Image> Images { get; set; }
    [DataMember(Name = "name")]
    public string Name { get; set; }
}

[DataContract]
public class Albums
{
    [DataMember(Name = "items")]
    public List<Item> Items { get; set; }
}

[DataContract]
public class Browse
{
    [DataMember(Name = "albums")]
    public Albums Albums { get; set; }
}

public class Library
{
    private const string client_id = "ClientId";
    private const string client_secret = "ClientSecret";
    private const string token_uri = "https://accounts.spotify.com/api/token";
    private const string request_uri = "https://api.spotify.com/v1/browse/new-releases";
    private const string request_param = "?limit=10";
    private const string grant_type = "grant_type";
    private const string client_credentials = "client_credentials";
    private const string token_regex = ".*\"access_token\":\"(.*?)\".*";
    private const string auth_basic = "Basic";
    private const string auth_bearer = "Bearer";

    private HttpClient _client = new HttpClient();
    private HttpResponseMessage _response = new HttpResponseMessage();
    private Browse _browse = new Browse();
    private string _token = null;

    private void Auth(string scheme, string value)
    {
        _client.DefaultRequestHeaders.Authorization =
        new HttpCredentialsHeaderValue(scheme, value);
    }

    private async Task<string> Token()
    {
        if (_token == null)
        {
            Auth(auth_basic,
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{client_id}:{client_secret}")));
            _response = await _client.PostAsync(new Uri(token_uri),
                new HttpFormUrlEncodedContent(new Dictionary<string, string>() { { grant_type, client_credentials } }));
            _token = System.Net.WebUtility.UrlEncode(
            Regex.Match(await _response.Content.ReadAsStringAsync(),
            token_regex, RegexOptions.IgnoreCase).Groups[1].Value);
        }
        return _token;
    }

    public async void Init(ListView display)
    {
        Auth(auth_bearer,
            await Token());
        _response = await _client.GetAsync(new Uri(request_uri + request_param));
        byte[] json = Encoding.Unicode.GetBytes(await _response.Content.ReadAsStringAsync());
        using (MemoryStream stream = new MemoryStream(json))
        {
            _browse = (Browse)new DataContractJsonSerializer(typeof(Browse)).ReadObject(stream);
        }
        display.ItemsSource = _browse?.Albums?.Items;
    }
}