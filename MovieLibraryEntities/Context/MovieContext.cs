using FakerDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MovieLibraryEntities.Models;

namespace MovieLibraryEntities.Context;

public class MovieContext : DbContext
{
    private readonly ILogger<MovieContext> _logger;
    public DbSet<Genre> Genres { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Occupation> Occupations { get; set; }
    public DbSet<UserDetail> UserDetails { get; set; }
    public DbSet<UserMovie> UserMovies { get; set; }
    public DbSet<User> Users { get; set; }

    public MovieContext()
    {
        var factory = LoggerFactory.Create(b => b.AddConsole());
        _logger = factory.CreateLogger<MovieContext>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        optionsBuilder
            .LogTo(action => _logger.LogInformation(action), LogLevel.Information)
            //.EnableSensitiveDataLogging()
            .UseSqlServer(configuration.GetConnectionString("MovieContext"), builder => builder.EnableRetryOnFailure()
            );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // This can be a way to generate some 'fake' data for the new UserDetails table
        // Note that it must be commented out if you're running more migrations because the 'Faker' class
        // randomly generates new data which will cause more migrations to be generated every time.

        //var userDetails = new List<UserDetail>();

        //for (var i = 1; i <= 943; i++)
        //{
        //    userDetails.Add(new UserDetail
        //    {
        //        Id = i,
        //        FirstName = Faker.Name.FirstName(),
        //        LastName = Faker.Name.LastName(),
        //        StreetAddress = Faker.Address.StreetAddress(),
        //        City = Faker.Address.City(),
        //        State = Faker.Address.StateAbbr(),
        //        UserId = i
        //    });
        //}

        //modelBuilder.Entity<UserDetail>().HasData(userDetails);
    }

    // You can then call this SeedData method from your application's startup code.
    // You might call this method in the Configure method of your Startup class
    public void SeedData()
    {
        // Check if there are any rows in the UserDetail table
        if (!UserDetails.Any())
        {
            var userDetails = new List<UserDetail>();

            foreach (var user in Users)
            {
                userDetails.Add(new UserDetail
                {
                    FirstName = Faker.Name.FirstName(),
                    LastName = Faker.Name.LastName(),
                    StreetAddress = Faker.Address.StreetAddress(),
                    City = Faker.Address.City(),
                    State = Faker.Address.StateAbbr(),
                    UserId = (int) user.Id
                });

            }
            // Save changes to the database
            SaveChanges();
        }
    }

}
