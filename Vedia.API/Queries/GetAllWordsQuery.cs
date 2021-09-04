using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using Vedia.API.Models;
using Vedia.API.Services;

namespace Vedia.API.Queries
{
    public class GetAllWordsQuery : IRequest<IEnumerable<Word>>
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
    }
    
    public class GetAllWordsQueryHandler : IRequestHandler<GetAllWordsQuery, IEnumerable<Word>>
    {
        private readonly WordService _wordService;
        public GetAllWordsQueryHandler(WordService wordService) => _wordService = wordService;
        
        public async Task<IEnumerable<Word>> Handle(GetAllWordsQuery request, CancellationToken cancellationToken)
        {
            return await _wordService.Words
                .Find(_ => true)
                .SortBy(f => f.Headword)
                .Skip(request.Offset)
                .Limit(request.Limit)
                .ToListAsync(cancellationToken);
        }
    }
}