using System;
using Integra.Space.Common;
using Integra.Space.Compiler;

namespace Integra.Space.Language.Analysis
{
    internal class SourceTypeFactory : ISourceTypeFactory
    {
        public Type GetSourceType(CommandObject source)
        {
            /*
            using (SpaceDbContext dbContext = new SpaceDbContext())
            {
                Schema schema = source.GetSchema(dbContext, login);
                Source sourceFromDatabase = dbContext.Sources.Single(x => x.ServerId == schema.ServerId
                                        && x.DatabaseId == schema.DatabaseId
                                        && x.SchemaId == schema.SchemaId
                                        && x.SourceName == source.Name);

                IEnumerable<FieldNode> fields = sourceFromDatabase.Columns.OrderBy(x => x.ColumnIndex).Select(x => new FieldNode(x.ColumnName, Type.GetType(x.ColumnType)));
                string typeSignature = string.Format("{0}_{1}_{2}_{3}", schema.Database.Server.ServerName, schema.Database.DatabaseName, schema.SchemaName, source.Name);

                SourceTypeBuilder typeBuilder = new SourceTypeBuilder(this.config.AsmBuilder, typeSignature, typeof(InputBase), fields);
                Type sourceGeneratedType = typeBuilder.CreateNewType();
                sourceGeneratedType = typeof(System.IObservable<>).MakeGenericType(new Type[] { sourceGeneratedType });
            }
            */

            return typeof(TestObject1);
        }
    }
}
