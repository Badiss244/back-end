using MediatR;
using System.Collections.Generic;

namespace Domain.Queries
{
   
    public class GetListGenericQuery<T> : IRequest<IEnumerable<T>> where T : class
    {
    }
}
