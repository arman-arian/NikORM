using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NikORM
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new  TestContext();
            var y = ctx.SelectFrom<Account>(a => a);

            ctx.SelectFrom<Account>(distinct: true);

            ctx.SelectFrom<Account>(1, a => a.Id).Load();

            var df = ctx.SelectFrom<Account>(a => a.Balance).Load(10);

            ctx.SelectFrom<Account>(x => new {x.Balance, x.Id})
                .Where(a => a.Balance == 1000)
                .Orderby(a => a.Balance)
                .Load();

            ctx.SelectFrom<Account>(a => SqlFunc.Max(a.Id)).Load();

            ctx.SelectFrom<Account>(a => a.Balance)
                .Join<Customer>(a => a.Id, c => c.AccountId)
                .SelectFrom<Customer>(a => a.Portion)
                .Load();

            //ctx.SelectFrom<Account>(x => new { x.Balance, x.Id }).Max(x => x.Balance);

        }
    }

    public class Account
    {
        public int Id { get; set; }

        public decimal Balance { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }

        public int Portion { get; set; }

        public int AccountId { get; set; }
    }

    public class TestContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; } 
    }






    public class OrderByResult<T>
    {
        public List<T> Load(int count = -1)
        {
            throw new NotImplementedException();
        }


    }

    public class WhereResult<T>
    {
        public List<T> Load(int count = -1)
        {
            throw new NotImplementedException();
        }

        public OrderByResult<T> Orderby(Expression<Func<T, object>> expression)
        {
            throw new NotImplementedException();
        }

        public OrderByResult<T> OrderbyDescending(Expression<Func<T, object>> expression)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectFromResult<T>
    {
        private readonly string _query;

        public SelectFromResult(string query)
        {
            _query = query;
        }

        public WhereResult<T> Where(Expression<Func<T, bool>> exp)
        {
            throw new NotImplementedException();
        }

        public List<T> Load(int count = -1)
        {
            if (count != -1)
            {
                var query = string.Format("SELECT TOP {0} * FROM ({1}) T", count, _query);
            }

            throw new NotImplementedException();
        }

        public JoinResult<TDes> Join<TDes>(Expression<Func<T, object>> src, Expression<Func<TDes, object>> des)
        {
            throw new NotImplementedException(); 
        }
    }

    public class JoinResult<TDes>
    {
        public SelectFromResult<TDes> SelectFrom<T>(Expression<Func<TDes, object>> expression)
        {
            var xxx = expression.GetArguments().ToDelimString();
            //var query = $"select {xxx} from {typeof (T).Name}";
            throw new NotImplementedException();
        }

        //select *
        public SelectFromResult<TDes> SelectFrom<T>()
        {
            //var xxx = expression.GetArguments().ToDelimString();
            //var query = $"select {xxx} from {typeof (T).Name}";
            throw new NotImplementedException();
        }
    }

    public class DbContext
    {
        public SelectFromResult<T> SelectFrom<T>(Expression<Func<T, object>> expression)
        {
            var cols = expression.GetArguments().ToDelimString();
            var tableName = typeof (T).Name;
            var query = string.Format("SELECT {0} FROM {1}", cols, tableName);
            return new SelectFromResult<T>(query);
        }

        public SelectFromResult<T> SelectFrom<T>(bool distinct, Expression<Func<T, object>> expression)
        {
            var cols = expression.GetArguments().ToDelimString();
            var tableName = typeof(T).Name;
            var query = string.Format(distinct ? "SELECT DISTINCT {0} FROM {1}" : "SELECT {0} FROM {1}", cols, tableName);
            return new SelectFromResult<T>(query);
        }

        public SelectFromResult<T> SelectFrom<T>(uint top, Expression<Func<T, object>> expression)
        {
            var cols = expression.GetArguments().ToDelimString();
            var tableName = typeof(T).Name;
            var query = string.Format("SELECT {0} {1} FROM {2}", top, cols, tableName);
            return new SelectFromResult<T>(query);
        }

        public SelectFromResult<T> SelectFrom<T>(bool distinct, uint top, Expression<Func<T, object>> expression)
        {
            var cols = expression.GetArguments().ToDelimString();
            var tableName = typeof(T).Name;
            var query = string.Format(distinct ? "SELECT DISTINCT {0} {1} FROM {2}" : "SELECT {0} {1} FROM {2}", top, cols, tableName);
            return new SelectFromResult<T>(query);
        }

        public SelectFromResult<T> SelectFrom<T>()
        {
            var tableName = typeof(T).Name;
            var query = string.Format("SELECT {0} FROM {1}", "*", tableName);
            return new SelectFromResult<T>(query);
        }

        public SelectFromResult<T> SelectFrom<T>(uint top)
        {
            var tableName = typeof(T).Name;
            var query = string.Format("SELECT TOP {0} {1} FROM {2}", top, "*", tableName);
            return new SelectFromResult<T>(query);
        }

        public SelectFromResult<T> SelectFrom<T>(bool distinct)
        {
            var tableName = typeof(T).Name;
            var query = string.Format(distinct ? "SELECT DISTINCT {0} FROM {1}" : "SELECT {0} FROM {1}", "*", tableName);
            return new SelectFromResult<T>(query);
        }

        public SelectFromResult<T> SelectFrom<T>(bool distinct, uint top)
        {
            var tableName = typeof(T).Name;
            var query = string.Format(distinct ? "SELECT DISTINCT {0} {1} FROM {2}" : "SELECT {0} {1} FROM {2}", top, "*", tableName);
            return new SelectFromResult<T>(query);
        }
    } 

    public class DbSet<T>
    {

    }

    public static class SqlFunc
    {
        public static AgFunc<T> Max<T>(T t)
        {
            throw new NotImplementedException();
        }

        public static AgFunc<T> Min<T>(T t)
        {
            throw new NotImplementedException();
        }

        public static AgFunc<T> Avg<T>(T t)
        {
            throw new NotImplementedException();
        }

        public static AgFunc<T> Sum<T>(T t)
        {
            throw new NotImplementedException();
        }

        public static AgFunc<int> Count()
        {
            throw new NotImplementedException();
        }

        public static AgFunc<long> LongCount()
        {
            throw new NotImplementedException();
        }
    }

    public class AgFunc<T>
    {
        
    }

    internal sealed class ReferencedPropertyFinder : ExpressionVisitor
    {
        private Type _ownerType;
        private readonly List<string> _properties;

        public ReferencedPropertyFinder()
        {
            _properties = new List<string>();
        }

        public IReadOnlyList<string> GetArguments<T, U>(Expression<Func<T, U>> expression)
        {
            _properties.Clear();
            _ownerType = typeof(T);

            Visit(expression);
            return _properties;
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
    }

    public static class Ex
    {
        public static string ToDelimString(this IEnumerable<string> arr, string delim = ",")
        {
            var ans = "";
            var Enum = arr.GetEnumerator();
            Enum.MoveNext();
            while (Enum.Current != null)
            {
                ans += Enum.Current;
                if (Enum.MoveNext())
                    ans += delim;
                else
                    break;
            }
            return ans;
        }

        public static IReadOnlyList<string> GetArguments<T>(this Expression<Func<T, object>> expression)
        {
            return new ReferencedPropertyFinder().GetArguments(expression).ToList();
        }
    }



}
