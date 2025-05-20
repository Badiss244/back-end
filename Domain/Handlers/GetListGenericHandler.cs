using Domain.Interface;
using Domain.Queries;
using MediatR;

namespace Domain.Handlers
{
    public class GetListGenericHandler<T, TKey> : IRequestHandler<GetListGenericQuery<T>, IEnumerable<T>> where T : class
    {
        private readonly IGenericRepository<T, TKey> _repository;

        public GetListGenericHandler(IGenericRepository<T, TKey> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<T>> Handle(GetListGenericQuery<T> request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync();
        }
    }
}
