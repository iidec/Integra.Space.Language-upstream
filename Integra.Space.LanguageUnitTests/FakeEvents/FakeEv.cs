using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace ET_Test
{
    public class FakeEv
    {
        public long DT = 0;
        public DateTime Timestamp;
        public DateTime ProcessTime;
        private IObservable<bool> observableResultante;
        public bool matched = false;
        public long Id { get; set; }
        public string MessageType { get; set; }
        public string Pan { get; set; }
        public decimal TransactionAmount { get; set; }
        public string Ref { get; set; }
        public string Comercio { get; set; }
        public int UnmatchCount;
        public int MatchCount;

        TaskCompletionSource<bool> s = new TaskCompletionSource<bool>();

        public IObservable<bool> Start()
        {
            observableResultante = s.Task.ToObservable();
            return observableResultante;
        }

        public void Match()
        {
            try
            {
                if (!s.Task.IsCompleted)
                {
                    s.SetResult(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error at Match");
                var aaa = e;
            }
        }

        public void Timeout()
        {
            try
            {
                if(!s.Task.IsCompleted)
                {
                    s.SetResult(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error at Timeout");
                var aaa = e;
            }
        }
    }
}