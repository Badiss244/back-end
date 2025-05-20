using MediatR;

namespace Domain.Commands
{
    public class DeleteGenericCommand<T, TKey> : IRequest<bool> where T : class
    {
        public TKey Id { get; set; }

        public DeleteGenericCommand(TKey id)
        {
            Id = id;
        }
    }
}
