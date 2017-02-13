using Integra.Space.LanguageUnitTests.TestObject;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Threading;

namespace Integra.Space.LanguageUnitTests.Helpers
{
    public class LoadTestsHelper
    {
        #region Locals

        private int cantEventos;
        private long whereDifference;
        private int porcentajeTimeouts;
        private const long MAX_TIMEOUT = 100000000; // TimeSpan.FromSeconds(10).Ticks;
        private long maxLimitTimeTest;
        private long timeout;
        private bool evaluateMatchedEvents;
        private List<Tuple<string, string, string, string, bool>> expectedResults;

        #region Randoms

        [ThreadStatic]
        static Random r1 = new Random();

        [ThreadStatic]
        static Random r2 = new Random();

        [ThreadStatic]
        static Random r3 = new Random();

        [ThreadStatic]
        static Random r4 = new Random();

        [ThreadStatic]
        static Random r5 = new Random();

        #endregion Randoms

        private static long reft = 0;

        #endregion Locals

        /// <summary>
        /// Initializes a new instance of the class <see cref="LoadTestsHelper"/> class.
        /// </summary>
        /// <param name="cantEventos">Number of eventos to generate</param>
        /// <param name="whereDifference"></param>
        /// <param name="timeout">Timeout in events in milliseconds.</param>
        /// <param name="maxLimitTimeTest">Limit maximum test duration in milliseconds.</param>
        /// <param name="porcentajeTimeouts">Timeouts percentage.</param>
        /// <param name="evaluateMatchedEvents"></param>
        public LoadTestsHelper(int cantEventos, int timeout, int whereDifference, int maxLimitTimeTest, int porcentajeTimeouts, bool evaluateMatchedEvents)
        {
            Contract.Requires(cantEventos > 0);
            Contract.Requires(whereDifference > 0 && whereDifference <= MAX_TIMEOUT && whereDifference < maxLimitTimeTest);
            Contract.Requires(timeout > 0 && timeout <= MAX_TIMEOUT && timeout < maxLimitTimeTest);
            Contract.Requires(maxLimitTimeTest > 0 && maxLimitTimeTest > whereDifference);
            Contract.Requires(porcentajeTimeouts >= 0 && porcentajeTimeouts <= 100);

            this.cantEventos = cantEventos;
            this.whereDifference = TimeSpan.FromMilliseconds(whereDifference).Ticks;
            this.timeout = TimeSpan.FromMilliseconds(timeout).Ticks;
            this.maxLimitTimeTest = TimeSpan.FromMilliseconds(maxLimitTimeTest - whereDifference).Ticks;
            this.porcentajeTimeouts = porcentajeTimeouts;
            this.evaluateMatchedEvents = evaluateMatchedEvents;
            this.expectedResults = new List<Tuple<string, string, string, string, bool>>();
        }

        #region Private methods

        private long LongRandom(long min, long max, Random rand)
        {
            long result = rand.Next((Int32)(min >> 32), (Int32)(max >> 32));
            result = (result << 32);
            result = result | (long)rand.Next((Int32)min, (Int32)max);
            return result;
        }

        private int CantidadTimeouts()
        {
            decimal r = ((decimal)this.porcentajeTimeouts / 100) * this.cantEventos;
            return (int)Decimal.Round(r, 0);
        }

        private int CantidadMatched()
        {
            return this.cantEventos - this.CantidadTimeouts();
        }

