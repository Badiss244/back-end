using MediatR;

namespace Domain.Queries
{
    
    public class GetGenericQuery<T, TKey> : IRequest<T> where T : class
    {
        public TKey Id { get; set; }

        public GetGenericQuery(TKey id)
        {
            Id = id;
        }
    }
}
