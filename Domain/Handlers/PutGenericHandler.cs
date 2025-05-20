using Domain.Commands;
using Domain.Interface;
using MediatR;

namespace Domain.Handlers
{
    public class PutGenericHandler<T, TKey> : IRequestHandler<PutGenericCommand<T>, T> where T : class
    {
        private readonly IGenericRepository<T, TKey> _repository;

        public PutGenericHandler(IGenericRepository<T, TKey> repository)
        {
            _repository = repository;
        }

        public async Task<T> Handle(PutGenericCommand<T> request, CancellationToken cancellationToken)
        {
            await _repository.UpdateAsync(request.Entity);
            return request.Entity;
        }
    }
}
