using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vedia.API.Commands;
using Vedia.API.Models;
using Vedia.API.Queries;

namespace Vedia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordController : ControllerBase
    {
        private readonly IMediator _mediator;
        public WordController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Word>>> GetAll([FromQuery] int offset, [FromQuery] int limit)
        {
            var words = await _mediator
                .Send(new GetAllWordsQuery {Offset = offset, Limit = limit})
                .ConfigureAwait(false);
            return Ok(words);
        }

        [Authorize(Roles="Language")]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Word word)
        {
            var created = await _mediator
                .Send(new CreateWordCommand {Word = word})
                .ConfigureAwait(false);
            if (created is not null) return NoContent();
            return BadRequest();
        }
    }
}