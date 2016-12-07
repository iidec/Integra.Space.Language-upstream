using System;
using Integra.Space.Common;
using Integra.Space.LanguageUnitTests.TestObject;
using Integra.Space.Database;
using System.Collections.Generic;
using Integra.Space.Compiler;
using System.Linq;

namespace Integra.Space.LanguageUnitTests
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

            if (source.Name == "SourceParaPruebas1" || source.Name == "SourceParaPruebas")
            {
                return typeof(TestObject1);
            }
            else if (source.Name == "SourceParaPruebas3")
            {
                return typeof(TestObject3);
            }
            else
            {
                throw new Exception(string.Format("Define a type for the source {0}.", source.Name));
            }
        }
    }
}
