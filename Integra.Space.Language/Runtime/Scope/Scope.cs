﻿//-----------------------------------------------------------------------
// <copyright file="Scope.cs" company="Ingetra.Vision.Language">
//     Copyright (c) Ingetra.Vision.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Scope class
    /// </summary>
    internal sealed class Scope
    {
        /// <summary>
        /// scope level
        /// </summary>
        private int level;

        /// <summary>
        /// dictionary of variables of the actual scope
        /// </summary>
        private Dictionary<string, ParameterExpression> variables;

        /// <summary>
        /// list of inner scopes
        /// </summary>
        private List<Scope> innerScopes;

        /// <summary>
        /// parent scope
        /// </summary>
        private Scope parentScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scope"/> class.
        /// </summary>
        /// <param name="level">Scope level.</param>
        /// <param name="parentScope">Parent scope.</param>
        public Scope(int level, Scope parentScope)
        {
            this.level = level;
            this.parentScope = parentScope;
            this.innerScopes = new List<Scope>();
            this.variables = new Dictionary<string, ParameterExpression>();
        }

        /// <summary>
        /// Gets the level of the scope.
        /// </summary>
        public int Level
        {
            get
            {
                return this.level;
            }
        }

        /// <summary>
        /// Gets the parent scope.
        /// </summary>
        public Scope ParentScope
        {
            get
            {
                return this.parentScope;
            }
        }

        /// <summary>
        /// Sets the inner scope.
        /// </summary>
        /// <param name="innerScope">Inner scope.</param>
        public void PushScope(Scope innerScope)
        {
            this.innerScopes.Insert(0, innerScope);
        }

        /// <summary>
        /// Pops an specify inner scope.
        /// </summary>
        /// <param name="innerScope">level of the inner scope to remove.</param>
        /// <returns>True if the scope was removed.</returns>
        public bool RemoveInnerScope(Scope innerScope)
        {
            return this.innerScopes.Remove(innerScope);
        }

        /// <summary>
        /// Gets the specify inner scope.
        /// </summary>
        /// <param name="level">Id of the inner scope.</param>
        /// <returns>Inner scope specified</returns>
        public Scope GetInnerScope(int level)
        {
            return this.innerScopes.Where(x => x.Level == level).First();
        }

        /// <summary>
        /// Gets the specify parameter.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <returns>Parameter found</returns>
        public ParameterExpression GetParameterByName(string name)
        {
            ParameterExpression param = null;

            if (this.variables.ContainsKey(name))
            {
                param = this.variables[name];
            }

            if (param == null && this.parentScope != null)
            {
                param = this.parentScope.GetParameterByName(name);
            }

            if (param == null)
            {
                throw new Exceptions.CompilationException(string.Format("Variable {0} not found", name));
            }

            return param;
        }

        /// <summary>
        /// Gets the specify parameter.
        /// </summary>
        /// <param name="type">Parameter types.</param>
        /// <returns>Parameter found</returns>
        public ParameterExpression GetParameterByType(params System.Type[] type)
        {
            ParameterExpression[] @params = this.variables.Values.Where(x => type.Contains(x.Type)).ToArray();
            
            ParameterExpression param = null;
            if (@params.Count() == 0 && this.parentScope != null)
            {
                param = this.parentScope.GetParameterByType(type);
            }
            else if (@params.Count() != 0)
            {
                param = @params[0];
            }

            if (param == null)
            {
                throw new Exceptions.CompilationException(string.Format("Variable with type {0} not found", type));
            }

            return param;
        }

        /// <summary>
        /// Gets the specify parameter.
        /// </summary>
        /// <param name="genericType">Generic parameter types.</param>
        /// <returns>Parameter found</returns>
        public ParameterExpression GetParameterByGenericType(params System.Type[] genericType)
        {
            ParameterExpression[] @params = this.variables.Values.Where(x =>
            {
                if (x.Type.IsGenericType)
                {
                    return genericType.Contains(x.Type.GetGenericTypeDefinition());
                }

                return false;
            }).ToArray();

            ParameterExpression param = null;
            if (@params.Count() == 0 && this.parentScope != null)
            {
                param = this.parentScope.GetParameterByGenericType(genericType);
            }
            else if (@params.Count() != 0)
            {
                param = @params[0];
            }

            if (param == null)
            {
                throw new Exceptions.CompilationException(string.Format("Variable with type {0} not found", genericType));
            }

            return param;
        }

        /// <summary>
        ///  Push a new parameter.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="param">Parameter name.</param>
        public void AddParameter(string name, ParameterExpression param)
        {
            try
            {
                this.variables.Add(name, param);
            }
            catch (System.Exception e)
            {
                throw new Exceptions.CompilationException(string.Format("The variable already exists in this scope."));
            }
        }

        /// <summary>
        /// Pop a parameter.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <returns>The variable at the first position of the dictionary.</returns>
        public bool RemoveParameter(string name = "")
        {
            return this.variables.Remove(name);
        }
    }
}
