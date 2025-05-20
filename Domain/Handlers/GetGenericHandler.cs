using Domain.Interface;
using Domain.Queries;
using MediatR;

namespace Domain.Handlers
{
    public class GetGenericHandler<T, TKey> : IRequestHandler<GetGenericQuery<T, TKey>, T> where T : class
    {
        private readonly IGenericRepository<T, TKey> _repository;

        public GetGenericHandler(IGenericRepository<T, TKey> repository)
        {
            _repository = repository;
        }

        public async Task<T> Handle(GetGenericQuery<T, TKey> request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            return entity; 
        }
    }
}
