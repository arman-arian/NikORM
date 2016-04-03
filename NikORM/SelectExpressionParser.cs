using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NikORM
{
    internal sealed class SelectExpressionParser : ExpressionVisitor
    {
        private Type _ownerType;
        private readonly List<string> _properties;

        public SelectExpressionParser()
        {
            _properties = new List<string>();
        }

        public string GetArguments<T, U>(Expression<Func<T, U>> expression)
        {
            _properties.Clear();
            _ownerType = typeof(T);

            if (expression.Body.Type == typeof(T))
            {
                return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(propertyInfo => propertyInfo.Name)
                    .ToList().ToDelimString();
            }

            Visit(expression);
            return _properties.ToDelimString();
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var propertyInfo = node.Member as PropertyInfo;
            if (propertyInfo != null && _ownerType.IsAssignableFrom(propertyInfo.DeclaringType))
            {
                _properties.Add(propertyInfo.Name);
            }
            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(SqlFunc))
            {
                var args = node.Arguments.Select(a => ((MemberExpression)a).Member.Name).ToDelimString();
                var methodName = node.Method.Name;
                var method = string.Format("{0}({1})", methodName, args);
                _properties.Add(method);
                return node;
            }
            return base.VisitMethodCall(node);
        }
    }
}
