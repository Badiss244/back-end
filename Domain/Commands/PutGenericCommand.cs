using MediatR;

namespace Domain.Commands
{
    public class PutGenericCommand<T> : IRequest<T> where T : class
    {
        public T Entity { get; set; }

        public PutGenericCommand(T entity)
        {
            Entity = entity;
        }
    }
}
