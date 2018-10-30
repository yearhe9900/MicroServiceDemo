using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroServiceDemo.Core.Common.Helpers
{
    public static class DapperFilterHelper
    {
        public static PredicateGroup NewFilter(GroupOperator groupOperator = GroupOperator.And)
        {
            var where = new PredicateGroup() { Operator = groupOperator, Predicates = new List<IPredicate>() };
            return where;
        }

        public static void Where(this PredicateGroup _this, IPredicate item)
        {
            _this.Predicates.Add(item);
        }
    }
}
