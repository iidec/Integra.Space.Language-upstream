//-----------------------------------------------------------------------
// <copyright file="EventResultTypeBuilder.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Compiler
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
        /// Initializes a new instance of the <see cref="EventResultTypeBuilder"/> class.
        /// </summary>
        /// <param name="asmBuilder">Assembly builder.</param>
        /// <param name="queryId">Query identifier.</param>
        /// <param name="listOfFields">List of fields</param>
        public EventResultTypeBuilder(AssemblyBuilder asmBuilder, string queryId, List<FieldNode> listOfFields) : base(asmBuilder, "SpaceEventResult_" + queryId, typeof(EventResult), listOfFields)
        {
        }

        /// <inheritdoc />
        public override Type CreateNewType()
        {
            TypeBuilder typeBuilder = this.CreateType();
            foreach (var field in this.Fields)
            {
                this.CreateProperty(typeBuilder, field, true);
            }

            this.CreateConstructor(typeBuilder);
            this.CreateSerializeMethod(typeBuilder);
            return typeBuilder.CreateType();
        }

        /// <inheritdoc />
        protected override void CreateConstructor(TypeBuilder typeBuilder)
        {
            // se obtiene el constructor de la clase padre.
            ConstructorInfo baseConstructor = null;

            if (this.ParentType.GetConstructors().Count() == 1)
            {
                // si solo tiene un constructor, se obtiene ese.
                baseConstructor = this.ParentType.GetConstructors().First();
            }
            else
            {
                // si tiene mas de un constructor se obtiene el constructor que tenga como parametros a cada una de las propiedades de la clase padre.
                baseConstructor = this.ParentType.GetConstructor(this.ParentType.GetProperties().Select(x => x.PropertyType).ToArray());

                // si no se encontró un constructor que cumpla la condicion anterior de parametros entonces se toma el constructor sin parámetros de clase padre.
                if (baseConstructor == null)
                {
                    baseConstructor = this.ParentType.GetConstructor(new Type[] { });
                }
            }            

            Type[] fieldTypes = this.Fields.Select(x => x.FieldType).ToArray();
            Type[] parentConstructorParameterTypes = baseConstructor.GetParameters().Select(x => x.ParameterType).ToArray(); // this.ParentType.GetProperties().Select(x => x.PropertyType).Concat(fieldTypes).ToArray();
            ConstructorBuilder constructor = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, fieldTypes.Concat(parentConstructorParameterTypes).ToArray());
            
            int fieldCount = fieldTypes.Count();
            for (int i = 1; i <= fieldCount; i++)
            {
                constructor.DefineParameter(i, ParameterAttributes.Optional | ParameterAttributes.HasDefault, null);
            }
            
            ILGenerator ctorIL = constructor.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);                // push "this"
            ctorIL.Emit(OpCodes.Call, baseConstructor);
            
            int iAux = 0;
            foreach (string propertyName in this.Fields.Select(x => x.FieldName))
            {
                MethodBuilder setter = this.PropertiesSetters.Single(x => x.Name == "set_" + propertyName);
                ctorIL.Emit(OpCodes.Ldarg_0); // push "this"
                ctorIL.Emit(OpCodes.Ldarg, ++iAux);
                ctorIL.Emit(OpCodes.Call, setter);
            }

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
            foreach (FieldNode t in this.Fields)
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
                if (type.Equals(typeof(TimeSpan)) || type.Equals(typeof(System.Guid)))
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
