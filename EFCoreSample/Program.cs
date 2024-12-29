// See https://aka.ms/new-console-template for more information

using EFCoreSample;
using Microsoft.EntityFrameworkCore;

var person = new Person("Test1");
person.Addresses.Add(new EMail("g.gg@g.com"));

var ctx = new Context();
ctx.Database.EnsureCreated();
Console.WriteLine(ctx.Database.GenerateCreateScript());

ctx.Persons.Add(person);
ctx.SaveChanges();

var persons = ctx.Persons.Include(p => p.Addresses).ToList();
Console.WriteLine($"{persons.Count} persons found");