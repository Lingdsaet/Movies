﻿using Microsoft.EntityFrameworkCore;
using Movie.Models;
using Movie.RequestDTO;
using Movie.ResponseDTO;

namespace Movie.Repository
{

    public class MovieRepository : IMovieRepository
    {
        private readonly movieDB _context;

        public MovieRepository(movieDB context)
        {
            _context = context;
        }

        public async Task<RequestMovieDTO> AddAsync(RequestMovieDTO movieDTO)
        {
            var movie = new Models.Movies
            {
                Title = movieDTO.Title,
                Description = movieDTO.Description,
                Rating = movieDTO.Rating,
                PosterUrl = movieDTO.PosterUrl,
                AvatarUrl = movieDTO.AvatarUrl,
                LinkFilmUrl = movieDTO.LinkFilmUrl,
                DirectorId = movieDTO.DirectorId,
                IsHot = movieDTO.IsHot,
                YearReleased = movieDTO.YearReleased,
                Status = 1
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return movieDTO;
        }

        public async Task<RequestMovieDTO?> GetByIdAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return null;

            return new RequestMovieDTO
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                Description = movie.Description,
                Rating = movie.Rating,
                PosterUrl = movie.PosterUrl,
                AvatarUrl = movie.AvatarUrl,
                LinkFilmUrl = movie.LinkFilmUrl,
                DirectorId = movie.DirectorId,
                Director = movie.Director?.NameDir,
                IsHot = movie.IsHot,
                YearReleased = movie.YearReleased

            };
        }

        // Sửa 
        public async Task<RequestMovieDTO?> UpdateAsync(RequestMovieDTO movieDTO)
        {
            var movie = await _context.Movies.FindAsync(movieDTO.MovieId);
            if (movie == null) return null;

            movie.Title = movieDTO.Title;
            movie.Description = movieDTO.Description;
            movie.Rating = movieDTO.Rating;
            movie.PosterUrl = movieDTO.PosterUrl;
            movie.AvatarUrl = movieDTO.AvatarUrl;
            movie.LinkFilmUrl = movieDTO.LinkFilmUrl;
            movie.DirectorId = movieDTO.DirectorId;
            movie.IsHot = movieDTO.IsHot;
            movie.YearReleased = movie.YearReleased;

            await _context.SaveChangesAsync();
            return movieDTO;
        }

        public async Task<IEnumerable<RequestMovieDTO>> GetMovieAsync(int pageNumber, int pageSize, string sortBy, string search, int? categoryID)
        {
            var query = _context.Movies
                .Include(m => m.Director)
                .Include(m => m.MovieActors).ThenInclude(ma => ma.Actors)
                .Include(m => m.MovieCategories).ThenInclude(mc => mc.Categories)
                .Where(m => m.Status == 1);

            //  Filtering
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m => m.Title.Contains(search));
            }

            if (categoryID.HasValue)
            {
                query = query.Where(m => m.MovieCategories.Any(mc => mc.CategoriesId == categoryID.Value));
            }

            //  Sorting
            query = sortBy switch
            {
                "Title" => query.OrderBy(m => m.Title),
                "Rating" => query.OrderByDescending(m => m.Rating),
                _ => query.OrderBy(m => m.Title)
            };

            var Movie = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Movie.Select(m => new RequestMovieDTO
            {
                MovieId = m.MovieId,
                Title = m.Title,
                Description = m.Description,
                Rating = m.Rating,
                PosterUrl = m.PosterUrl,
                AvatarUrl = m.AvatarUrl,
                LinkFilmUrl = m.LinkFilmUrl,
                DirectorId = m.DirectorId,
                Director = m.Director?.NameDir,
                Actors = m.MovieActors.Select(ma => new RequestActorDTO
                {
                    ActorsId = ma.Actors.ActorsId,
                    NameAct = ma.Actors.NameAct
                }).ToList(),
                Categories = m.MovieCategories.Select(mc => new RequestCategoryDTO
                {
                    CategoriesId = mc.Categories.CategoriesId,
                    CategoryName = mc.Categories.CategoryName
                }).ToList()
            }).ToList();
        }
        //Xoá mềm 
        public async Task<RequestMovieDTO?> SoftDeleteAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                movie.Status = 0;
                await _context.SaveChangesAsync();
            }
            return null;
        }
        //lich su xoa
        public async Task<IEnumerable<RequestMovieDTO>> GetDeleteAsync()
        {
            var deletedMovie = await _context.Movies
                .Include(m => m.Director)
                .Include(m => m.MovieActors).ThenInclude(ma => ma.Actors)
                .Include(m => m.MovieCategories).ThenInclude(mc => mc.Categories)
                .Where(m => m.Status == 0)
                .Select(m => new RequestMovieDTO
                {
                    MovieId = m.MovieId,
                    Title = m.Title,
                    Description = m.Description,
                    Rating = m.Rating,
                    PosterUrl = m.PosterUrl,
                    AvatarUrl = m.AvatarUrl,
                    LinkFilmUrl = m.LinkFilmUrl,
                    DirectorId = m.DirectorId,
                    Director = m.Director != null ? m.Director.NameDir : null,
                    Actors = m.MovieActors.Select(ma => new RequestActorDTO
                    {
                        ActorsId = ma.Actors.ActorsId,
                        NameAct = ma.Actors.NameAct
                    }).ToList(),
                    Categories = m.MovieCategories.Select(mc => new RequestCategoryDTO
                    {
                        CategoriesId = mc.Categories.CategoriesId,
                        CategoryName = mc.Categories.CategoryName
                    }).ToList()
                }).ToListAsync();

            return deletedMovie;
        }

        public async Task DeletedMovieAsync(int id)
        {
            var Movie = _context.Movies.Where(m => m.Status == 0);
            _context.Movies.RemoveRange(Movie);
            await _context.SaveChangesAsync();

        }

        public async Task<RequestMovieDTO> GetMovieByIdAsync(int id)
        {

            var movie = await _context.Movies
                .Include(m => m.Director)
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actors)
                .Include(m => m.MovieCategories)
                    .ThenInclude(mc => mc.Categories)
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null) return null!;

            var movieDTO = new RequestMovieDTO
            {
                Title = movie.Title,
                LinkFilmUrl = movie.LinkFilmUrl,
                YearReleased = movie.YearReleased,
                Nation = movie.Nation,
                Categories = movie.MovieCategories
                    .Select(mc => new RequestCategoryDTO
                    {
                        CategoryName = mc.Categories.CategoryName
                    }).ToList(),
                Description = movie.Description,
                Actors = movie.MovieActors.Select(ma => new RequestActorDTO
                {
                    ActorsId = ma.ActorsId,
                    NameAct = ma.Actors.NameAct
                }).ToList(),
                Director = movie.Director!.NameDir
            };

            return movieDTO;
        }
    }
}