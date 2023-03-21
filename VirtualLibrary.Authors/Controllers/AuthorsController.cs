using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VirtualLibrary.Authors.Dtos;

namespace VirtualLibrary.Authors.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorsController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetAuthors()
        {
            string json = System.IO.File.ReadAllText("authors.json");
            List<AuthorsDataTransferObject> authors = JsonConvert.DeserializeObject<List<AuthorsDataTransferObject>>(json);
            return Ok(authors);
        }

        [HttpGet("{author_id}")]
        public IActionResult GetAuthorById(int author_id)
        {
            string json = System.IO.File.ReadAllText("authors.json");
            List<AuthorsDataTransferObject> authors = JsonConvert.DeserializeObject<List<AuthorsDataTransferObject>>(json);

            var author = authors.SingleOrDefault(x => x.id == author_id);
            
            return author is null
            ?
                NotFound($"No se encontro el autor con el id: {author_id}")
            :
                Ok(author);
        }
    }
}
