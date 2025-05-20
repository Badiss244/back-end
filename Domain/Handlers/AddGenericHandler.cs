using Domain.Commands;
using Domain.Interface;
using MediatR;

namespace Domain.Handlers
{
   
    public class AddGenericHandler<T, TKey> : IRequestHandler<AddGenericCommand<T>, T> where T : class
    {
        private readonly IGenericRepository<T, TKey> _repository;

        public AddGenericHandler(IGenericRepository<T, TKey> repository)
        {
            _repository = repository;
        }

        public async Task<T> Handle(AddGenericCommand<T> request, CancellationToken cancellationToken)
        {
            await _repository.AddAsync(request.Entity);
            return request.Entity;
        }
    }
}
