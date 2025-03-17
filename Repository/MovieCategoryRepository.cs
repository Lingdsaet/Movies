﻿
using System.Linq;
using System.Threading.Tasks;
using Movie.Models;

namespace Movie.Repository
{
    public class MovieCategoryRepository : IMovieCategoryRepository<MovieCategory>
    {
        private readonly movieDB _context;
        public MovieCategoryRepository(movieDB context)
        {
            _context = context;
        }
        public async Task AddAsync(MovieCategory entity)
        {
            await _context.MovieCategories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteByMovieIdAsync(int movieId)
        {
            var categories = _context.MovieCategories.Where(mc => mc.MovieId == movieId);
            _context.MovieCategories.RemoveRange(categories);
            await _context.SaveChangesAsync();
        }
    }
}
