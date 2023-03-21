using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VirtualLibrary.Books.Dtos;
using VirtualLibrary.Books.Services;
using VirtualLibrary.Books.Services.Event;

namespace VirtualLibrary.Books.Services.Books;

public class BookService : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly EventingBasicConsumer _consumer;
    private readonly IAuthorsService _authors;

    public BookService(IAuthorsService authorService)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare("books-queue", false, false, false, null);
        _consumer = new EventingBasicConsumer(_channel);
        _authors = authorService;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _consumer.Received += async (model, content) =>
        {
            var body = content.Body.ToArray();
            var book_id = Encoding.UTF8.GetString(body);

            Console.WriteLine("\n\n" + book_id);

            var book = await GetBook(book_id, cancellationToken);
            var author = await _authors.GetAuthorsByIdAsync(book.authorId);

            Console.WriteLine("\n\n" + book);
            Console.WriteLine("\n\n" + author);

            var book_information = new BookInformationDataTransferObject
            {
                book_information = book,
                author_information = author,
            };
           
            sendBookInformation(book_information);
        };

        _channel.BasicConsume("books-queue", true, _consumer);
        return Task.CompletedTask;
    }

    public async Task<BooksDataTransferObject> GetBook(string book_id, CancellationToken token)
    {
        string json = System.IO.File.ReadAllText("books.json");
        List<BooksDataTransferObject> books = JsonConvert.DeserializeObject<List<BooksDataTransferObject>>(json);

        BooksDataTransferObject book = new BooksDataTransferObject();

        if (books != null)
        {
            if (books.Any())
                book = books.SingleOrDefault(x => x.isbn == book_id);
        }

        await Task.Delay(2000, token);

        return book;
    }

    private void sendBookInformation(BookInformationDataTransferObject book_information)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672
        };

        using (var connection = factory.CreateConnection())
        {
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("book-results", false, false, false, null);
                var json = JsonConvert.SerializeObject(book_information);
                var body = Encoding.UTF8.GetBytes(json);
                Console.WriteLine("\n\n" + json);
                channel.BasicPublish(string.Empty, "book-results", null, body);
            }
        }
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"Books Service in Progress {DateTimeOffset.Now}");
            await Task.Delay(1000, stoppingToken);
        }
    }
}