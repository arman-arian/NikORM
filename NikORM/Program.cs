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

            var xd = new Query<Account>().Select(false, 1, a => new {a.Balance, a.Id}).Where(a => a.Balance == 0);


            

            Expression<Func<Account, object>> expression = a => new {Ad = SqlFunc.Max(a.Id), a.Id};

            var xdd = expression.GetArguments();


            var ctx = new  TestContext();
            var y = new Query<Account>().Select(a => a).Where(a => a.Balance == 0 && a.Id == 1);

            new Query<Account>().Select(a => a);

            new Query<Account>().Select(1, a => a.Id).Load();

            var df = new Query<Account>().Select(a => a.Balance).Load(10);

            new Query<Account>().Select(x => new {x.Balance, x.Id})
                .Where(a => a.Balance == 1000)
                .Orderby(a => a.Balance)
                .Load();

            new Query<Account>().Select(a => SqlFunc.Max(a.Id)).Load();

            new Query<Account>().Select(a => a.Balance)
                .Join<Customer>(a => a.Id, c => c.AccountId)
                .Select(a => a.Portion)
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


    public class DbContext
    {

    }

    public class Query<TSource> where TSource : new()
    {
        public SelectQueryFromResult<TSource, TResult> Select<TResult>(Expression<Func<TSource, TResult>> expression)
        {
            return Select<TResult>(false, 0, expression);
        }

        public SelectQueryFromResult<TSource, TResult> Select<TResult>(bool distinct, Expression<Func<TSource, TResult>> expression)
        {
            return Select<TResult>(distinct, 0, expression);
        }

        public SelectQueryFromResult<TSource, TResult> Select<TResult>(uint top, Expression<Func<TSource, TResult>> expression)
        {
            return Select<TResult>(false, top, expression);
        }

        public SelectQueryFromResult<TSource, TResult> Select<TResult>(bool distinct, uint top, Expression<Func<TSource, TResult>> expression)
        { 
            var cols = NikHelpers.GetColumnsName(expression);
            var tableName = NikHelpers.GetTableName<TSource>();

            var query = "SELECT ";

            if (distinct)
                query += "DISTINCT ";

            if (top > 0)
                query += string.Format("TOP {0} ", top);

            query += string.Format("{0} ", cols);

            query += string.Format("FROM {0}", tableName);

            return new SelectQueryFromResult<TSource, TResult>(query);
        }
    }

    public class SelectQueryFromResult<TSource, TResult> : IDisposable
    {
        private readonly string _query;

        public SelectQueryFromResult(string query)
        {
            _query = query;
        }

        public WhereQueryResult<TSource> Where(Expression<Func<TSource, bool>> exp)
        {
            throw new NotImplementedException();
        }

        public JoinQueryResult<TDes> Join<TDes>(Expression<Func<TSource, object>> src, Expression<Func<TDes, object>> des)
        {
            throw new NotImplementedException();
        }

        public List<TSource> Load(int count = -1)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }
    }

    public class WhereQueryResult<T>
    {
        public List<T> Load(int count = -1)
        {
            throw new NotImplementedException();
        }

        public OrderQueryByResult<T> Orderby(Expression<Func<T, object>> expression)
        {
            throw new NotImplementedException();
        }

        public OrderQueryByResult<T> OrderbyDescending(Expression<Func<T, object>> expression)
        {
            throw new NotImplementedException();
        }
    }

    public class JoinQueryResult<TSource>
    {
        public SelectQueryFromResult<TSource, TResult> Select<TResult>(Expression<Func<TSource, TResult>> expression)
        {
            return Select<TResult>(false, 0, expression);
        }

        public SelectQueryFromResult<TSource, TResult> Select<TResult>(bool distinct, Expression<Func<TSource, TResult>> expression)
        {
            return Select<TResult>(distinct, 0, expression);
        }

        public SelectQueryFromResult<TSource, TResult> Select<TResult>(uint top, Expression<Func<TSource, TResult>> expression)
        {
            return Select<TResult>(false, top, expression);
        }

        public SelectQueryFromResult<TSource, TResult> Select<TResult>(bool distinct, uint top, Expression<Func<TSource, TResult>> expression)
        {
            var cols = NikHelpers.GetColumnsName(expression);
            var tableName = NikHelpers.GetTableName<TSource>();

            var query = "SELECT ";

            if (distinct)
                query += "DISTINCT ";

            if (top > 0)
                query += string.Format("TOP {0} ", top);

            query += string.Format("{0} ", cols);

            query += string.Format("FROM {0}", tableName);

            return new SelectQueryFromResult<TSource, TResult>(query);
        }
    }

    public class OrderQueryByResult<T>
    {
        public List<T> Load(int count = -1)
        {
            throw new NotImplementedException();
        }
    }







    public static class NikHelpers
    {
        public static string GetColumnsName<TSource, TResult>(Expression<Func<TSource, TResult>> expression)
        {
            return expression.GetArguments();
        }

        public static string GetTableName<T>()
        {
            return typeof(T).Name;
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

        public static string ToDelimString(this IReadOnlyCollection<string> arr, string delim = ",")
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

        public static string GetArguments<TSource, TResult>(this Expression<Func<TSource, TResult>> expression)
        {
            return new SelectExpressionParser().GetArguments(expression);
        }
    }
}
