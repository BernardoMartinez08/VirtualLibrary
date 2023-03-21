using VirtualLibrary.Books.Dtos;

namespace VirtualLibrary.Books.Services.Event;

public interface IAuthorsService
{
    Task<IEnumerable<AuthorsDataTransferObject>> GetAuthorsAsync();
    Task<AuthorsDataTransferObject> GetAuthorsByIdAsync(int author_id);
}