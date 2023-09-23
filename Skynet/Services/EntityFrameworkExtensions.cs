using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;

namespace EfCoreHelper.Database.Core
{
    //public static class EfCleanHelper
    //{
    //    public static string Truncate<T>(this DbSet<T> dbSet) where T : class
    //    {
    //        string cmd = $"TRUNCATE TABLE {AnnotationHelper.TableName(dbSet)}";
    //        var context = dbSet.GetService<ICurrentDbContext>().Context;
    //        context.Database.ExecuteSqlRaw(cmd);
    //        return cmd;
    //    }

    //    public static string Delete<T>(this DbSet<T> dbSet) where T : class
    //    {
    //        string cmd = $"DELETE FROM {AnnotationHelper.TableName(dbSet)}";
    //        var context = dbSet.GetService<ICurrentDbContext>().Context;
    //        context.Database.ExecuteSqlRaw(cmd);
    //        return cmd;
    //    }

    //    public static void Clear<T>(this DbContext context) where T : class
    //    {
    //        DbSet<T> dbSet = context.Set<T>();
    //        if (dbSet.Any())
    //        {
    //            dbSet.RemoveRange(dbSet.ToList());
    //        }
    //    }

    //    public static void Clear<T>(this DbSet<T> dbSet) where T : class
    //    {
    //        if (dbSet.Any())
    //        {
    //            dbSet.RemoveRange(dbSet.ToList());
    //        }
    //    }

    //    public static string Truncate(this DbContext context,
    //                  string tableName, string schemaName = "dbo")
    //    {
    //        string name = string.Format("[{0}].[{1}]", schemaName, tableName);
    //        string cmd = $"TRUNCATE TABLE {name}";
    //        context.Database.ExecuteSqlRaw(cmd);
    //        return cmd;
    //    }

    //    public static string Delete(this DbContext context,
    //                  string tableName, string schemaName = "dbo")
    //    {
    //        string name = string.Format("[{0}].[{1}]", schemaName, tableName);
    //        string cmd = $"DELETE FROM {name}";
    //        context.Database.ExecuteSqlRaw(cmd);
    //        return cmd;
    //    }
    //}
}