//-----------------------------------------------------------------------
// <copyright file="EventResultTypeBuilder.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Event result type builder class.
    /// </summary>
    internal class EventResultTypeBuilder : SpaceTypeBuilder
    {
        /// <summary>
        /// Fields to create in the new type.
        /// </summary>
        private List<FieldNode> listOfFields;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventResultTypeBuilder"/> class.
        /// </summary>
        /// <param name="asmBuilder">Assembly builder.</param>
        /// <param name="queryId">Query identifier.</param>
        /// <param name="listOfFields">List of fields</param>
        public EventResultTypeBuilder(AssemblyBuilder asmBuilder, string queryId, List<FieldNode> listOfFields) : base(asmBuilder, "SpaceEventResult_" + queryId, typeof(EventResult))
        {
            this.listOfFields = listOfFields;
        }

        /// <inheritdoc />
        public override Type CreateNewType()
        {
            TypeBuilder typeBuilder = this.CreateType();
            this.CreateConstructor(typeBuilder);

            foreach (var field in this.listOfFields)
            {
                this.CreateProperty(typeBuilder, field);
            }

            this.CreateSerializeMethod(typeBuilder);

            return typeBuilder.CreateType();
        }

        /// <inheritdoc />
        protected override void CreateConstructor(TypeBuilder typeBuilder)
        {
            ConstructorBuilder constructor = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, this.ParentType.GetProperties().Select(x => x.PropertyType).ToArray());
            ConstructorInfo baseConstructor = this.ParentType.GetConstructor(this.ParentType.GetProperties().Select(x => x.PropertyType).ToArray());

            ILGenerator ctorIL = constructor.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);                // push "this"
            ctorIL.Emit(OpCodes.Call, baseConstructor);
            ctorIL.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Create serialize method.
        /// </summary>
        /// <param name="tb">Type builder.</param>
        private void CreateSerializeMethod(TypeBuilder tb)
        {
            // obtengo el método Serialize del padre
            MethodInfo parentSerialize = this.ParentType.GetMethod("Serialize", new Type[] { typeof(IQueryResultWriter) });

            // defino el cuerpo que sobreescribirá al cuerpo del método Serialize
            // MethodBuilder serializeMethod = tb.DefineMethod("Serialize", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, null, new Type[] { typeof(IQueryResultWriter) });
            MethodBuilder serializeMethod = tb.DefineMethod("Serialize", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final, CallingConventions.HasThis, null, new Type[] { typeof(IQueryResultWriter) });

            // defino el parámetro de tipo IQueryResultWriter
            serializeMethod.DefineParameter(1, ParameterAttributes.In, "writer");

            tb.DefineMethodOverride(serializeMethod, parentSerialize);

            // definición del método WriteValue de IQueryResultWriter
            MethodInfo writeStartQueryResultRowMethod = typeof(IQueryResultWriter).GetMethods().Where(x => x.Name == "WriteStartQueryResultRow").Single();
            MethodInfo writeEndQueryResultRowMethod = typeof(IQueryResultWriter).GetMethods().Where(x => x.Name == "WriteEndQueryResultRow").Single();

            // defino el cuerpo del metodo Serialize
            ILGenerator serializeIL = serializeMethod.GetILGenerator();
            serializeIL.Emit(OpCodes.Ldarg_0);                   // push "this"
            serializeIL.Emit(OpCodes.Ldarg_1);                   // push the first parameter
            serializeIL.Emit(OpCodes.Call, parentSerialize);    // llamo al método base para que imprima el nombre del query

            MethodInfo writeValueMethod = null;
            foreach (FieldNode t in this.listOfFields)
            {
                writeValueMethod = typeof(IQueryResultWriter).GetMethods().Where(x => x.Name == "WriteValue" && x.GetParameters()[0].ParameterType == this.ToPrimitiveNullable(t.FieldType)).Single();

                serializeIL.Emit(OpCodes.Ldarg_1);                   // push the first parameter
                serializeIL.Emit(OpCodes.Ldarg_0);
                serializeIL.Emit(OpCodes.Ldfld, t.FieldBuilder);
                serializeIL.Emit(OpCodes.Call, writeValueMethod);   // llamo al método WriteValue del serializador 
            }

            serializeIL.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// check if is a numeric type
        /// </summary>
        /// <param name="type">type to check</param>
        /// <returns>true if is numeric type</returns>
        private Type ToPrimitiveNullable(Type type)
        {
            if (Nullable.GetUnderlyingType(type) != null)
            {
                return type;
            }
            else
            {
                if (type.Equals(typeof(TimeSpan)))
                {
                    return typeof(Nullable<>).MakeGenericType(type);
                }

                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                    case TypeCode.DateTime:
                        return typeof(Nullable<>).MakeGenericType(type);
                    default:
                        return type;
                }
            }
        }
    }
}
