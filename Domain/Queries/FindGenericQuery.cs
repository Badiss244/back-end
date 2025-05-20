using MediatR;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Domain.Queries
{
    public class FindGenericQuery<T> : IRequest<IEnumerable<T>> where T : class
    {
        public Expression<Func<T, bool>> Predicate { get; }

        public FindGenericQuery(Expression<Func<T, bool>> predicate)
        {
            Predicate = predicate;
        }
    }
}
