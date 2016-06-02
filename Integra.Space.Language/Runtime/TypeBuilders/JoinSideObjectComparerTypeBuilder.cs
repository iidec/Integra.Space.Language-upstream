//-----------------------------------------------------------------------
// <copyright file="JoinSideObjectComparerTypeBuilder.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Join side object comparer class.
    /// </summary>
    internal class JoinSideObjectComparerTypeBuilder : SpaceTypeBuilder
    {
        /// <summary>
        /// Fields to create in the new type.
        /// </summary>
        private List<FieldNode> listOfFields;

        /// <summary>
        /// Type of the opposite.
        /// </summary>
        private Type typeOfTheOtherSource;

        /// <summary>
        /// Value indicating whether this type is for the second source.
        /// </summary>
        private bool isSecondSource;

        /// <summary>
        /// Lambda expression of the on condition of the query.
        /// </summary>
        private LambdaExpression onCondition;

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinSideObjectComparerTypeBuilder"/> class.
        /// </summary>
        /// <param name="listOfFields">List of fields</param>
        /// <param name="parentType">Parent type of the new type.</param>
        /// <param name="typeOfTheOtherSource">Type of the opposite.</param>
        /// <param name="isSecondSource">Value indicating whether this type is for the second source.</param>
        /// <param name="onCondition">Lambda expression of the on condition of the query.</param>
        public JoinSideObjectComparerTypeBuilder(List<FieldNode> listOfFields, Type parentType, Type typeOfTheOtherSource, bool isSecondSource, LambdaExpression onCondition) : base("SpaceJoinSideObjectComparerAssembly_" + parentType.Name, "SpaceJoinSideObjectComparer_" + parentType.Name, parentType)
        {
            this.listOfFields = listOfFields;
            this.typeOfTheOtherSource = typeOfTheOtherSource;
            this.isSecondSource = isSecondSource;
            this.onCondition = onCondition;
        }

        /// <inheritdoc />
        public override Type CreateNewType()
        {
            AssemblyBuilder asmBuilder = this.CreateAssembly();
            ModuleBuilder modBuilder = this.CreateModule(asmBuilder);
            TypeBuilder typeBuilder = this.CreateType(modBuilder);
            this.CreateConstructor(typeBuilder);
            
            this.CreateGetHashCodeMethod(typeBuilder);
            this.CreateEqualsMethod(typeBuilder);

            return typeBuilder.CreateType();
        }

        /// <summary>
        /// Create serialize method.
        /// </summary>
        /// <param name="tb">Type builder.</param>
        private void CreateGetHashCodeMethod(TypeBuilder tb)
        {
            // obtengo el método Serialize del padre
            MethodInfo parentGetHashCode = this.ParentType.GetMethod("GetHashCode");

            // defino el cuerpo que sobreescribirá al cuerpo del método Serialize
            MethodBuilder getHashCodeMethod = tb.DefineMethod("GetHashCode", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final, CallingConventions.HasThis, typeof(int), Type.EmptyTypes);
            tb.DefineMethodOverride(getHashCodeMethod, parentGetHashCode);

            // defino el cuerpo del metodo Serialize
            ILGenerator getHashCodeIL = getHashCodeMethod.GetILGenerator();

            MethodInfo getHashCodeMethodOfField = typeof(object).GetMethod("GetHashCode");
            bool first = true;

            foreach (FieldNode t in this.listOfFields)
            {
                if (first)
                {
                    getHashCodeIL.Emit(OpCodes.Ldc_I4, 2166136261);
                    first = false;
                }

                for (int i = 0; i < t.IncidenciasEnOnCondition; i++)
                {
                    getHashCodeIL.Emit(OpCodes.Ldc_I4, 16777619);
                    getHashCodeIL.Emit(OpCodes.Mul);

                    getHashCodeIL.Emit(OpCodes.Ldarg_0);

                    getHashCodeIL.Emit(OpCodes.Call, this.ParentType.GetMethod("get_" + t.FieldName));
                    getHashCodeIL.Emit(OpCodes.Callvirt, getHashCodeMethodOfField);
                    getHashCodeIL.Emit(OpCodes.Xor);
                }
            }

            getHashCodeIL.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Doc goes here.
        /// </summary>
        /// <param name="tb">Type builder</param>
        private void CreateEqualsMethod(TypeBuilder tb)
        {
            // obtengo el método Serialize del padre
            MethodInfo parentEquals = this.ParentType.GetMethod("Equals", new Type[] { typeof(object) });

            // defino el cuerpo que sobreescribirá al cuerpo del método Serialize
            MethodBuilder equalsMethod = tb.DefineMethod("Equals", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final, CallingConventions.HasThis, typeof(bool), new Type[] { typeof(object) });
            tb.DefineMethodOverride(equalsMethod, parentEquals);

            ILGenerator equalsIL = equalsMethod.GetILGenerator();
            equalsIL.Emit(OpCodes.Nop);

            Label sameObjectComparer = equalsIL.DefineLabel();
            Label endOfMethod = equalsIL.DefineLabel();

            MethodInfo getTypeMethodInfo = typeof(object).GetMethod("GetType");
            MethodInfo getBaseTypeMethodInfo = typeof(Type).GetProperty("BaseType").GetGetMethod();

            // coloco en la pila de evaluación el tipo de la clase padre
            MethodInfo getTypeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle");
            equalsIL.Emit(OpCodes.Ldtoken, this.ParentType);
            equalsIL.Emit(OpCodes.Call, getTypeFromHandleMethod);

            // obtengo el tipo de la clase actual
            equalsIL.Emit(OpCodes.Ldarg_1); // push the first parameter
            equalsIL.Emit(OpCodes.Callvirt, getTypeMethodInfo);
            equalsIL.Emit(OpCodes.Callvirt, getBaseTypeMethodInfo);

            // llamo el metodo Inequality de la clase Type
            MethodInfo inequalityMethod = typeof(Type).GetMethod("op_Inequality");
            equalsIL.Emit(OpCodes.Call, inequalityMethod);

            // si son iguales salta a la etiqueta especificada
            equalsIL.Emit(OpCodes.Brfalse, sameObjectComparer);

            MethodBuilder evaluateOnCondition = null;
            MethodInfo writeMethodInfo = typeof(System.Diagnostics.Debug).GetMethod("Write", new Type[] { typeof(object) });
            MethodInfo writeLineMethodInfo = typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) });
            
            if (!this.isSecondSource)
            {
                equalsIL.Emit(OpCodes.Ldarg_0); // push "this"
                equalsIL.Emit(OpCodes.Ldarg_1); // push the first parameter
                equalsIL.Emit(OpCodes.Castclass, this.typeOfTheOtherSource);

                evaluateOnCondition = tb.DefineMethod("EvaluateOnCondition", MethodAttributes.Public | MethodAttributes.Static, typeof(bool), new[] { this.ParentType, this.typeOfTheOtherSource });
                this.onCondition.CompileToMethod(evaluateOnCondition);
            }
            else
            {
                equalsIL.Emit(OpCodes.Ldarg_1); // push the first parameter
                equalsIL.Emit(OpCodes.Castclass, this.typeOfTheOtherSource);
                equalsIL.Emit(OpCodes.Ldarg_0); // push "this"

                evaluateOnCondition = tb.DefineMethod("EvaluateOnCondition", MethodAttributes.Public | MethodAttributes.Static, typeof(bool), new[] { this.typeOfTheOtherSource, this.ParentType });
                this.onCondition.CompileToMethod(evaluateOnCondition);
            }

            equalsIL.Emit(OpCodes.Call, evaluateOnCondition);
            equalsIL.Emit(OpCodes.Br_S, endOfMethod);

            MethodBuilder sameObjectComparerMethodBuilder = this.CreateCompareSameObjectMethod(tb);
            equalsIL.MarkLabel(sameObjectComparer);

            equalsIL.Emit(OpCodes.Ldarg_0);
            equalsIL.Emit(OpCodes.Ldarg_1);
            equalsIL.Emit(OpCodes.Call, sameObjectComparerMethodBuilder);

            equalsIL.MarkLabel(endOfMethod);
            equalsIL.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Create serialize method.
        /// </summary>
        /// <param name="tb">Type builder.</param>
        /// <returns>CompareSameObject method builder.</returns>
        private MethodBuilder CreateCompareSameObjectMethod(TypeBuilder tb)
        {
            // defino el cuerpo que sobreescribirá al cuerpo del método Serialize
            MethodBuilder compareSameTypeMethod = tb.DefineMethod("CompareSameType", MethodAttributes.Public | MethodAttributes.Static, typeof(bool), new Type[] { this.ParentType, this.ParentType });

            // defino el cuerpo del metodo Serialize
            ILGenerator compareSameTypeIL = compareSameTypeMethod.GetILGenerator();

            compareSameTypeIL.Emit(OpCodes.Nop);
            Label pushFalse = compareSameTypeIL.DefineLabel();

            bool first = true;

            foreach (FieldNode t in this.listOfFields)
            {
                for (int i = 0; i < t.IncidenciasEnOnCondition; i++)
                {
                    if (first)
                    {
                        compareSameTypeIL.Emit(OpCodes.Ldarg_0); // push this
                        compareSameTypeIL.Emit(OpCodes.Call, this.ParentType.GetMethod("get_" + t.FieldName));

                        compareSameTypeIL.Emit(OpCodes.Ldarg_1); // push the first parameter
                        compareSameTypeIL.Emit(OpCodes.Call, this.ParentType.GetMethod("get_" + t.FieldName));

                        compareSameTypeIL.Emit(OpCodes.Ceq);
                        compareSameTypeIL.Emit(OpCodes.Brfalse, pushFalse);

                        first = false;
                    }
                    else
                    {
                        compareSameTypeIL.Emit(OpCodes.Ldarg_0); // push this
                        compareSameTypeIL.Emit(OpCodes.Call, this.ParentType.GetMethod("get_" + t.FieldName));

                        compareSameTypeIL.Emit(OpCodes.Ldarg_1); // push the first parameter
                        compareSameTypeIL.Emit(OpCodes.Call, this.ParentType.GetMethod("get_" + t.FieldName));

                        compareSameTypeIL.Emit(OpCodes.Ceq);
                        compareSameTypeIL.Emit(OpCodes.Brfalse, pushFalse);
                    }
                }
            }

            compareSameTypeIL.Emit(OpCodes.Ldc_I4_1);
            compareSameTypeIL.Emit(OpCodes.Ret);

            compareSameTypeIL.MarkLabel(pushFalse);
            compareSameTypeIL.Emit(OpCodes.Ldc_I4_0);
            compareSameTypeIL.Emit(OpCodes.Ret);

            return compareSameTypeMethod;
        }
    }
}
