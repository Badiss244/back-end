using Domain.Interface;
using Domain.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Handlers
{
    
    public class FindGenericHandler<T> : IRequestHandler<FindGenericQuery<T>, IEnumerable<T>> where T : class
    {
        private readonly IGenericRepository<T, Guid> _repository;

        public FindGenericHandler(IGenericRepository<T, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<T>> Handle(FindGenericQuery<T> request, CancellationToken cancellationToken)
        {
            
            return await _repository.FindAsync(request.Predicate);
        }
    }
}