        public Tuple<Tuple<TestObject1, long>[], Tuple<TestObject1, long>[], Tuple<string, string, string, string, bool>[]> CreateEvents(JoinTypeEnum joinType)
        {
            DateTime now = DateTime.Now;
            int matches = this.CantidadMatched();
            int timeouts = this.CantidadTimeouts();
            long lowerLimit = TimeSpan.FromMilliseconds(10).Ticks;
            List<Tuple<TestObject1, long>> rq = new List<Tuple<TestObject1, long>>();
            List<Tuple<TestObject1, long>> rs = new List<Tuple<TestObject1, long>>();
            //List<Tuple<string, string, string, string, bool>> expectedResults = new List<Tuple<string, string, string, string, bool>>();
            Random rand = new Random();

            for (int i = 0; i < matches; i++)
            {
                string tarjeta = this.GenerateRandomPan();
                string referencia = this.GenerateVisaRetrievalReferenceNumber(now);
                long timeToAdd = this.LongRandom(0, this.maxLimitTimeTest, rand);

                // requests
                long relativeTimeRq = this.LongRandom(lowerLimit, this.whereDifference, rand) + timeToAdd; // momento en que fue generado
                long relativeSystemTimestampRq = this.LongRandom(lowerLimit, this.timeout, rand) + timeToAdd; // momento en que llegará al sistema
                DateTime rqSystemTimestamp = now.Add(TimeSpan.FromTicks(relativeTimeRq));
                rq.Add(Tuple.Create(this.GenerateEvent("0100", tarjeta, referencia, rqSystemTimestamp), relativeSystemTimestampRq));

                // responses
                long relativeTimeRs = this.LongRandom(lowerLimit, this.whereDifference, rand) + timeToAdd; // momento en que fue generado
                long relativeSystemTimestampRs = this.LongRandom(lowerLimit, this.timeout, rand) + timeToAdd; // momento en que llegará al sistema
                DateTime rsSystemTimestamp = now.Add(TimeSpan.FromTicks(relativeTimeRs));
                rs.Add(Tuple.Create(this.GenerateEvent("0110", tarjeta, referencia, rsSystemTimestamp), relativeSystemTimestampRs));
                
                var timeToAddTS = TimeSpan.FromTicks(timeToAdd);
                var rtrq = TimeSpan.FromTicks(relativeTimeRq);
                var rtsrq = TimeSpan.FromTicks(relativeSystemTimestampRq);
                var rtrs = TimeSpan.FromTicks(relativeTimeRs);
                var rtsrs = TimeSpan.FromTicks(relativeSystemTimestampRs);
                var time1 = TimeSpan.FromTicks(Math.Abs(relativeTimeRs - relativeTimeRq));
                var time2 = TimeSpan.FromTicks(Math.Abs(relativeSystemTimestampRs - relativeSystemTimestampRq));

                if (Math.Abs(relativeSystemTimestampRs - relativeSystemTimestampRq) >= this.timeout)
                {
                    throw new Exception("Diferencia inválida en eventos que deben coincidir.");
                }

                if (Math.Abs(relativeTimeRs - relativeTimeRq) > this.whereDifference)
                {
                    throw new Exception("Diferencia inválida en eventos que deben coincidir.");
                }

                this.AddToExpectedResults(true, tarjeta, referencia, tarjeta, referencia);
            }

            for (int i = 0; i < timeouts; i++)
            {
                string tarjeta = this.GenerateRandomPan();
                string referencia = this.GenerateVisaRetrievalReferenceNumber(DateTime.Now);
                long timeToAdd = this.LongRandom(0, this.maxLimitTimeTest, rand);

                // requests
                long relativeTimeRq = this.LongRandom(lowerLimit, this.whereDifference, rand) + timeToAdd;
                long relativeSystemTimestampRq = this.LongRandom(lowerLimit, this.timeout, rand) + timeToAdd;
                DateTime auxNowRq = now.Add(TimeSpan.FromTicks(relativeTimeRq));
                rq.Add(Tuple.Create(this.GenerateEvent("0100", tarjeta, referencia, auxNowRq), relativeSystemTimestampRq));

                // responses
                long relativeTimeRs = this.LongRandom(this.timeout + TimeSpan.FromMilliseconds(10).Ticks /* mas 10 milisegundos */, this.whereDifference + MAX_TIMEOUT, rand) + timeToAdd;
                long relativeSystemTimestampRs = this.LongRandom(this.timeout + relativeSystemTimestampRq + TimeSpan.FromMilliseconds(10).Ticks /* mas 10 milisegundos */, this.timeout + relativeSystemTimestampRq + MAX_TIMEOUT, rand) + timeToAdd;
                DateTime auxNowRs = now.Add(TimeSpan.FromTicks(relativeTimeRs));
                rs.Add(Tuple.Create(this.GenerateEvent("0110", tarjeta, referencia, auxNowRs), relativeSystemTimestampRs));

                if (Math.Abs(relativeSystemTimestampRs - relativeSystemTimestampRq) <= this.timeout)
                {
                    throw new Exception("Diferencia inválida en eventos que NO deben coincidir.");
                }

                if (Math.Abs(relativeTimeRs - relativeTimeRq) <= this.whereDifference)
                {
                    this.AddToExpectedResults(false, tarjeta, referencia, tarjeta, referencia);
                }
                else
                {
                    if (joinType.Equals(JoinTypeEnum.Cross))
                    {
                        // expectedResults.Add(Tuple.Create<string, string, string, string, bool>(tarjeta, referencia, null, null, false));
                        this.AddToExpectedResults(false, tarjeta, referencia, null, null);
                        //expectedResults.Add(Tuple.Create<string, string, string, string, bool>(null, null, tarjeta, referencia, false));
                        this.AddToExpectedResults(false, null, null, tarjeta, referencia);
                    }
                    else if (joinType.Equals(JoinTypeEnum.Inner))
                    {
                        //expectedResults.Add(Tuple.Create<string, string, string, string, bool>(null, null, null, null, false));
                        this.AddToExpectedResults(false, null, null, null, null);
                    }
                    else if (joinType.Equals(JoinTypeEnum.Left))
                    {
                        //expectedResults.Add(Tuple.Create<string, string, string, string, bool>(tarjeta, referencia, null, null, false));
                        this.AddToExpectedResults(false, tarjeta, referencia, null, null);
                    }
                    else if (joinType.Equals(JoinTypeEnum.Right))
                    {
                        //expectedResults.Add(Tuple.Create<string, string, string, string, bool>(null, null, tarjeta, referencia, false));
                        this.AddToExpectedResults(false, null, null, tarjeta, referencia);
                    }
                }
            }

            return Tuple.Create(rq.ToArray(), rs.ToArray(), expectedResults.ToArray());
        }

