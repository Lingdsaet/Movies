﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Movie.Models;

namespace Movie.Repository
{
    public class MovieActorRepository : IMovieActorRepository<MovieActor>
    {

        private readonly movieDB _context;
        public MovieActorRepository(movieDB context)
        {
            _context = context;
        }
        public async Task<int> CountAsync()
        {
            return await _context.Movies.CountAsync();
        }
        public async Task AddAsync(MovieActor entity)
        {
            await _context.MovieActors.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteByMovieIdAsync(int movieId)
        {
            var actors = _context.MovieActors.Where(mc => mc.MovieId == movieId);
            _context.MovieActors.RemoveRange(actors);
            await _context.SaveChangesAsync();
        }
    }
}