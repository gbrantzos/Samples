using Microsoft.EntityFrameworkCore;

namespace EFCoreSample;

public class Context : DbContext
{
    public string DbPath { get; set; }

    public Context()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "persons.db");
    }

    public DbSet<Person> Persons => Set<Person>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");
}