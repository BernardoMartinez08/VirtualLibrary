using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using VirtualLibrary.Gateway.Dtos;

namespace VirtualLibrary.Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        [HttpGet("{isbn}")]
        public async Task<IActionResult> getBook(string isbn)
        {
            if (string.IsNullOrEmpty(isbn))
                return BadRequest("El parámetro ISBN no puede estar vacío.");

            try
            {
                Guid guid = Guid.NewGuid();
                string file_name = guid.ToString() + ".txt";

                string rutaArchivo = Path.GetFullPath(file_name);
               
                send_book(isbn);
                recibe_book_information(file_name);

                return Ok(rutaArchivo);
            }
            catch (Exception ex)
            {
                return BadRequest($"Algo Salio Mal... {ex}");
            }
        }

        public void send_book(string book_id)
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
                    channel.QueueDeclare("books-queue", false, false, false, null);
                    var body = Encoding.UTF8.GetBytes(book_id);
                    Console.WriteLine("\n\n" + book_id);
                    channel.BasicPublish(string.Empty, "books-queue", null, body);
                }
            }
        }

        public void recibe_book_information(string rutaArchivo)
        {
            IConnection _connection;
            IModel _channel;
            EventingBasicConsumer _consumer;

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("book-results", false, false, false, null);
            _consumer = new EventingBasicConsumer(_channel);

            _consumer.Received += async (object? model, BasicDeliverEventArgs content) =>
            {
                var body = content.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var book = JsonConvert.DeserializeObject<BookInformationDataTransferObject>(json);
                save_book_information(book, rutaArchivo);

            };
            _channel.BasicConsume("book-results", true, _consumer);
        }

        public void save_book_information(BookInformationDataTransferObject book_recibed, string rutaArchivo)
        {
            Console.WriteLine("\n\nRuta: " + rutaArchivo + "\n\nLibro Info: " + book_recibed);
            string json = JsonConvert.SerializeObject(book_recibed);
            System.IO.File.WriteAllText(rutaArchivo, json);
        }
    }
}
