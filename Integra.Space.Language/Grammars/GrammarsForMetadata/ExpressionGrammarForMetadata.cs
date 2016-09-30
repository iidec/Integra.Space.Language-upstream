//-----------------------------------------------------------------------
// <copyright file="ExpressionGrammarForMetadata.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Grammars
{
    using System;
    using System.Linq;
    using ASTNodes;
    using ASTNodes.Cast;
    using ASTNodes.Constants;
    using ASTNodes.Identifier;
    using ASTNodes.Lists;
    using ASTNodes.MetadataQuery;
    using ASTNodes.Operations;
    using ASTNodes.QuerySections;
    using ASTNodes.Values.Functions;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// EQLGrammar grammar for the commands and the predicates 
    /// </summary>
    [Language("ExpressionGrammarForMetadata", "0.1", "")]
    internal sealed class ExpressionGrammarForMetadata : InterpretedLanguageGrammar
    {
        /// <summary>
        /// Grammar to add the expression rules
        /// </summary>
        private NonTerminal logicExpression;

        /// <summary>
        /// Grammar to add the expression rules
        /// </summary>
        private NonTerminal logicExpressionForOnCondition;

        /// <summary>
        /// All values
        /// </summary>
        private NonTerminal values;

        /// <summary>
        /// Other values: string, boolean, null
        /// </summary>
        private NonTerminal otherValues;

        /// <summary>
        /// Numeric values
        /// </summary>
        private NonTerminal numericValues;

        /// <summary>
        /// Non constant values: objects
        /// </summary>
        private NonTerminal nonConstantValues;

        /// <summary>
        /// Projection values
        /// </summary>
        private NonTerminal projectionValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionGrammarForMetadata"/> class
        /// </summary>
        public ExpressionGrammarForMetadata() : base(false)
        {
            this.Grammar();
        }

        /// <summary>
        /// Gets the nonterminal for all values
        /// </summary>
        public NonTerminal Values
        {
            get
            {
                return this.values;
            }
        }

        /// <summary>
        /// Gets the projection value
        /// </summary>
        public NonTerminal ProjectionValue
        {
            get
            {
                return this.projectionValue;
            }
        }
        
        /// <summary>
        /// Gets the nonterminal for expression conditions
        /// </summary>
        public NonTerminal LogicExpression
        {
            get
            {
                return this.logicExpression;
            }
        }

        /// <summary>
        /// Gets the nonterminal for 'on' conditions
        /// </summary>
        public NonTerminal LogicExpressionForOnCondition
        {
            get
            {
                return this.logicExpressionForOnCondition;
            }
        }

        /// <summary>
        /// Expression grammar
        /// </summary>
        public void Grammar()
        {
            /* PROYECCIÓN */
            KeyTerm terminalKey = ToTerm("key", "key");

            /* FUNCIONES */
            KeyTerm terminalYear = ToTerm("year", "year");
            KeyTerm terminalMonth = ToTerm("month", "month");
            KeyTerm terminalDay = ToTerm("day", "day");
            KeyTerm terminalHour = ToTerm("hour", "hour");
            KeyTerm terminalMinute = ToTerm("minute", "minute");
            KeyTerm terminalSecond = ToTerm("second", "second");
            KeyTerm terminalMillisecond = ToTerm("millisecond", "millisecond");
            KeyTerm terminalCount = ToTerm("count", "count");
            KeyTerm terminalSum = ToTerm("sum", "sum");
            KeyTerm terminalMin = ToTerm("min", "min");
            KeyTerm terminalMax = ToTerm("max", "max");
            KeyTerm terminalLeft = ToTerm("left", "left");
            KeyTerm terminalRight = ToTerm("right", "right");
            KeyTerm terminalUpper = ToTerm("upper", "upper");
            KeyTerm terminalLower = ToTerm("lower", "lower");
            KeyTerm terminalAbs = ToTerm("abs", "abs");
            KeyTerm terminalIsnull = ToTerm("isnull", "isnull");
            
            /* OPERADORES */
            KeyTerm terminalLike = ToTerm("like", "like");
            KeyTerm terminalNot = ToTerm("not", "not");
            KeyTerm terminalAnd = ToTerm("and", "and");
            KeyTerm terminalOr = ToTerm("or", "or");

            // Marcamos los terminales, definidos hasta el momento, como palabras reservadas
            this.MarkReservedWords(this.KeyTerms.Keys.ToArray());

            /* OPERADORES ARITMETICOS */
            KeyTerm terminalMenos = ToTerm("-", "minus");
            KeyTerm terminalMas = ToTerm("+", "plus");

            /* OPERADORES COMPARATIVOS */
            KeyTerm terminalIgualIgual = ToTerm("==", "equalEqual");
            KeyTerm terminalNoIgual = ToTerm("!=", "notEqual");
            KeyTerm terminalMayorIgual = ToTerm(">=", "greaterThanOrEqual");
            KeyTerm terminalMenorIgual = ToTerm("<=", "lessThanOrEqual");
            KeyTerm terminalMayorQue = ToTerm(">", "greaterThan");
            KeyTerm terminalMenorQue = ToTerm("<", "lessThan");

            /* TIPOS PARA CASTEO */
            ConstantTerminal terminalType = new ConstantTerminal("dataTypes");
            terminalType.Add("byte", typeof(byte));
            terminalType.Add("byte?", typeof(byte?));
            terminalType.Add("sbyte", typeof(sbyte));
            terminalType.Add("sbyte?", typeof(sbyte?));
            terminalType.Add("short", typeof(short));
            terminalType.Add("short?", typeof(short?));
            terminalType.Add("ushort", typeof(ushort));
            terminalType.Add("ushort?", typeof(ushort?));
            terminalType.Add("int", typeof(int));
            terminalType.Add("int?", typeof(int?));
            terminalType.Add("uint", typeof(uint));
            terminalType.Add("uint?", typeof(uint?));
            terminalType.Add("long", typeof(long));
            terminalType.Add("long?", typeof(long?));
            terminalType.Add("ulong", typeof(ulong));
            terminalType.Add("ulong?", typeof(ulong?));
            terminalType.Add("float", typeof(float));
            terminalType.Add("float?", typeof(float?));
            terminalType.Add("double", typeof(double));
            terminalType.Add("double?", typeof(double?));
            terminalType.Add("decimal", typeof(decimal));
            terminalType.Add("decimal?", typeof(decimal?));
            terminalType.Add("char", typeof(char));
            terminalType.Add("char?", typeof(char?));
            terminalType.Add("string", typeof(string));
            terminalType.Add("bool", typeof(bool));
            terminalType.Add("bool?", typeof(bool?));
            terminalType.Add("object", typeof(object));
            terminalType.Add("DateTime", typeof(DateTime));
            terminalType.Add("DateTime?", typeof(DateTime?));
            terminalType.Add("TimeSpan", typeof(TimeSpan));
            terminalType.Add("TimeSpan?", typeof(TimeSpan?));
            terminalType.AstConfig.NodeType = null;
            terminalType.AstConfig.DefaultNodeCreator = () => new TypeNode();

            /* SIMBOLOS */
            KeyTerm terminalParentesisIz = ToTerm("(", "leftParenthesis");
            KeyTerm terminalParentesisDer = this.ToTerm(")", "rightParenthesis");
            KeyTerm terminalCorcheteIz = ToTerm("[", "leftBracket");
            KeyTerm terminalCorcheteDer = ToTerm("]", "rightBracket");
            KeyTerm terminalPunto = ToTerm(".", "dot");
            KeyTerm terminalComillaSimple = ToTerm("'", "singleQuote");
            KeyTerm terminalComa = ToTerm(",", "coma");

            /* COMENTARIOS */
            CommentTerminal comentarioLinea = new CommentTerminal("line_coment", "//", "\n", "\r\n");
            CommentTerminal comentarioBloque = new CommentTerminal("block_coment", "/*", "*/");
            NonGrammarTerminals.Add(comentarioLinea);
            NonGrammarTerminals.Add(comentarioBloque);

            /* CONSTANTES E IDENTIFICADORES */
            Terminal terminalNumero = TerminalFactory.CreateCSharpNumber("number");
            terminalNumero.AstConfig.NodeType = null;
            terminalNumero.AstConfig.DefaultNodeCreator = () => new NumberNode();
            Terminal terminalCadena = TerminalFactory.CreateCSharpString("string");
            terminalCadena.AstConfig.NodeType = null;
            terminalCadena.AstConfig.DefaultNodeCreator = () => new StringNode();
            ConstantTerminal terminalBool = new ConstantTerminal("constantBool");
            terminalBool.Add("true", true);
            terminalBool.Add("false", false);
            terminalBool.AstConfig.NodeType = null;
            terminalBool.AstConfig.DefaultNodeCreator = () => new BooleanNode();
            Terminal terminalDateTimeValue = new QuotedValueLiteral("datetimeValue", "'", TypeCode.String);
            terminalDateTimeValue.AstConfig.NodeType = null;
            terminalDateTimeValue.AstConfig.DefaultNodeCreator = () => new DateTimeOrTimespanNode();
            ConstantTerminal terminalNull = new ConstantTerminal("constantNull");
            terminalNull.Add("null", null);
            terminalNull.AstConfig.NodeType = null;
            terminalNull.AstConfig.DefaultNodeCreator = () => new NullValueNode();

            RegexBasedTerminal terminalId = new RegexBasedTerminal("[a-zA-Z]+([a-zA-Z]|[0-9]|[_])*");
            terminalId.Name = "identifier";
            terminalId.AstConfig.NodeType = null;
            terminalId.AstConfig.DefaultNodeCreator = () => new IdentifierNode();

            /* PRECEDENCIA Y ASOCIATIVIDAD */
            this.RegisterBracePair("(", ")");
            this.RegisterBracePair("[", "]");
            this.RegisterOperators(40, Associativity.Right, terminalParentesisIz, terminalParentesisDer, terminalCorcheteIz, terminalCorcheteDer);
            this.RegisterOperators(35, Associativity.Right, terminalMas, terminalMenos);
            this.RegisterOperators(30, Associativity.Right, terminalIgualIgual, terminalNoIgual, terminalMayorIgual, terminalMayorQue, terminalMenorIgual, terminalMenorQue, terminalLike);
            this.RegisterOperators(20, Associativity.Right, terminalAnd);
            this.RegisterOperators(10, Associativity.Right, terminalOr);
            this.RegisterOperators(5, Associativity.Right, terminalNot);
            this.MarkPunctuation(terminalParentesisIz, terminalParentesisDer, terminalCorcheteIz, terminalCorcheteDer, terminalPunto, terminalComa);
            
            /* NO TERMINALES */
            NonTerminal nt_COMPARATIVE_EXPRESSION = new NonTerminal("COMPARATIVE_EXPRESSION_FOR_METADATA", typeof(ComparativeExpressionNode));
            nt_COMPARATIVE_EXPRESSION.AstConfig.NodeType = null;
            nt_COMPARATIVE_EXPRESSION.AstConfig.DefaultNodeCreator = () => new ComparativeExpressionNode();
            this.logicExpression = new NonTerminal("LOGIC_EXPRESSION_FOR_METADATA", typeof(LogicExpressionNode));
            this.logicExpression.AstConfig.NodeType = null;
            this.logicExpression.AstConfig.DefaultNodeCreator = () => new LogicExpressionNode();
            this.values = new NonTerminal("VALUE_FOR_METADATA", typeof(PassNode));
            this.values.AstConfig.NodeType = null;
            this.values.AstConfig.DefaultNodeCreator = () => new PassNode();
            this.numericValues = new NonTerminal("NUMERIC_VALUES_FOR_METADATA", typeof(PassNode));
            this.numericValues.AstConfig.NodeType = null;
            this.numericValues.AstConfig.DefaultNodeCreator = () => new PassNode();
            this.nonConstantValues = new NonTerminal("NON_CONSTANT_VALUES_FOR_METADATA", typeof(NonConstantForMetadataASTNode));
            this.nonConstantValues.AstConfig.NodeType = null;
            this.nonConstantValues.AstConfig.DefaultNodeCreator = () => new NonConstantForMetadataASTNode();
            this.otherValues = new NonTerminal("OTHER_VALUES_FOR_METADATA", typeof(PassNode));
            this.otherValues.AstConfig.NodeType = null;
            this.otherValues.AstConfig.DefaultNodeCreator = () => new PassNode();
            NonTerminal nt_DATETIME_TIMESPAN_VALUES = new NonTerminal("DATETIME_TIMESPAN_VALUES_FOR_METADATA", typeof(PassNode));
            nt_DATETIME_TIMESPAN_VALUES.AstConfig.NodeType = null;
            nt_DATETIME_TIMESPAN_VALUES.AstConfig.DefaultNodeCreator = () => new PassNode();

            NonTerminal nt_EXPLICIT_CAST = new NonTerminal("EXPLICIT_CAST_FOR_METADATA", typeof(ExplicitCast));
            nt_EXPLICIT_CAST.AstConfig.NodeType = null;
            nt_EXPLICIT_CAST.AstConfig.DefaultNodeCreator = () => new ExplicitCast();

            NonTerminal nt_VALUES_WITH_ALIAS = new NonTerminal("VALUES_WITH_ALIAS_FOR_METADATA", typeof(ConstantValueWithAliasNode));
            nt_VALUES_WITH_ALIAS.AstConfig.NodeType = null;
            nt_VALUES_WITH_ALIAS.AstConfig.DefaultNodeCreator = () => new ConstantValueWithAliasNode();
            NonTerminal nt_LIST_OF_VALUES = new NonTerminal("LIST_OF_VALUES_FOR_METADATA", typeof(PlanNodeListNode));
            nt_LIST_OF_VALUES.AstConfig.NodeType = null;
            nt_LIST_OF_VALUES.AstConfig.DefaultNodeCreator = () => new PlanNodeListNode();
            NonTerminal nt_DATE_FUNCTIONS = new NonTerminal("DATE_FUNCTIONS_FOR_METADATA", typeof(DateFunctionNode));
            nt_DATE_FUNCTIONS.AstConfig.NodeType = null;
            nt_DATE_FUNCTIONS.AstConfig.DefaultNodeCreator = () => new DateFunctionNode();
            NonTerminal nt_UNARY_ARITHMETIC_EXPRESSION = new NonTerminal("UNARY_ARITHMETIC_EXPRESSION_FOR_METADATA", typeof(UnaryArithmeticExpressionNode));
            nt_UNARY_ARITHMETIC_EXPRESSION.AstConfig.NodeType = null;
            nt_UNARY_ARITHMETIC_EXPRESSION.AstConfig.DefaultNodeCreator = () => new UnaryArithmeticExpressionNode();
            NonTerminal nt_ARITHMETIC_EXPRESSION = new NonTerminal("ARITHMETIC_EXPRESSION_FOR_METADATA", typeof(ArithmeticExpressionNode));
            nt_ARITHMETIC_EXPRESSION.AstConfig.NodeType = null;
            nt_ARITHMETIC_EXPRESSION.AstConfig.DefaultNodeCreator = () => new ArithmeticExpressionNode();
            NonTerminal nt_PROJECTION_FUNCTIONS = new NonTerminal("PROJECTION_FUNCTION_FOR_METADATA", typeof(ProjectionFunctionNode));
            nt_PROJECTION_FUNCTIONS.AstConfig.NodeType = null;
            nt_PROJECTION_FUNCTIONS.AstConfig.DefaultNodeCreator = () => new ProjectionFunctionNode();
            NonTerminal nt_STRING_FUNCTIONS = new NonTerminal("STRING_FUNCTIONS_FOR_METADATA", typeof(StringFunctionNode));
            nt_STRING_FUNCTIONS.AstConfig.NodeType = null;
            nt_STRING_FUNCTIONS.AstConfig.DefaultNodeCreator = () => new StringFunctionNode();
            this.projectionValue = new NonTerminal("PROJECTION_VALUES_FOR_METADATA", typeof(PassNode));
            this.projectionValue.AstConfig.NodeType = null;
            this.projectionValue.AstConfig.DefaultNodeCreator = () => new PassNode();
            NonTerminal nt_MATH_FUNCTIONS = new NonTerminal("MATH_FUNCTIONS_FOR_METADATA", typeof(MathFunctionNode));
            nt_MATH_FUNCTIONS.AstConfig.NodeType = null;
            nt_MATH_FUNCTIONS.AstConfig.DefaultNodeCreator = () => new MathFunctionNode();
            NonTerminal nt_ISNULL_FUNCTION = new NonTerminal("ISNULL_FUNCTION_FOR_METADATA", typeof(IsNullFunction));
            nt_ISNULL_FUNCTION.AstConfig.NodeType = null;
            nt_ISNULL_FUNCTION.AstConfig.DefaultNodeCreator = () => new IsNullFunction();

            /* EXPRESIONES LÓGICAS */
            this.logicExpression.Rule = this.logicExpression + terminalAnd + this.logicExpression
                                    | this.logicExpression + terminalOr + this.logicExpression
                                    | terminalParentesisIz + this.logicExpression + terminalParentesisDer
                                    | terminalNot + terminalParentesisIz + this.logicExpression + terminalParentesisDer
                                    | nt_COMPARATIVE_EXPRESSION;
            /* **************************** */
            /* EXPRESIONES COMPARATIVAS */
            nt_COMPARATIVE_EXPRESSION.Rule = nt_ARITHMETIC_EXPRESSION + terminalIgualIgual + nt_ARITHMETIC_EXPRESSION
                                            | nt_ARITHMETIC_EXPRESSION + terminalNoIgual + nt_ARITHMETIC_EXPRESSION
                                            | nt_ARITHMETIC_EXPRESSION + terminalMayorIgual + nt_ARITHMETIC_EXPRESSION
                                            | nt_ARITHMETIC_EXPRESSION + terminalMayorQue + nt_ARITHMETIC_EXPRESSION
                                            | nt_ARITHMETIC_EXPRESSION + terminalMenorIgual + nt_ARITHMETIC_EXPRESSION
                                            | nt_ARITHMETIC_EXPRESSION + terminalMenorQue + nt_ARITHMETIC_EXPRESSION
                                            | nt_ARITHMETIC_EXPRESSION + terminalLike + terminalCadena
                                            | terminalParentesisIz + nt_COMPARATIVE_EXPRESSION + terminalParentesisDer
                                            | terminalNot + terminalParentesisIz + nt_COMPARATIVE_EXPRESSION + terminalParentesisDer
                                            | nt_ARITHMETIC_EXPRESSION;            
            /* **************************** */
            /* EXPRESIONES ARITMETICAS */
            nt_ARITHMETIC_EXPRESSION.Rule = nt_ARITHMETIC_EXPRESSION + terminalMenos + nt_ARITHMETIC_EXPRESSION
                                            | nt_UNARY_ARITHMETIC_EXPRESSION
                                            | terminalParentesisIz + nt_ARITHMETIC_EXPRESSION + terminalParentesisDer;
            /* **************************** */
            /* OPERACION ARITMETICA UNARIA */
            nt_UNARY_ARITHMETIC_EXPRESSION.Rule = terminalMenos + this.values
                                                    | terminalMas + this.values
                                                    | this.values
                                                    | terminalParentesisIz + nt_UNARY_ARITHMETIC_EXPRESSION + terminalParentesisDer;
            /* **************************** */

            /* PROJECTION VALUES */
            this.projectionValue.Rule = nt_PROJECTION_FUNCTIONS
                                        | nt_ARITHMETIC_EXPRESSION;
            /* **************************** */
            /* VALUES */
            this.values.Rule = this.numericValues
                                | this.nonConstantValues
                                | this.otherValues
                                | terminalDateTimeValue
                                | nt_EXPLICIT_CAST
                                | nt_STRING_FUNCTIONS
                                | nt_ISNULL_FUNCTION
                                | terminalParentesisIz + this.values + terminalParentesisDer;

            nt_EXPLICIT_CAST.Rule = terminalParentesisIz + terminalType + terminalParentesisDer + this.values;
            /* **************************** */
            /* NO CONSTANTES */
            this.nonConstantValues.Rule = terminalId;
            /* **************************** */
            /* CONSTANTES */            
            this.numericValues.Rule = terminalNumero
                                        | nt_DATE_FUNCTIONS
                                        | nt_MATH_FUNCTIONS;

            this.otherValues.Rule = terminalBool
                                | terminalNull
                                | terminalCadena;

            nt_DATETIME_TIMESPAN_VALUES.Rule = this.nonConstantValues /* verificar si se debe sustituir por nt_EXPLICIT_CAST */
                                            | terminalDateTimeValue;
            /* **************************** */
            /* FUNCIONES DE FECHAS */
            nt_DATE_FUNCTIONS.Rule = terminalYear + terminalParentesisIz + nt_DATETIME_TIMESPAN_VALUES + terminalParentesisDer
                                    | terminalMonth + terminalParentesisIz + nt_DATETIME_TIMESPAN_VALUES + terminalParentesisDer
                                    | terminalDay + terminalParentesisIz + nt_DATETIME_TIMESPAN_VALUES + terminalParentesisDer
                                    | terminalHour + terminalParentesisIz + nt_DATETIME_TIMESPAN_VALUES + terminalParentesisDer
                                    | terminalMinute + terminalParentesisIz + nt_DATETIME_TIMESPAN_VALUES + terminalParentesisDer
                                    | terminalSecond + terminalParentesisIz + nt_ARITHMETIC_EXPRESSION + terminalParentesisDer
                                    | terminalMillisecond + terminalParentesisIz + nt_DATETIME_TIMESPAN_VALUES + terminalParentesisDer;
            /* **************************** */
            /* FUNCIONES DE LA PROYECCION */
            nt_PROJECTION_FUNCTIONS.Rule = terminalCount + terminalParentesisIz + terminalParentesisDer
                                            | terminalSum + terminalParentesisIz + this.values + terminalParentesisDer
                                            | terminalMin + terminalParentesisIz + this.values + terminalParentesisDer
                                            | terminalMax + terminalParentesisIz + this.values + terminalParentesisDer;
            /* **************************** */
            /* FUNCIONES DE CADENA DE TEXTO */
            nt_STRING_FUNCTIONS.Rule = terminalLeft + terminalParentesisIz + this.values + terminalComa + terminalNumero + terminalParentesisDer
                                        | terminalRight + terminalParentesisIz + this.values + terminalComa + terminalNumero + terminalParentesisDer
                                        | terminalUpper + terminalParentesisIz + this.values + terminalParentesisDer
                                        | terminalLower + terminalParentesisIz + this.values + terminalParentesisDer;
            /* **************************** */
            /* FUNCIONES MATEMÁTICAS */
            nt_MATH_FUNCTIONS.Rule = terminalAbs + terminalParentesisIz + nt_ARITHMETIC_EXPRESSION + terminalParentesisDer;
            /* **************************** */
            /* FUNCIONES FUNCIONES DEL OBJETO */
            nt_ISNULL_FUNCTION.Rule = terminalIsnull + terminalParentesisIz + this.values  + terminalComa + this.values + terminalParentesisDer;
            /* **************************** */

            this.Root = this.logicExpression;
            this.LanguageFlags = Irony.Parsing.LanguageFlags.CreateAst;
        }
    }
}
