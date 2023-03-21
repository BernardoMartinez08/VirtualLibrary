using Microsoft.AspNetCore.Mvc;
using VirtualLibrary.Books.Services.Books;
using VirtualLibrary.Books.Services.Event;

[assembly: ApiController]
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddHttpClient<IAuthorsService, AuthorsService>();
builder.Services.AddHostedService<BookService>();

var app = builder.Build();
app.MapControllers();

app.Run();