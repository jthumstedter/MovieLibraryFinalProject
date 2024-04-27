using MovieLibraryEntities.Context;
using MovieLibraryEntities.Dao;
using MovieLibraryEntities.Models;
using System;

public class Program
{
    static public void Main(string[] args)
    {
        bool continueLoop = true;
        do
        {
            Console.WriteLine("1) Search Movie");
            Console.WriteLine("2) Add Movie");
            Console.WriteLine("3) List Movies");
            Console.WriteLine("4) Update Movie");
            Console.WriteLine("5) Delete Movie");
            Console.Write("Enter your choice (-1 to exit): ");
            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    SearchMovie();
                    break;
                case "2":
                    AddMovie();
                    break;
                case "3":
                    ListMovie();
                    break;
                case "4":
                    UpdateMovie();
                    break;
                case "5":
                    DeleteMovie();
                    break;
                case "-1":
                    continueLoop = false;
                    break;
            }
        } while (continueLoop);
    }
    private static void SearchMovie()
    {
        using (var repo = new Repository(new MovieContext()))
        {
            Console.Write("What movie do you want to search for?: ");
            var movieList = repo.Search(Console.ReadLine());
            var genreList = repo.GetMovieGenres();
            foreach (var movie in movieList)
            {
                var genreListForThisMovie = genreList.Where(x => x.Movie.Id == movie.Id).Select(g => g.Genre);
                Console.WriteLine(movie.MovieWithGenresString(genreListForThisMovie));
                Console.WriteLine();
            }
        }
    }
    private static void AddMovie()
    {
        var movie = new Movie();
        List<string> titleErrors;
        while (true)
        {
            Console.Write("Enter movie title: ");
            movie.Title = Console.ReadLine();

            if (movie.ValidateTitle(out titleErrors))
            {
                break;
            }
            DisplayErrors(titleErrors);
        }

        DateTime releaseDate = new DateTime();
        while (true)
        {
            Console.Write($"Enter {movie.Title} release date (MM/DD/YYYY): ");
            bool validDate = DateTime.TryParse(Console.ReadLine(), out releaseDate);
            if (validDate)
            {
                break;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid input. Please try again.");
                Console.ResetColor();
            }
        }
        movie.ReleaseDate = releaseDate;
        using (var context = new MovieContext())
        {
            context.Movies.Add(movie);
            context.SaveChanges();
        }
        /*
         * My attempt on implementing a way to add the genres to the movies.
        var addedGenres = new List<MovieGenre>();
        using (var repo = new Repository(new MovieContext()))
        {
            var createdMovie = repo.GetAllMovies().Last();
            var genreList = repo.GetAllGenres();
            var exitLoop = false;
            while (!exitLoop)
            {
                Console.Write("Enter a genre: ");
                var inputGenre = Console.ReadLine();
                var selectedGenre = genreList.FirstOrDefault(x => x.Name == inputGenre);
                if (selectedGenre != null)
                {
                    addedGenres.Add(new MovieGenre()
                    {
                        Movie = createdMovie,
                        Genre = selectedGenre
                    });
                    Console.WriteLine("Do you want another genre (y/n)?: ");
                    var input = Console.ReadLine();
                    exitLoop = input == "N" || input == "n";
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input. Please try again.");
                    Console.ResetColor();
                }
            }
        }
        using (var context = new MovieContext())
        {
            foreach (var genre in addedGenres)
            {
                context.MovieGenres.Add(genre);
            }
            context.SaveChanges();
        }
        */
        Console.WriteLine("Movie added to the database. Press any key to exit.");
        Console.ReadKey();
    }
    private static void ListMovie()
    {
        using (var repo = new Repository(new MovieContext()))
        {
            var movieList = repo.GetAllMovies();
            var genreList = repo.GetMovieGenres();
            foreach (var movie in movieList)
            {
                var genreListForThisMovie = genreList.Where(x => x.Movie.Id == movie.Id).Select(g => g.Genre);
                Console.WriteLine(movie.MovieWithGenresString(genreListForThisMovie));
                Console.WriteLine();
            }
        }
    }
    private static void UpdateMovie()
    {
        throw new NotImplementedException();
    }
    private static void DeleteMovie()
    {
        throw new NotImplementedException();
    }

    private static void DisplayErrors(List<string> errors)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Invalid input. Please try again.");
        foreach (var error in errors)
        {
            Console.WriteLine(error);
        }

        Console.ResetColor();
    }
}
