using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language.Runtime;
using System.Dynamic;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive;
using Microsoft.Reactive.Testing;
using System.Reactive.Concurrency;

namespace Integra.Space.LanguageUnitTests.TypeBuilder
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestJoin()
        {
            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            var testDuration = Observable.Never<Unit>()
                .Timeout(TimeSpan.FromMilliseconds(2000))
                .Catch(Observable.Return<Unit>(Unit.Default));

            ReplaySubject<int> r1 = new ReplaySubject<int>();
            ReplaySubject<string> r2 = new ReplaySubject<string>();

            r1
            .Publish()
            .RefCount()
            .Subscribe<int>(e =>
            {
                Observable.Return(e)
                .Join(
                    r2,
                    _ => testDuration,
                    _ => testDuration,
                    (l, r) => Tuple.Create(l, r)
                    )
                //.Take(1)
                .Timeout(TimeSpan.FromMilliseconds(3000), Observable.Return(Tuple.Create<int, string>(e, null)), TaskPoolScheduler.Default)
                .Subscribe(result =>
                {
                    Console.WriteLine(result);
                },
                () =>
                {
                    // Console.WriteLine("Complete!");
                });
            });
        }
    }
}
