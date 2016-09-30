//-----------------------------------------------------------------------
// <copyright file="SpaceParseTreeNodeTypeEnum.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// Parse tree node type enumerable
    /// </summary>
    public enum SpaceParseTreeNodeTypeEnum
    {
        /// <summary>
        /// Command node type
        /// </summary>
        COMMAND_NODE = 0,

        /// <summary>
        /// user query type
        /// </summary>
        USER_QUERY = 1,

        /// <summary>
        /// Metadata query type.
        /// </summary>
        METADATA_QUERY = 2,

        /// <summary>
        /// Id or id with alias type
        /// </summary>
        ID_OR_ID_WITH_ALIAS = 10,
                
        /// <summary>
        /// source definition type
        /// </summary>
        SOURCE_DEFINITION = 19,

        /// <summary>
        /// NonTerminal From type
        /// </summary>
        FROM = 20,

        /// <summary>
        /// Join source type
        /// </summary>
        JOIN_SOURCE = 21,

        /// <summary>
        /// With type
        /// </summary>
        WITH = 22,
        
        /// <summary>
        /// where type
        /// </summary>
        WHERE = 23,

        /// <summary>
        /// optional group by type
        /// </summary>
        GROUP_BY_OP = 24,

        /// <summary>
        /// apply window type
        /// </summary>
        APPLY_WINDOW = 25,

        /// <summary>
        /// order by type
        /// </summary>
        ORDER_BY = 26,

        /// <summary>
        /// list of values type
        /// </summary>
        LIST_OF_VALUES = 27,

        /// <summary>
        /// values with alias type
        /// </summary>
        VALUES_WITH_ALIAS = 28,

        /// <summary>
        /// id with alias type 
        /// </summary>
        ID_WITH_ALIAS = 29,

        /// <summary>
        /// join type
        /// </summary>
        JOIN = 30,

        /// <summary>
        /// on type
        /// </summary>
        ON = 31,

        /// <summary>
        /// timeout type
        /// </summary>
        TIMEOUT = 32,

        /// <summary>
        /// join type type
        /// </summary>
        JOIN_TYPE = 33,

        /// <summary>
        /// event life time type
        /// </summary>
        EVENT_LIFE_TIME = 34,

        /// <summary>
        /// logic expression
        /// </summary>
        LOGIC_EXPRESSION = 35,

        /// <summary>
        /// comparative expression type
        /// </summary>
        COMPARATIVE_EXPRESSION = 36,

        /// <summary>
        /// arithmetic expression type
        /// </summary>
        ARITHMETIC_EXPRESSION = 37,

        /// <summary>
        /// unary arithmetic expression type
        /// </summary>
        UNARY_ARITHMETIC_EXPRESSION = 38,

        /// <summary>
        /// values type
        /// </summary>
        VALUE = 40,

        /// <summary>
        /// non constant values type
        /// </summary>
        NON_CONSTANT_VALUES = 41,

        /// <summary>
        /// object value type
        /// </summary>
        OBJECT_VALUE = 42,

        /// <summary>
        /// object type
        /// </summary>
        OBJECT = 43,

        /// <summary>
        /// event type
        /// </summary>
        EVENT = 44,

        /// <summary>
        /// numeric values type
        /// </summary>
        NUMERIC_VALUES = 45,

        /// <summary>
        /// other values type
        /// </summary>
        OTHER_VALUES = 46,

        /// <summary>
        /// date time timespan values type
        /// </summary>
        DATETIME_TIMESPAN_VALUES = 47,

        /// <summary>
        /// explicit cast type
        /// </summary>
        EXPLICIT_CAST = 48,

        /// <summary>
        /// date functions type
        /// </summary>
        DATE_FUNCTIONS = 49,

        /// <summary>
        /// group key type
        /// </summary>
        GROUP_KEY = 50,

        /// <summary>
        /// group key value type
        /// </summary>
        GROUP_KEY_VALUE = 51,

        /// <summary>
        /// event values type
        /// </summary>
        EVENT_PROPERTY_VALUE = 62,

        /// <summary>
        /// event with source type
        /// </summary>
        EVENT_WITH_SOURCE = 63,

        /// <summary>
        /// object id or number type
        /// </summary>
        OBJECT_ID_OR_NUMBER = 64,
        
        /// <summary>
        /// projection function type
        /// </summary>
        PROJECTION_FUNCTION = 65,

        /// <summary>
        /// string function type
        /// </summary>
        STRING_FUNCTIONS = 66,

        /// <summary>
        /// projection values type
        /// </summary>
        PROJECTION_VALUES = 67,

        /// <summary>
        /// optional id type
        /// </summary>
        OPTIONAL_ID = 68,

        /// <summary>
        /// select type
        /// </summary>
        SELECT = 69,

        /// <summary>
        /// Top type
        /// </summary>
        TOP = 70,

        /// <summary>
        /// Group by values type
        /// </summary>
        GROUP_BY_VALUES = 71,

        /// <summary>
        /// Group by type
        /// </summary>
        GROUP_BY = 72,

        /// <summary>
        /// identifier node type
        /// </summary>
        identifier = 100,

        /// <summary>
        /// number type
        /// </summary>
        number = 101,

        /// <summary>
        /// string type
        /// </summary>
        @string = 102,

        /// <summary>
        /// constant boolean type
        /// </summary>
        constantBool = 103,

        /// <summary>
        /// date time value type
        /// </summary>
        datetimeValue = 104,

        /// <summary>
        /// constant null type
        /// </summary>
        constantNull = 105,

        /// <summary>
        /// where type
        /// </summary>
        where = 110,

        /// <summary>
        /// apply type
        /// </summary>
        apply = 111,

        /// <summary>
        /// window type
        /// </summary>
        window = 112,

        /// <summary>
        /// of type
        /// </summary>
        of = 113,

        /// <summary>
        /// order type
        /// </summary>
        order = 114,

        /// <summary>
        /// ascendant order type
        /// </summary>
        asc = 115,

        /// <summary>
        /// descendant order type
        /// </summary>
        desc = 116,

        /// <summary>
        /// by type
        /// </summary>
        by = 117,

        /// <summary>
        /// as type
        /// </summary>
        @as = 118,

        /// <summary>
        /// left type
        /// </summary>
        left = 119,

        /// <summary>
        /// right type
        /// </summary>
        right = 120,

        /// <summary>
        /// cross type
        /// </summary>
        cross = 121,

        /// <summary>
        /// inner type
        /// </summary>
        inner = 122,

        /// <summary>
        /// join type
        /// </summary>
        join = 123,

        /// <summary>
        /// on type
        /// </summary>
        on = 124,

        /// <summary>
        /// timeout type
        /// </summary>
        timeout = 125,

        /// <summary>
        /// with type
        /// </summary>
        with = 126,

        /// <summary>
        /// event life time type
        /// </summary>
        eventlifetime = 127,

        /// <summary>
        /// line comment type
        /// </summary>
        line_coment = 128,

        /// <summary>
        /// block comment type
        /// </summary>
        block_coment = 129,

        /// <summary>
        /// '==' symbol type
        /// </summary>
        equalEqual = 130,

        /// <summary>
        /// key type
        /// </summary>
        key = 131,

        /// <summary>
        /// year type
        /// </summary>
        year = 132,

        /// <summary>
        /// month type
        /// </summary>
        month = 133,

        /// <summary>
        /// day type
        /// </summary>
        day = 134,

        /// <summary>
        /// hour type
        /// </summary>
        hour = 135,

        /// <summary>
        /// minute type
        /// </summary>
        minute = 136,

        /// <summary>
        /// millisecond type
        /// </summary>
        millisecond = 137,

        /// <summary>
        /// count type
        /// </summary>
        count = 138,

        /// <summary>
        /// sum type
        /// </summary>
        sum = 139,

        /// <summary>
        /// min type
        /// </summary>
        min = 140,

        /// <summary>
        /// max type
        /// </summary>
        max = 141,

        /// <summary>
        /// upper type
        /// </summary>
        upper = 142,

        /// <summary>
        /// lower type
        /// </summary>
        lower = 143,

        /// <summary>
        /// timestamp type
        /// </summary>
        Timestamp = 144,

        /// <summary>
        /// agent property type
        /// </summary>
        Agent = 145,

        /// <summary>
        /// adapter property type
        /// </summary>
        Adapter = 146,

        /// <summary>
        /// name property type
        /// </summary>
        Name = 147,

        /// <summary>
        /// like type
        /// </summary>
        like = 148,

        /// <summary>
        /// not type
        /// </summary>
        not = 149,

        /// <summary>
        /// and type
        /// </summary>
        and = 150,

        /// <summary>
        /// or type
        /// </summary>
        or = 151,

        /// <summary>
        /// minus type
        /// </summary>
        menos = 152,

        /// <summary>
        /// plus type
        /// </summary>
        plus = 153,

        /// <summary>
        /// not equal type
        /// </summary>
        notEqual = 154,

        /// <summary>
        /// greater than or equal type
        /// </summary>
        greaterThanOrEqual = 155,

        /// <summary>
        /// less than or equal type
        /// </summary>
        lessThanOrEqual = 156,

        /// <summary>
        /// greater than type
        /// </summary>
        greaterThan = 157,

        /// <summary>
        /// less than type
        /// </summary>
        lessThan = 158,

        /// <summary>
        /// data types type
        /// </summary>
        dataTypes = 159,

        /// <summary>
        /// left parenthesis type
        /// </summary>
        leftParenthesis = 160,

        /// <summary>
        /// right parenthesis type
        /// </summary>
        rightParenthesis = 161,

        /// <summary>
        /// left bracket type
        /// </summary>
        leftBracket = 162,

        /// <summary>
        /// right bracket type
        /// </summary>
        rightBracket = 163,

        /// <summary>
        /// dot symbol type
        /// </summary>
        dot = 164,

        /// <summary>
        /// single quote type
        /// </summary>
        singleQuote = 165,

        /// <summary>
        /// coma type
        /// </summary>
        coma = 166,

        /// <summary>
        /// group type
        /// </summary>
        group = 167,

        /// <summary>
        /// top type
        /// </summary>
        top = 168,

        /// <summary>
        /// select type
        /// </summary>
        select = 169,

        /// <summary>
        /// '@' symbol type
        /// </summary>
        arroba = 170,

        /// <summary>
        /// event type
        /// </summary>
        @event = 171,

        /// <summary>
        /// message type
        /// </summary>
        Message = 172,

        /// <summary>
        /// sharp symbol type
        /// </summary>
        sharp = 173,

        /// <summary>
        /// terminal from type
        /// </summary>
        from = 174,

        /// <summary>
        /// logic expression
        /// </summary>
        LOGIC_EXPRESSION_FOR_ON_CONDITION = 175,

        /// <summary>
        /// comparative expression type
        /// </summary>
        COMPARATIVE_EXPRESSION_FOR_ON_CONDITION = 176,

        /// <summary>
        /// Explicit cast for on condition type.
        /// </summary>
        EXPLICIT_CAST_FOR_ON_CONDITION = 177,

        /// <summary>
        /// Doc goes here.
        /// </summary>
        LOGIC_EXPRESSION_FOR_METADATA = 178,

        /// <summary>
        /// Doc goes here.
        /// </summary>
        COMPARATIVE_EXPRESSION_FOR_METADATA = 179,

        /// <summary>
        /// Doc goes here.
        /// </summary>
        ARITHMETIC_EXPRESSION_FOR_METADATA = 180,

        /// <summary>
        /// Doc goes here.
        /// </summary>
        UNARY_ARITHMETIC_EXPRESSION_FOR_METADATA = 181,

        /// <summary>
        /// Doc goes here.
        /// </summary>
        VALUE_FOR_METADATA = 182,

        /// <summary>
        /// Doc goes here.
        /// </summary>
        EXPLICIT_CAST_FOR_METADATA = 183,

        /// <summary>
        /// Doc goes here.
        /// </summary>
        NON_CONSTANT_VALUES_FOR_METADATA = 184,

        /// <summary>
        /// Doc goes here.
        /// </summary>
        OTHER_VALUES_FOR_METADATA = 185,

        /// <summary>
        /// Doc goes here.
        /// </summary>
        PROJECTION_VALUES_FOR_METADATA = 186,

        /// <summary>
        /// Doc goes here.
        /// </summary>
        PROJECTION_FUNCTION_FOR_METADATA = 187,

        /// <summary>
        /// Doc goes here.
        /// </summary>
        NUMERIC_VALUES_FOR_METADATA = 188
    }
}
