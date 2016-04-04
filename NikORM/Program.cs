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

            var xd = new Query<Account>().SelectFrom(a => new {a.Balance, a.Id});


            

            Expression<Func<Account, object>> expression = a => new {Ad = SqlFunc.Max(a.Id), a.Id};

            var xdd = expression.GetArguments();


            var ctx = new  TestContext();
            var y = ctx.SelectFrom<Account>(a => a).Where(a => a.Balance == 0 && a.Id == 1);

            ctx.SelectFrom<Account>(a => a);

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



    public class JoinResult<TDes>
    {
        public SelectFromResult<TDes> SelectFrom<T>(Expression<Func<TDes, object>> expression)
        {
            var xxx = expression.GetArguments();
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
            return SelectFrom<T>(false, 0, expression);
        }

        private static TResult A<TSource, TResult>(TSource source, Func<TSource, TResult> expr) where TSource : new()
        {
            var obj = new TSource();
            var property = typeof(TSource).GetProperty("Id");
            property.SetValue(obj, 1, null);
            return expr(obj);
        }


        public SelectFromResult<T> SelectFrom<T>(bool distinct, Expression<Func<T, object>> expression)
        {
            return SelectFrom<T>(distinct, 0, expression);
        }

        public SelectFromResult<T> SelectFrom<T>(uint top, Expression<Func<T, object>> expression)
        {
            return SelectFrom<T>(false, top, expression);
        }

        public SelectFromResult<T> SelectFrom<T>(bool distinct, uint top, Expression<Func<T, object>> expression)
        {
            var cols = NikHelpers.GetColumnsName(expression);
            var tableName = NikHelpers.GetTableName<T>();

            var query = "SELECT ";

            if (distinct)
                query += "DISTINCT ";

            if (top > 0)
                query += string.Format("TOP {0} ", top);

            query += string.Format("{0} ", cols);

            query += string.Format("FROM {0}", tableName);

            return new SelectFromResult<T>(query);
        }
    }

    public class Query<TSource> where TSource : new()
    {
        public TResult SelectFrom<TResult>(Func<TSource, TResult> expr) 
        {
            var obj = new TSource();
            var property = typeof(TSource).GetProperty("Id");
            property.SetValue(obj, 1, null);
            return expr(obj);
        }
    }


    public class SelectFromResult<T> : IDisposable
    {
        private readonly string _query;

        public SelectFromResult(string query)
        {
            _query = query;
        }

        public WhereResult<T> Where(Expression<Func<T, bool>> exp)
        {
            var qstr = exp.ToString();

            throw new NotImplementedException();
        }

        public JoinResult<TDes> Join<TDes>(Expression<Func<T, object>> src, Expression<Func<TDes, object>> des)
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

        public void Dispose()
        {
            
        }

        //class Program
        //{
        //    static void Main(string[] args)
        //    {
        //        var v1 = new { Amount = 108, Message = "Hello" };

        //        var v2 = new[] { new { name = "apple", diam = 4 }, new { name = "grape", diam = 1 } };

        //        Expression<Func<Account, object>> expr = a => new { a.Id, B = a.Balance };

        //        var t = A(new Account(), a => new { a.Id, B = SqlFunc.Avg(a.Balance) });

        //        var id = t.Id;
        //    }

        //    private static TResult A<TSource, TResult>(TSource source, Func<TSource, TResult> expr) where TSource : new()
        //    {
        //        var obj = new TSource();
        //        var property = typeof(TSource).GetProperty("Id");
        //        property.SetValue(obj, 1, null);
        //        return expr(obj);
        //    }
        //}
    }

















    public static class NikHelpers
    {
        public static string GetColumnsName<T>(Expression<Func<T, object>> expression)
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

        public static string GetArguments<T>(this Expression<Func<T, object>> expression)
        {
            return new SelectExpressionParser().GetArguments(expression);
        }
    }



}
