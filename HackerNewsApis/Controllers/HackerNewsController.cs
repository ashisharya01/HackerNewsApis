using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HackerNewsApis.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class HackerNewsController : ControllerBase
    {
        private readonly HackerNewsService _service;

        public HackerNewsController(HackerNewsService service)
        {
            _service = service;
        }

        [HttpGet("newest")]
        public async Task<IActionResult> GetNewestStories()
        {
            var stories = await _service.GetNewestStoriesAsync();
            return Ok(stories);
        }
    }
}