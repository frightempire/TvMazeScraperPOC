using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TvMazeScraper.API.ViewModels;
using TvMazeScraper.Infrastructure.Persistence;

namespace TvMazeScraper.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TvShowsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly SqLiteTvMazeContext _context;

        public TvShowsController(IMapper mapper, SqLiteTvMazeContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TvShowModel>>> GetTvShowsWithCast([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            await _context.Database.EnsureCreatedAsync();

            var tvShows = await _context.Shows
                .OrderBy(t => t.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(t => t.Cast)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<TvShowModel>>(tvShows));
        }
    }
}