        private void AddToExpectedResults(bool fromMatched, string l1, string l2, string r1, string r2)
        {
            if (this.evaluateMatchedEvents == fromMatched)
            {
                this.expectedResults.Add(Tuple.Create<string, string, string, string, bool>(l1, l2, r1, r2, false));
            }
        }

        private string GenerateVisaRetrievalReferenceNumber(DateTime date)
        {
            //Esta referencia debe componerse por 12 caracteres
            //4 caracteres. Del dia actual en formato YDDD, donde Y es el ultimo digito del año. DDD es el dia del año.
            string today = string.Format("{0}{1:d3}", date.Year % 10, date.DayOfYear);
            //Tomo el tick de la fecha, los ultimos 6 y lo relleno con ceros hasta llegar a 8
            //se toman solo los ultimos 6 para que coincidan con los 6 del campo 11 System trace number
            string ticks = string.Format("{0:D6}", date.Ticks + Interlocked.Increment(ref reft));
            ticks = ticks.Substring(ticks.Length - 6).PadLeft(8, '0'); //Tomo lo ultimos 6 y relleno con '0' para llegar a 8;
                                                                       //8 del dia actual en formato HHMMSSMS, HH=Horas MM=Minutos SS=Segundos MS=Milisegundos.
                                                                       //string time = string.Format("{0:d2}{1:d2}{2:d2}{3:d2}", date.Hour, date.Minute, date.Second, date.Millisecond);
                                                                       //return string.Format("{0}{1}", today, time);
            return string.Format("{0}{1}", today, ticks);
        }

        private string GenerateRandomPan()
        {
            if (r1 == null)
            {
                r1 = new Random();
            }
            if (r2 == null)
            {
                r2 = new Random();
            }
            if (r3 == null)
            {
                r3 = new Random();
            }
            if (r4 == null)
            {
                r4 = new Random();
            }

            return RandomGen3.Next(1, 1000).ToString().PadLeft(4, '0') + RandomGen3.Next(1, 1000).ToString().PadLeft(4, '0') + RandomGen3.Next(1, 1000).ToString().PadLeft(4, '0') + RandomGen3.Next(1, 1000).ToString().PadLeft(4, '0');
        }

        private TestObject1 GenerateEvent(string messageType, string tarjeta, string referencia, DateTime systemTimeStamp)
        {
            return new TestObject1(messageType: messageType, primaryAccountNumber: tarjeta, retrievalReferenceNumber: referencia)
            {
                SourceTimestamp = systemTimeStamp
            };

            /*
             return new EventObject(new string[] { string.Empty })
            {
                SourceTimestamp = systemTimeStamp,
                Message = new Messaging.Message()
                {
                    new Messaging.MessagePart(0, "Header")
                    {
                        new Messaging.MessageField(0, "MessageType") { Value = messageType }
                    },
                    new Messaging.MessagePart(1, "Body")
                    {
                        new Messaging.MessageField(0, "PrimaryAccountNumber") { Value = tarjeta },
                        new Messaging.MessageField(1, "RetrievalReferenceNumber") { Value = referencia }
                    }
                }
            };
             */
        }

        #endregion Private methods
        
        private static class RandomGen3
        {
            private static RNGCryptoServiceProvider _global = new RNGCryptoServiceProvider();

            [ThreadStatic]
            private static Random _local;

            public static int Next(int min, int max)
            {
                Random inst = _local;
                if (inst == null)
                {
                    //Console.WriteLine("Creando... {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
                    byte[] buffer = new byte[4];
                    _global.GetBytes(buffer);
                    _local = inst = new Random(BitConverter.ToInt32(buffer, 0));
                }
                return inst.Next(min, max);
            }
        }
    }
}
