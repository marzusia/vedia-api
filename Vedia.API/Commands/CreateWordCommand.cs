using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Vedia.API.Models;
using Vedia.API.Services;

namespace Vedia.API.Commands
{
    public class CreateWordCommand : IRequest<Word>
    {
        public Word Word { get; set; }
    }
    
    public class CreateWordCommandHandler : IRequestHandler<CreateWordCommand, Word>
    {
        private readonly WordService _wordService;
        public CreateWordCommandHandler(WordService wordService) => _wordService = wordService;
        
        public async Task<Word> Handle(CreateWordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _wordService.Words.InsertOneAsync(request.Word, cancellationToken: cancellationToken);
                return request.Word;
            }
            catch
            {
                return null;
            }
        }
    }
}