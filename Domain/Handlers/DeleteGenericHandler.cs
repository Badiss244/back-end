using Domain.Commands;
using Domain.Interface;
using MediatR;

namespace Domain.Handlers
{
    public class DeleteGenericHandler<T, TKey> : IRequestHandler<DeleteGenericCommand<T, TKey>, bool> where T : class
    {
        private readonly IGenericRepository<T, TKey> _repository;

        public DeleteGenericHandler(IGenericRepository<T, TKey> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteGenericCommand<T, TKey> request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
                return false;

            await _repository.DeleteByIdAsync(request.Id);
            return true;
        }
    }
}
