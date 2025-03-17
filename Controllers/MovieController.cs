﻿

using Microsoft.AspNetCore.Mvc;
using Movie.Models;
using Movie.Repository;
using Movie.RequestDTO;

namespace Movie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;
        public MovieController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        //  Lấy danh sách phim + phân trang+ lọc + sắp xếp
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestMovieDTO>>> GetMovie(
            int pageNumber = 1,
            int pageSize = 10,
            int? categoryID = null,
            string sortBy = "Title",
            string search = ""
            )
        {
            var Movie = await _movieRepository.GetMovieAsync(pageNumber, pageSize, sortBy, search, categoryID);
            return Ok(Movie);
        }


        //  Lấy thông tin phim theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestMovieDTO>> GetMovie(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound("Movie not found");
            }
            return Ok(movie);
        }
        //  Thêm phim
        [HttpPost]
        public async Task<ActionResult<RequestMovieDTO>> AddMovie([FromBody] RequestMovieDTO request)
        {
            if (request == null)
            {
                return BadRequest("Invalid movie data.");
            }

            var movie = await _movieRepository.AddAsync(request);
            return CreatedAtAction(nameof(GetMovie), new { id = movie.MovieId }, movie);
        }

        // Sửa phim
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] RequestMovieDTO request)
        {
            if (request == null || id != request.MovieId)
            {
                return BadRequest("Invalid data");
            }

            var movie = await _movieRepository.UpdateAsync(request);
            if (movie == null)
            {
                return NotFound("Movie not found");
            }

            return NoContent();
        }

        // Xoá mềm (chuyển status từ 1 -> 0)
        [HttpDelete("de/{id}")]
        public async Task<IActionResult> SoftDeleteMovie(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound("Movie not found");
            }

            await _movieRepository.SoftDeleteAsync(id);
            return NoContent();
        }

        //Danh sách xoá
        [HttpGet("getDeleted")]
        public async Task<ActionResult<IEnumerable<RequestMovieDTO>>> GetDeletedMovie()
        {
            var deletedMovie = await _movieRepository.GetDeleteAsync();

            if (deletedMovie == null || !deletedMovie.Any())
            {
                return NotFound(new { message = "No deleted Movie found!" });
            }

            return Ok(deletedMovie);
        }

        //  Xoá vĩnh viễn các phim có Status = 0
        [HttpDelete("deleted")]
        public async Task<IActionResult> DeletedMovie(int id)
        {
            await _movieRepository.DeletedMovieAsync(id);
            return NoContent();
        }
    }
}