// See https://aka.ms/new-console-template for more information

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using DDD.Domain.Core;
using DDD.Domain.Customers;
using DDD.Infrastructure;
using DDD.Persistence;
using DDD.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var customer = new Customer
{
    Code = "3241",
    FullName = "Test Customer - Δοκιμαστικός Πελάτης"
};
Console.WriteLine(customer.ID);

var serializationOptions = new JsonSerializerOptions()
{
    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.GreekandCoptic),
    WriteIndented = true
};
serializationOptions.Converters.Add(new EntityIDJsonConverter());

var rawJson = JsonSerializer.Serialize(customer, serializationOptions);
Console.WriteLine(rawJson);

var connectionString =
    "Server = sqlserver.gbworks.lan; Database = Customers; User Id=sa;Password=Devel1!; TrustServerCertificate=True; Application Name=DDD";
var options = new DbContextOptionsBuilder<Context>()
    .UseSqlServer(connectionString)
    .LogTo(Console.WriteLine)
    .EnableSensitiveDataLogging()
    .Options;
var context = new Context(options);
var schema = context.Database.GenerateCreateScript();
// await context.Database.EnsureDeletedAsync();
// await context.Database.EnsureCreatedAsync();

var customers = await context
    .Customer
    .Take(20)
    .AsNoTracking()
    .ToListAsync();

Console.WriteLine($"Fetched first {customers.Count} customers");

ICustomerRepository repository = new CustomerRepository(context);
IUnitOfWork uow = new UnitOfWork(context);

var existing = await repository.GetByID(new CustomerID(196));
existing.AddAddress("test 1");
Console.WriteLine(JsonSerializer.Serialize(existing, serializationOptions));

// existing.FullName = "Simple Customer - v3";
// await uow.SaveChanges();

context.Add(customer);
await context.SaveChangesAsync();
Console.ReadKey();