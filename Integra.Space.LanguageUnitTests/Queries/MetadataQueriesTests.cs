using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Integra.Space.Language.Runtime;
using Microsoft.Reactive.Testing;
using System.Linq;
using System.Reactive.Linq;
using System.Data.Entity;
using System.Reactive;
using System.Collections.Generic;

namespace Integra.Space.LanguageUnitTests.Queries
{
    [TestClass]
    public class MetadataQueriesTests
    {
        private IObservable<object> Process<T>(string eql, DefaultSchedulerFactory dsf, ITestableObservable<T> input)
        {
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;
            bool isTestMode = true;
            CompileContext context = new CompileContext() { PrintLog = printLog, QueryName = string.Empty, Scheduler = dsf, DebugMode = debugMode, MeasureElapsedTime = measureElapsedTime, IsTestMode = isTestMode };

            FakePipeline fp = new FakePipeline();
            //Func<IObservable<T>, IObservable<object>> func = (Func<IObservable<T>, IObservable<object>>)fp.ProcessWithMetadataQueryParser<T>(context, eql, dsf);
            //return func(input.AsObservable());
            Delegate d = fp.ProcessWithMetadataQueryParser<T>(context, eql, dsf);

            if (isTestMode)
            {
                return (IObservable<object>)d.DynamicInvoke(input.AsObservable(), dsf.TestScheduler);
            }
            else
            {
                return (IObservable<object>)d.DynamicInvoke(input.AsObservable());
            }

            Assembly assembly = null; //fp.ProcessWithMetadataQueryParser(context, eql, dsf);
            Type[] types = assembly.GetTypes();
            Type queryInfo = assembly.GetTypes().First(x => x.GetInterface("IQueryInformation") == typeof(IQueryInformation));
            IQueryInformation queryInfoObject = (IQueryInformation)Activator.CreateInstance(queryInfo);
            Type queryType = queryInfoObject.GetQueryType();
            object queryObject = Activator.CreateInstance(queryType);
            MethodInfo result = queryObject.GetType().GetMethod("MainFunction");

            return ((IObservable<object>)result.Invoke(queryObject, new object[] { input.AsObservable(), dsf.TestScheduler }));
        }

        [TestMethod]
        public void TestMethod1()
        {
            string command = "from Servers as x where (string)ServerId == \"59e858fc-c84d-48a7-8a98-c0e7adede20a\" select 1 as servId order by servId";

            command = @"cross 
                                 JOIN SpaceObservable1 as t1 WHERE ServerId == ""59e858fc-c84d-48a7-8a98-c0e7adede20a"" 
                                 WITH SpaceObservable2 as t2 
                                 ON ServerId == ServerId 
                                 SELECT 1 as c1, 2 as c2 ";

            command = "from Servers as x where (string)ServerId == \"59e858fc-c84d-48a7-8a98-c0e7adede20a\" select ServerId as servId, max(1) as maxTest order by desc servId, maxTest";
            command = "from Servers as x select ServerId as servId, max(1) as maxTest order by desc servId, maxTest";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            using (Space.Database.SpaceDbContext context = new Space.Database.SpaceDbContext())
            {
                ITestableObservable<Space.Database.Server> input = dsf.TestScheduler.CreateColdObservable(
                    this.GetRecoredList<Space.Database.Server>(context.Servers)
                );

                ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process<Space.Database.Server>(command, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        servId = (Guid)((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("servId").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)),
                        abc = (int)((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("maxTest").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0))
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

                ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(150, Notification.CreateOnNext((object)(new { servId = Guid.Parse("59e858fc-c84d-48a7-8a98-c0e7adede20a"), abc = 1 }))),
                    new Recorded<Notification<object>>(250, Notification.CreateOnCompleted<object>())
                });
            }
        }

        private Recorded<Notification<TItem>>[] GetRecoredList<TItem>(IEnumerable<TItem> objects)
        {
            List<Recorded<Notification<TItem>>> resultList = new List<Recorded<Notification<TItem>>>();
            foreach (TItem item in objects)
            {
                resultList.Add(new Recorded<Notification<TItem>>(100, Notification.CreateOnNext(item)));
            }

            resultList.Add(new Recorded<Notification<TItem>>(200, Notification.CreateOnCompleted<TItem>()));

            return resultList.ToArray();
        }
    }
}
