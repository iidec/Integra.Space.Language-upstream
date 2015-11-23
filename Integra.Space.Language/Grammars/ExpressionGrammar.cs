//-----------------------------------------------------------------------
// <copyright file="ExpressionGrammar.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Grammars
{
    using System;
    using System.Linq;
    using ASTNodes.Cast;
    using ASTNodes.Constants;
    using ASTNodes.Lists;
    using ASTNodes.Objects.Event;
    using ASTNodes.Objects.Object;
    using ASTNodes.Operations;
    using ASTNodes.QuerySections;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// EQLGrammar grammar for the commands and the predicates 
    /// </summary>
    [Language("ExpressionsGrammar", "0.1", "")]
    internal sealed class ExpressionGrammar : InterpretedLanguageGrammar
    {
        /// <summary>
        /// Grammar to add the expression rules
        /// </summary>
        private NonTerminal logicExpression;

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
        /// Initializes a new instance of the <see cref="ExpressionGrammar"/> class
        /// </summary>
        public ExpressionGrammar() : base(false)
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
        /// Gets the nonterminal for numeric values
        /// </summary>
        public NonTerminal NumericValues
        {
            get
            {
                return this.numericValues;
            }
        }

        /// <summary>
        /// Gets the nonterminal for non constant values
        /// </summary>
        public NonTerminal NonConstantValues
        {
            get
            {
                return this.nonConstantValues;
            }
        }

        /// <summary>
        /// Gets the nonterminal for other values
        /// </summary>
        public NonTerminal OtherValues
        {
            get
            {
                return this.otherValues;
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

            /* EVENTOS */
            KeyTerm terminalEvent = ToTerm("event", "event");
            KeyTerm terminalMessage = ToTerm("Message", "Message");
            KeyTerm terminalTimestamp = ToTerm("Timestamp", "Timestamp");
            KeyTerm terminalAgent = ToTerm("Agent", "Agent");
            KeyTerm terminalAdapter = ToTerm("Adapter", "Adapter");
            KeyTerm terminalName = ToTerm("Name", "Name");

            /* OPERADORES */
            KeyTerm terminalLike = ToTerm("like", "like");
            KeyTerm terminalNot = ToTerm("not", "not");
            KeyTerm terminalIn = ToTerm("in", "in");
            KeyTerm terminalAnd = ToTerm("and", "and");
            KeyTerm terminalOr = ToTerm("or", "or");

            // Marcamos los terminales, definidos hasta el momento, como palabras reservadas
            this.MarkReservedWords(this.KeyTerms.Keys.ToArray());

            /* OPERADORES ARITMETICOS */
            KeyTerm terminalMenos = ToTerm("-", "menos");
            KeyTerm terminalMas = ToTerm("+", "mas");

            /* OPERADORES COMPARATIVOS */
            KeyTerm terminalIgualIgual = ToTerm("==", "igualIgual");
            KeyTerm terminalNoIgual = ToTerm("!=", "noIgual");
            KeyTerm terminalMayorIgual = ToTerm(">=", "mayorIgual");
            KeyTerm terminalMenorIgual = ToTerm("<=", "menorIgual");
            KeyTerm terminalMayorQue = ToTerm(">", "mayorQue");
            KeyTerm terminalMenorQue = ToTerm("<", "menorQue");

            /* TIPOS PARA CASTEO */
            ConstantTerminal terminalType = new ConstantTerminal("contanteTipo");
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
            terminalType.Add("TimeSpan", typeof(TimeSpan));
            terminalType.AstConfig.NodeType = null;
            terminalType.AstConfig.DefaultNodeCreator = () => new TypeNode();

            /* SIMBOLOS */
            KeyTerm terminalParentesisIz = ToTerm("(", "parentesisIzExpressionGrammar");
            KeyTerm terminalParentesisDer = this.ToTerm(")", "parentesisDerValuesGrammar");
            KeyTerm terminalCorcheteIz = ToTerm("[", "corcheteIz");
            KeyTerm terminalCorcheteDer = ToTerm("]", "corcheteDer");
            KeyTerm terminalPunto = ToTerm(".", "punto");
            KeyTerm terminalNumeral = ToTerm("#", "numeral");
            KeyTerm terminalComillaSimple = ToTerm("'", "comillaSimple");
            KeyTerm terminalComa = ToTerm(",", "coma");
            KeyTerm terminalArroba = ToTerm("@", "arroba");

            /* COMENTARIOS */
            CommentTerminal comentarioLinea = new CommentTerminal("comentario_linea", "//", "\n", "\r\n");
            CommentTerminal comentarioBloque = new CommentTerminal("comentario_bloque", "/*", "*/");
            NonGrammarTerminals.Add(comentarioLinea);
            NonGrammarTerminals.Add(comentarioBloque);

            /* CONSTANTES E IDENTIFICADORES */
            Terminal terminalNumero = TerminalFactory.CreateCSharpNumber("numero");
            terminalNumero.AstConfig.NodeType = null;
            terminalNumero.AstConfig.DefaultNodeCreator = () => new NumberNode();
            Terminal terminalCadena = TerminalFactory.CreateCSharpString("cadena");
            terminalCadena.AstConfig.NodeType = null;
            terminalCadena.AstConfig.DefaultNodeCreator = () => new StringNode();
            ConstantTerminal terminalBool = new ConstantTerminal("constanteBool");
            terminalBool.Add("true", true);
            terminalBool.Add("false", false);
            terminalBool.AstConfig.NodeType = null;
            terminalBool.AstConfig.DefaultNodeCreator = () => new BooleanNode();
            Terminal terminalDateTimeValue = new QuotedValueLiteral("datetimeValue", "'", TypeCode.String);
            terminalDateTimeValue.AstConfig.NodeType = null;
            terminalDateTimeValue.AstConfig.DefaultNodeCreator = () => new DateTimeOrTimespanNode();
            ConstantTerminal terminalNull = new ConstantTerminal("constanteNull");
            terminalNull.Add("null", null);
            terminalNull.AstConfig.NodeType = null;
            terminalNull.AstConfig.DefaultNodeCreator = () => new NullValueNode();

            RegexBasedTerminal terminalId = new RegexBasedTerminal("[a-zA-Z]+([a-zA-Z]|[0-9]|[_])*");
            terminalId.AstConfig.NodeType = null;
            terminalId.AstConfig.DefaultNodeCreator = () => new IdentifierNode();

            /* PRECEDENCIA Y ASOCIATIVIDAD */
            this.RegisterBracePair("(", ")");
            this.RegisterBracePair("[", "]");
            this.RegisterOperators(40, Associativity.Right, terminalParentesisIz, terminalParentesisDer, terminalCorcheteIz, terminalCorcheteDer);
            this.RegisterOperators(30, Associativity.Right, terminalIgualIgual, terminalNoIgual, terminalMayorIgual, terminalMayorQue, terminalMenorIgual, terminalMenorQue, terminalLike, terminalIn);
            this.RegisterOperators(20, Associativity.Right, terminalAnd);
            this.RegisterOperators(10, Associativity.Right, terminalOr);
            this.RegisterOperators(5, Associativity.Right, terminalNot);
            this.MarkPunctuation(terminalParentesisIz, terminalParentesisDer, terminalCorcheteIz, terminalCorcheteDer, terminalPunto, terminalComa);

            /* NO TERMINALES */
            NonTerminal nt_COMPARATIVE_EXPRESSION = new NonTerminal("COMPARATIVE_EXPRESSION", typeof(ComparativeExpressionNode));
            nt_COMPARATIVE_EXPRESSION.AstConfig.NodeType = null;
            nt_COMPARATIVE_EXPRESSION.AstConfig.DefaultNodeCreator = () => new ComparativeExpressionNode();
            this.logicExpression = new NonTerminal("LOGIC_EXPRESSION", typeof(LogicExpressionNode));
            this.logicExpression.AstConfig.NodeType = null;
            this.logicExpression.AstConfig.DefaultNodeCreator = () => new LogicExpressionNode();
            this.values = new NonTerminal("VALUES", typeof(ConstantValueNode));
            this.values.AstConfig.NodeType = null;
            this.values.AstConfig.DefaultNodeCreator = () => new ConstantValueNode();
            this.numericValues = new NonTerminal("NUMERIC_VALUES", typeof(ConstantValueNode));
            this.numericValues.AstConfig.NodeType = null;
            this.numericValues.AstConfig.DefaultNodeCreator = () => new ConstantValueNode();
            this.nonConstantValues = new NonTerminal("NON_CONSTANT_VALUES", typeof(ConstantValueNode));
            this.nonConstantValues.AstConfig.NodeType = null;
            this.nonConstantValues.AstConfig.DefaultNodeCreator = () => new ConstantValueNode();
            this.otherValues = new NonTerminal("OTHER_VALUES", typeof(ConstantValueNode));
            this.otherValues.AstConfig.NodeType = null;
            this.otherValues.AstConfig.DefaultNodeCreator = () => new ConstantValueNode();
            NonTerminal nt_DATETIME_TIMESPAN_VALUES = new NonTerminal("DATETIME_TIMESPAN_VALUES", typeof(ConstantValueNode));
            nt_DATETIME_TIMESPAN_VALUES.AstConfig.NodeType = null;
            nt_DATETIME_TIMESPAN_VALUES.AstConfig.DefaultNodeCreator = () => new ConstantValueNode();

            NonTerminal nt_EXPLICIT_CAST = new NonTerminal("EXPLICIT_CAST", typeof(ExplicitCast));
            nt_EXPLICIT_CAST.AstConfig.NodeType = null;
            nt_EXPLICIT_CAST.AstConfig.DefaultNodeCreator = () => new ExplicitCast();

            NonTerminal nt_PARAMETER_VALUES = new NonTerminal("PARAMETER_VALUES", typeof(ConstantValueNode));
            nt_PARAMETER_VALUES.AstConfig.NodeType = null;
            nt_PARAMETER_VALUES.AstConfig.DefaultNodeCreator = () => new ConstantValueNode();
            NonTerminal nt_VALUES_WITH_ALIAS = new NonTerminal("VALUES_WITH_ALIAS", typeof(ConstantValueWithAliasNode));
            nt_VALUES_WITH_ALIAS.AstConfig.NodeType = null;
            nt_VALUES_WITH_ALIAS.AstConfig.DefaultNodeCreator = () => new ConstantValueWithAliasNode();
            NonTerminal nt_LIST_OF_VALUES = new NonTerminal("LIST_OF_VALUES", typeof(PlanNodeListNode));
            nt_LIST_OF_VALUES.AstConfig.NodeType = null;
            nt_LIST_OF_VALUES.AstConfig.DefaultNodeCreator = () => new PlanNodeListNode();
            NonTerminal nt_DATE_FUNCTIONS = new NonTerminal("DATE_FUNCTIONS", typeof(DateFunctionNode));
            nt_DATE_FUNCTIONS.AstConfig.NodeType = null;
            nt_DATE_FUNCTIONS.AstConfig.DefaultNodeCreator = () => new DateFunctionNode();
            NonTerminal nt_EVENT = new NonTerminal("EVENT", typeof(EventNode));
            nt_EVENT.AstConfig.NodeType = null;
            nt_EVENT.AstConfig.DefaultNodeCreator = () => new EventNode();
            NonTerminal nt_GROUP_KEY = new NonTerminal("GROUP_KEY", typeof(GroupKeyNode));
            nt_GROUP_KEY.AstConfig.NodeType = null;
            nt_GROUP_KEY.AstConfig.DefaultNodeCreator = () => new GroupKeyNode();
            NonTerminal nt_GROUP_KEY_VALUE = new NonTerminal("GROUP_KEY_VALUE", typeof(GroupKeyValueNode));
            nt_GROUP_KEY_VALUE.AstConfig.NodeType = null;
            nt_GROUP_KEY_VALUE.AstConfig.DefaultNodeCreator = () => new GroupKeyValueNode();
            NonTerminal nt_EVENT_PROPERTIES = new NonTerminal("EVENT_VALUES", typeof(EventPropertiesNode));
            nt_EVENT_PROPERTIES.AstConfig.NodeType = null;
            nt_EVENT_PROPERTIES.AstConfig.DefaultNodeCreator = () => new EventPropertiesNode();
            NonTerminal nt_EVENT_WITH_SOURCE = new NonTerminal("EVENT_WITH_SOURCE", typeof(EventWithSource));
            nt_EVENT_WITH_SOURCE.AstConfig.NodeType = null;
            nt_EVENT_WITH_SOURCE.AstConfig.DefaultNodeCreator = () => new EventWithSource();
            NonTerminal nt_OBJECT_ID_OR_NUMBER = new NonTerminal("OBJECT_ID_OR_NUMBER", typeof(ObjectIdOrNumberNode));
            nt_OBJECT_ID_OR_NUMBER.AstConfig.NodeType = null;
            nt_OBJECT_ID_OR_NUMBER.AstConfig.DefaultNodeCreator = () => new ObjectIdOrNumberNode();
            NonTerminal nt_OBJECT = new NonTerminal("OBJECT", typeof(ObjectNode));
            nt_OBJECT.AstConfig.NodeType = null;
            nt_OBJECT.AstConfig.DefaultNodeCreator = () => new ObjectNode();
            NonTerminal nt_OBJECT_VALUE = new NonTerminal("OBJECT_VALUE", typeof(ObjectValueNode));
            nt_OBJECT_VALUE.AstConfig.NodeType = null;
            nt_OBJECT_VALUE.AstConfig.DefaultNodeCreator = () => new ObjectValueNode();
            NonTerminal nt_UNARY_ARITHMETIC_EXPRESSION = new NonTerminal("UNARY_ARITHMETIC_EXPRESSION", typeof(UnaryArithmeticExpressionNode));
            nt_UNARY_ARITHMETIC_EXPRESSION.AstConfig.NodeType = null;
            nt_UNARY_ARITHMETIC_EXPRESSION.AstConfig.DefaultNodeCreator = () => new UnaryArithmeticExpressionNode();
            NonTerminal nt_ARITHMETIC_EXPRESSION = new NonTerminal("ARITHMETIC_EXPRESSION", typeof(ArithmeticExpressionNode));
            nt_ARITHMETIC_EXPRESSION.AstConfig.NodeType = null;
            nt_ARITHMETIC_EXPRESSION.AstConfig.DefaultNodeCreator = () => new ArithmeticExpressionNode();
            NonTerminal nt_PROJECTION_FUNCTIONS = new NonTerminal("PROJECTION_FUNCTION", typeof(ProjectionFunctionNode));
            nt_PROJECTION_FUNCTIONS.AstConfig.NodeType = null;
            nt_PROJECTION_FUNCTIONS.AstConfig.DefaultNodeCreator = () => new ProjectionFunctionNode();
            NonTerminal nt_STRING_FUNCTIONS = new NonTerminal("STRING_FUNCTIONS", typeof(StringFunctionNode));
            nt_STRING_FUNCTIONS.AstConfig.NodeType = null;
            nt_STRING_FUNCTIONS.AstConfig.DefaultNodeCreator = () => new StringFunctionNode();
            this.projectionValue = new NonTerminal("PROJECTION_VALUES", typeof(ProjectionValueNode));
            this.projectionValue.AstConfig.NodeType = null;
            this.projectionValue.AstConfig.DefaultNodeCreator = () => new ProjectionValueNode();

            /* EXPRESIONES LÓGICAS */
            this.logicExpression.Rule = this.logicExpression + terminalAnd + this.logicExpression
                                    | this.logicExpression + terminalOr + this.logicExpression
                                    | terminalParentesisIz + this.logicExpression + terminalParentesisDer
                                    | terminalNot + terminalParentesisIz + this.logicExpression + terminalParentesisDer
                                    | nt_COMPARATIVE_EXPRESSION;
            /* **************************** */
            /* EXPRESIONES COMPARATIVAS */
            nt_COMPARATIVE_EXPRESSION.Rule = this.values + terminalIgualIgual + this.values
                                            | this.values + terminalNoIgual + this.values
                                            | this.values + terminalMayorIgual + this.values
                                            | this.values + terminalMayorQue + this.values
                                            | this.values + terminalMenorIgual + this.values
                                            | this.values + terminalMenorQue + this.values
                                            | this.values + terminalLike + terminalCadena
                                            | terminalParentesisIz + nt_COMPARATIVE_EXPRESSION + terminalParentesisDer
                                            | terminalNot + terminalParentesisIz + nt_COMPARATIVE_EXPRESSION + terminalParentesisDer
                                            | this.values;
            /* **************************** */
            
            /* PROJECTION VALUES */
            this.projectionValue.Rule = nt_PROJECTION_FUNCTIONS
                                        | nt_GROUP_KEY_VALUE
                                        | this.values
                                        | terminalId;
            /* **************************** */
            /* GROUP KEY */
            nt_GROUP_KEY.Rule = nt_GROUP_KEY + terminalPunto + terminalId
                                | terminalKey;

            nt_GROUP_KEY_VALUE.Rule = nt_GROUP_KEY;
            /* **************************** */
            /* CONSTANTES */
            this.values.Rule = this.numericValues
                                | this.nonConstantValues
                                | this.otherValues
                                | nt_EXPLICIT_CAST
                                | nt_STRING_FUNCTIONS;

            nt_EXPLICIT_CAST.Rule = terminalParentesisIz + terminalType + terminalParentesisDer + this.values;

            this.nonConstantValues.Rule = nt_EVENT_WITH_SOURCE
                                            | nt_OBJECT_VALUE
                                            | nt_EVENT_PROPERTIES;

            this.numericValues.Rule = terminalDateTimeValue
                                        | terminalNumero
                                        | nt_DATE_FUNCTIONS
                                        | nt_ARITHMETIC_EXPRESSION
                                        | nt_UNARY_ARITHMETIC_EXPRESSION;

            this.otherValues.Rule = terminalBool
                                | terminalNull
                                | terminalCadena;

            nt_DATETIME_TIMESPAN_VALUES.Rule = this.nonConstantValues
                                            | terminalDateTimeValue;

            /* **************************** */
            /* FUNCIONES DE FECHAS */
            nt_DATE_FUNCTIONS.Rule = terminalYear + terminalParentesisIz + nt_DATETIME_TIMESPAN_VALUES + terminalParentesisDer
                                    | terminalMonth + terminalParentesisIz + nt_DATETIME_TIMESPAN_VALUES + terminalParentesisDer
                                    | terminalDay + terminalParentesisIz + nt_DATETIME_TIMESPAN_VALUES + terminalParentesisDer
                                    | terminalHour + terminalParentesisIz + nt_DATETIME_TIMESPAN_VALUES + terminalParentesisDer
                                    | terminalMinute + terminalParentesisIz + nt_DATETIME_TIMESPAN_VALUES + terminalParentesisDer
                                    | terminalSecond + terminalParentesisIz + nt_DATETIME_TIMESPAN_VALUES + terminalParentesisDer
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
            /* EXPRESIONES ARITMETICAS */
            nt_ARITHMETIC_EXPRESSION.Rule = nt_ARITHMETIC_EXPRESSION + terminalMenos + nt_ARITHMETIC_EXPRESSION
                                            | this.numericValues;
            /* **************************** */
            /* OPERACION ARITMETICA UNARIA */
            nt_UNARY_ARITHMETIC_EXPRESSION.Rule = terminalMenos + terminalNumero
                                                    | terminalMas + terminalNumero;
            /* **************************** */
            /* VALORES DE LOS OBJETOS */
            nt_OBJECT_VALUE.Rule = nt_OBJECT;
            /* **************************** */
            /* OBJETOS */
            nt_OBJECT.Rule = nt_OBJECT + terminalPunto + nt_OBJECT_ID_OR_NUMBER
                                | nt_EVENT + terminalPunto + terminalMessage + terminalPunto + nt_OBJECT_ID_OR_NUMBER + terminalPunto + nt_OBJECT_ID_OR_NUMBER;
            /* **************************** */
            /* IDENTIFICADORES DE PARTES Y CAMPOS DE OBJETOS */
            nt_OBJECT_ID_OR_NUMBER.Rule = terminalNumeral + terminalNumero
                                        | terminalId
                                        | terminalCorcheteIz + terminalCadena + terminalCorcheteDer;
            /* **************************** */
            /* VALORES DEL EVENTO */
            nt_EVENT_PROPERTIES.Rule = nt_EVENT_PROPERTIES + terminalPunto + terminalId
                                        | nt_EVENT + terminalPunto + terminalAdapter
                                        | nt_EVENT + terminalPunto + terminalAgent;
            /* **************************** */
            /* EVENTO */
            nt_EVENT.Rule = terminalArroba + terminalEvent;
            /* **************************** */
            /* EVENTO CON FUENTE */
            nt_EVENT_WITH_SOURCE.Rule = terminalId + terminalPunto + nt_OBJECT_VALUE
                                            | terminalId + terminalPunto + nt_EVENT_PROPERTIES;
            /* **************************** */

            this.Root = this.logicExpression;
            this.LanguageFlags = Irony.Parsing.LanguageFlags.CreateAst;
        }
    }
}
