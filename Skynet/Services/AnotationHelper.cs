using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfCoreHelper.Database.Core
{
    public class AnnotationHelper
    {
        private static string GetName(IEntityType entityType,
                                      string defaultSchemaName = "dbo")
        {
            /*3.0.1 these were working*/
            //var schemaName = entityType.GetSchema();
            //var tableName = entityType.GetTableName();
            /*5 and 6 these are working*/
            var schema = entityType.FindAnnotation("Relational:Schema").Value;
            string tableName = entityType.GetAnnotation
                               ("Relational:TableName").Value.ToString();
            string schemaName = schema == null ? defaultSchemaName : schema.ToString();
            /*table full name*/
            string name = string.Format("[{0}].[{1}]", schemaName, tableName);
            return name;
        }

        public static string TableName<T>(DbContext dbContext) where T : class
        {
            var entityType = dbContext.Model.FindEntityType(typeof(T));
            return GetName(entityType);
        }

        public static string TableName<T>(DbSet<T> dbSet) where T : class
        {
            var entityType = dbSet.EntityType;
            return GetName(entityType);
        }
    }
}