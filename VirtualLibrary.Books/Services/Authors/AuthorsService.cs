using Microsoft.Extensions.Options;
using VirtualLibrary.Books.Dtos;
using Newtonsoft.Json;

namespace VirtualLibrary.Books.Services.Event;

public class AuthorsService : IAuthorsService
{
    public readonly HttpClient _httpClient;

    public AuthorsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<AuthorsDataTransferObject>> GetAuthorsAsync()
    {
        var baseUrl = $"http://localhost:5151/authors";
        var result = await _httpClient.GetStringAsync(baseUrl);
        return JsonConvert.DeserializeObject<IEnumerable<AuthorsDataTransferObject>>(result);
    }

    public async Task<AuthorsDataTransferObject> GetAuthorsByIdAsync(int author_id)
    {
        var baseUrl = $"http://localhost:5151/authors";
        var result = await _httpClient.GetStringAsync($"{baseUrl}/{author_id}");
        return JsonConvert.DeserializeObject<AuthorsDataTransferObject>(result);
    }
}
