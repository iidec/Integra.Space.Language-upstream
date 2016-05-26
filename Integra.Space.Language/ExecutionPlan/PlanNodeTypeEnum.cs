//-----------------------------------------------------------------------
// <copyright file="PlanNodeTypeEnum.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System;

    /// <summary>
    /// PlanType enumerable
    /// </summary>
    internal enum PlanNodeTypeEnum : uint
    {
        /// <summary>
        /// Constant type
        /// </summary>
        Constant = 10,
        
        /// <summary>
        /// DateTimeFunction type
        /// </summary>
        DateTimeFunction = 11,

        /// <summary>
        /// Identifier type
        /// </summary>
        Identifier = 12,

        /// <summary>
        /// String left function type
        /// </summary>
        StringLeftFunction = 13,

        /// <summary>
        /// String right function type
        /// </summary>
        StringRightFunction = 14,

        /// <summary>
        /// String upper function type
        /// </summary>
        StringUpperFunction = 15,

        /// <summary>
        /// String lower function type
        /// </summary>
        StringLowerFunction = 16,
        
        /// <summary>
        /// Math function type
        /// </summary>
        MathFunctionWithOneParameter = 17,

        /// <summary>
        /// IsNull function type
        /// </summary>
        IsNullFunction = 18,

        /// <summary>
        /// Cast type
        /// </summary>
        Cast = 21,

        /// <summary>
        /// Equal type
        /// </summary>
        Equal = 31,

        /// <summary>
        /// NotEqual type
        /// </summary>
        NotEqual = 32,

        /// <summary>
        /// LessThan type
        /// </summary>
        LessThan = 33,

        /// <summary>
        /// LessThanOrEqual type
        /// </summary>
        LessThanOrEqual = 34,

        /// <summary>
        /// GreaterThan type
        /// </summary>
        GreaterThan = 35,

        /// <summary>
        /// GreaterThanOrEqual type
        /// </summary>
        GreaterThanOrEqual = 36,

        /// <summary>
        /// Not type
        /// </summary>
        Not = 37,

        /// <summary>
        /// Like type
        /// </summary>
        Like = 38,

        /// <summary>
        /// Or type
        /// </summary>
        Or = 50,

        /// <summary>
        /// And type
        /// </summary>
        And = 51,

        /// <summary>
        /// Event type
        /// </summary>
        Event = 60,

        /// <summary>
        /// Timestamp type
        /// </summary>
        Timestamp = 61,

        /// <summary>
        /// Name type
        /// </summary>
        Name = 62,

        /// <summary>
        /// Adapter type
        /// </summary>
        Adapter = 63,

        /// <summary>
        /// Agent type
        /// </summary>
        Agent = 64,

        /// <summary>
        /// Source type
        /// </summary>
        Source = 70,

        /// <summary>
        /// ObjectPart type
        /// </summary>
        ObjectPart = 71,

        /// <summary>
        /// ObjectField type
        /// </summary>
        ObjectField = 72,

        /// <summary>
        /// ObjectValue type
        /// </summary>
        ObjectValue = 73,

        /// <summary>
        /// ObjectMessage type
        /// </summary>
        ObjectMessage = 74,

        /// <summary>
        /// Object prefix type
        /// </summary>
        ObjectPrefix = 76,

        /// <summary>
        /// Object property value type
        /// </summary>
        ObjectPropertyValue = 77,

        /// <summary>
        /// Group key type
        /// </summary>
        GroupKey = 80,

        /// <summary>
        /// Group key property type
        /// </summary>
        GroupKeyProperty = 81,

        /// <summary>
        /// Group key value type
        /// </summary>
        GroupKeyValue = 82,

        /// <summary>
        /// Negate type
        /// </summary>
        Negate = 90,

        /// <summary>
        /// Subtract type
        /// </summary>
        Subtract = 91,

        /// <summary>
        /// Property type
        /// </summary>
        Property = 100,

        /// <summary>
        /// Event property type
        /// </summary>
        EventProperty = 101,

        /// <summary>
        /// TupleProjection type
        /// </summary>
        TupleProjection = 109,

        /// <summary>
        /// On type
        /// </summary>
        On = 113,

        /// <summary>
        /// Left join type
        /// </summary>
        LeftJoin = 114,

        /// <summary>
        /// Right join type
        /// </summary>
        RightJoin = 115,

        /// <summary>
        /// Cross join type
        /// </summary>
        CrossJoin = 116,

        /// <summary>
        /// Inner join type
        /// </summary>
        InnerJoin = 117,

        /// <summary>
        /// With type
        /// </summary>
        With = 118,

        /// <summary>
        /// Timeout type
        /// </summary>
        Timeout = 119,
        
        /// <summary>
        /// Event life time type
        /// </summary>
        EventLifeTime = 120,

        /// <summary>
        /// ValueWithAlias type
        /// </summary>
        ValueWithAlias = 140,

        /// <summary>
        /// ValueWithoutAlias type
        /// </summary>
        ValueWithoutAlias = 141,

        /// <summary>
        /// If type
        /// </summary>
        IfHasEvents = 130,

        /// <summary>
        /// Send type
        /// </summary>
        Send = 131,

        /// <summary>
        /// If not type
        /// </summary>
        IfNotHasEvents = 132,

        /// <summary>
        /// Status type
        /// </summary>
        Status = 133,

        /// <summary>
        /// Create assembly type.
        /// </summary>
        CreateAssembly = 150,

        /// <summary>
        /// Create adapter type.
        /// </summary>
        CreateAdapter = 151,

        /// <summary>
        /// Create source type.
        /// </summary>
        CreateSource = 152,

        /// <summary>
        /// Create join stream type.
        /// </summary>
        CreateStream = 153,

        /// <summary>
        /// Create simple trigger type.
        /// </summary>
        CreateTrigger = 154,

        /// <summary>
        /// Create user type.
        /// </summary>
        CreateUser = 155,

        /// <summary>
        /// Create role type.
        /// </summary>
        CreateRole = 156,

        /// <summary>
        /// Drop assembly type.
        /// </summary>
        DropAssembly = 160,

        /// <summary>
        /// Drop adapter type.
        /// </summary>
        DropAdapter = 161,

        /// <summary>
        /// Drop source type.
        /// </summary>
        DropSource = 162,

        /// <summary>
        /// Drop stream type.
        /// </summary>
        DropStream = 163,

        /// <summary>
        /// Drop trigger type.
        /// </summary>
        DropTrigger = 164,

        /// <summary>
        /// Drop user type.
        /// </summary>
        DropUser = 165,

        /// <summary>
        /// Drop role type.
        /// </summary>
        DropRole = 166,

        /// <summary>
        /// Alter adapter type.
        /// </summary>
        AlterAdapter = 170,

        /// <summary>
        /// Alter source type.
        /// </summary>
        AlterSource = 171,

        /// <summary>
        /// Alter stream type.
        /// </summary>
        AlterStream = 172,

        /// <summary>
        /// Alter trigger type.
        /// </summary>
        AlterTrigger = 173,

        /// <summary>
        /// Alter user type.
        /// </summary>
        AlterUser = 174,

        /// <summary>
        /// Alter assembly type
        /// </summary>
        AlterAssembly = 175,

        /// <summary>
        /// Start assembly type.
        /// </summary>
        StartAssembly = 180,

        /// <summary>
        /// Start adapter type.
        /// </summary>
        StartAdapter = 181,

        /// <summary>
        /// Start source type.
        /// </summary>
        StartSource = 182,

        /// <summary>
        /// Start stream type.
        /// </summary>
        StartStream = 183,

        /// <summary>
        /// Start trigger type.
        /// </summary>
        StartTrigger = 184,

        /// <summary>
        /// Start user type.
        /// </summary>
        StartUser = 185,

        /// <summary>
        /// Start role type.
        /// </summary>
        StartRole = 186,

        /// <summary>
        /// Stop assembly type.
        /// </summary>
        StopAssembly = 187,

        /// <summary>
        /// Stop adapter type.
        /// </summary>
        StopAdapter = 188,

        /// <summary>
        /// Stop source type.
        /// </summary>
        StopSource = 189,

        /// <summary>
        /// Stop stream type.
        /// </summary>
        StopStream = 190,

        /// <summary>
        /// Stop trigger type.
        /// </summary>
        StopTrigger = 191,

        /// <summary>
        /// Stop user type.
        /// </summary>
        StopUser = 192,

        /// <summary>
        /// Stop role type.
        /// </summary>
        StopRole = 193,        

        /// <summary>
        /// Set trace adapter type.
        /// </summary>
        SetTraceAdapter = 200,

        /// <summary>
        /// Set trace source type.
        /// </summary>
        SetTraceSource = 201,

        /// <summary>
        /// Set trace stream type.
        /// </summary>
        SetTraceStream = 202,

        /// <summary>
        /// Set trace trigger type.
        /// </summary>
        SetTraceTrigger = 203,

        /// <summary>
        /// Set trace engine type.
        /// </summary>
        SetTraceEngine = 204,
        
        /// <summary>
        /// Set trace to specific object type
        /// </summary>
        SetTraceObject = 205,

        /// <summary>
        /// Grant type
        /// </summary>
        Grant = 210,

        /// <summary>
        /// Deny type
        /// </summary>
        Deny = 211,

        /// <summary>
        /// Revoke type
        /// </summary>
        Revoke = 212,

        /// <summary>
        /// System query type.
        /// </summary>
        SystemQuery = 220,

        /// <summary>
        /// Boot engine type
        /// </summary>
        BootEngine = 230,

        /// <summary>
        /// Load assembly type
        /// </summary>
        LoadAssembly = 231,

        /// <summary>
        /// Receive type
        /// </summary>
        Receive = 241,

        /// <summary>
        /// Publish type
        /// </summary>
        Publish = 242,

        /// <summary>
        /// User query type
        /// </summary>
        UserQuery = 250,

        /// <summary>
        /// From observable type
        /// </summary>
        ObservableFrom = 500,

        /// <summary>
        /// From for lambda expression
        /// </summary>
        ObservableFromForLambda = 501,

        /// <summary>
        /// Where observable type
        /// </summary>
        ObservableWhere = 502,

        /// <summary>
        /// Window observable type
        /// </summary>
        ObservableWindow = 503,

        /// <summary>
        /// Group by observable type
        /// </summary>
        ObservableGroupBy = 504,

        /// <summary>
        /// Select Observable type
        /// </summary>
        ObservableSelect = 505,

        /// <summary>
        /// Select many observable type
        /// </summary>
        ObservableSelectManyForWindow = 506,

        /// <summary>
        /// Select many for group by observable type
        /// </summary>
        ObservableSelectManyForGroupBy = 507,

        /// <summary>
        /// Count observable type
        /// </summary>
        ObservableCount = 508,

        /// <summary>
        /// Select for group by observable type
        /// </summary>
        ObservableSelectForGroupBy = 509,

        /// <summary>
        /// Select for buffer observable type
        /// </summary>
        ObservableSelectForObservableBufferOrSource = 510,

        /// <summary>
        /// Buffer observable type
        /// </summary>
        ObservableBuffer = 511,
        
        /// <summary>
        /// Projection type
        /// </summary>
        Projection = 512,

        /// <summary>
        /// Observable merge type
        /// </summary>
        ObservableMerge = 513,

        /// <summary>
        /// Observable buffer time and size type
        /// </summary>
        ObservableBufferTimeAndSize = 514,

        /// <summary>
        /// Enumerable take type
        /// </summary>
        ObservableTake = 515,

        /// <summary>
        /// Observable select for event lock type
        /// </summary>
        ObservableWhereForEventLock = 516,

        /// <summary>
        /// Observable create type
        /// </summary>
        ObservableCreate = 517,

        /// <summary>
        /// Observable timeout type
        /// </summary>
        ObservableTimeout = 518,

        /// <summary>
        /// Observable catch type
        /// </summary>
        ObservableCatch = 519,

        /// <summary>
        /// Observable never type
        /// </summary>
        ObservableNever = 520,

        /// <summary>
        /// Observable empty type
        /// </summary>
        ObservableEmpty = 521,

        /// <summary>
        /// Observable join type
        /// </summary>
        ObservableJoin = 522,

        /// <summary>
        /// Observable publish type
        /// </summary>
        ObservablePublish = 523,

        /// <summary>
        /// Observable ref-count type
        /// </summary>
        ObservableRefCount = 524,

        /// <summary>
        /// Observable extracted event data type
        /// </summary>
        ObservableExtractedEventData = 525,

        /// <summary>
        /// Observable extracted event data comparer type
        /// </summary>
        ObservableExtractedEventDataComparer = 526,

        /// <summary>
        /// Observable switch type
        /// </summary>
        ObservableSwitch = 527,

        /// <summary>
        /// Observable concatenate type
        /// </summary>
        ObservableConcat = 528,

        /// <summary>
        /// Enumerable count type
        /// </summary>
        EnumerableCount = 613,

        /// <summary>
        /// Enumerable sum type
        /// </summary>
        EnumerableSum = 614,

        /// <summary>
        /// Enumerable select for enumerable type
        /// </summary>
        EnumerableSelectForEnumerable = 615,

        /// <summary>
        /// Enumerable select for group by type
        /// </summary>
        EnumerableSelectForGroupBy = 616,

        /// <summary>
        /// Enumerable select for group by
        /// </summary>
        EnumerableGroupBy = 617,

        /// <summary>
        /// Enumerable order by type
        /// </summary>
        EnumerableOrderBy = 618,

        /// <summary>
        /// Enumerable order by descending type
        /// </summary>
        EnumerableOrderByDesc = 619,

        /// <summary>
        /// Enumerable max type
        /// </summary>
        EnumerableMax = 620,

        /// <summary>
        /// Enumerable min type
        /// </summary>
        EnumerableMin = 621,

        /// <summary>
        /// Enumerable take type
        /// </summary>
        EnumerableTake = 622,

        /// <summary>
        /// Enumerable to list type
        /// </summary>
        EnumerableToList = 623,

        /// <summary>
        /// Enumerable where type
        /// </summary>
        EnumerableWhere = 624,

        /// <summary>
        /// Enumerable join type
        /// </summary>
        EnumerableJoin = 625,

        /// <summary>
        /// Enumerable to array type
        /// </summary>
        EnumerableToArray = 626,

        /// <summary>
        /// Enumerable to observable type
        /// </summary>
        EnumerableToObservable = 627,

        /// <summary>
        /// New Scope type
        /// </summary>
        NewScope = 700,
        
        /// <summary>
        /// Projection of constants type, projection for group by, this does not create a lambda expression
        /// </summary>
        ProjectionOfConstants = 701,

        /// <summary>
        /// Select for the query result
        /// </summary>
        SelectForResult = 702,

        /// <summary>
        /// Projection for the query result select
        /// </summary>
        SelectForResultProjection = 703,

        /// <summary>
        /// Join left duration type
        /// </summary>
        JoinLeftDuration = 704,

        /// <summary>
        /// Join right duration type
        /// </summary>
        JoinRightDuration = 705,

        /// <summary>
        /// Join result selector
        /// </summary>
        JoinResultSelector = 706,

        /// <summary>
        /// Join projection type
        /// </summary>
        JoinProjection = 707,

        /// <summary>
        /// Key selector projection
        /// </summary>
        KeySelectorProjection = 708,

        /// <summary>
        /// Subscription type
        /// </summary>
        Subscription = 709,

        /// <summary>
        /// Buffer size for join type.
        /// </summary>
        BufferSizeForJoin = 710
    }
}