using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Queries.SortByItemQueries
{

    public class GetAllSortByItemQuery : IRequest<IEnumerable<SortByItem>>
    {
        public class GetAllSortByItemQueryHandler : IRequestHandler<GetAllSortByItemQuery, IEnumerable<SortByItem>>
        {
            private readonly IApplicationDbContext _context;
            private IApiClientGoogleDrive _client;

            public GetAllSortByItemQueryHandler(IApplicationDbContext context, IApiClientGoogleDrive client)
            {
                _context = context;
                _client = client;
            }

            public async Task<IEnumerable<SortByItem>> Handle(GetAllSortByItemQuery request, CancellationToken cancellationToken)
            {
                var sortByItemList = await _context.SortByItem.ToListAsync(cancellationToken);

                return sortByItemList.AsReadOnly();
            }
        }
    }
}
