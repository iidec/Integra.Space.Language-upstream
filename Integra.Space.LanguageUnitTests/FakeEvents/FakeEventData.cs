using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET_Test
{
    public enum FakeEventDataStateEnum
    {
        Created,
        Matched,
        Expired
    }

    public class FakeEventData
    {
        public FakeEventData()
        {
            this.InEvaluation = false;
            this.State = FakeEventDataStateEnum.Created;
        }

        public bool InEvaluation { get; set; }

        public DateTime Timestamp { get; set; }

        public FakeEventDataStateEnum State {
            get;
            private set;
        }

        /// <summary>
        /// Indicates whether the event matched with another event. 0 = false, 1 = true
        /// </summary>
        private long matched = 0;

        /// <summary>
        /// Indicates whether the event don't matched with another event. 0 = false, 1 = true
        /// </summary>
        private long timeout = 0;

        /// <summary>
        /// Object to lock the change of the match value
        /// </summary>
        private static object lockState = new object();

        public bool SetState(FakeEventDataStateEnum state)
        {
            lock(lockState)
            {
                if (state == FakeEventDataStateEnum.Matched && (this.State == FakeEventDataStateEnum.Created || this.State == FakeEventDataStateEnum.Matched))
                {
                    this.State = state;
                    return true;
                }


                if (state == FakeEventDataStateEnum.Expired && this.State == FakeEventDataStateEnum.Created)
                {
                    this.State = state;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Object to lock the change of the match value
        /// </summary>
        private static object lockMatch = new object();

        /// <summary>
        /// Object to lock the change of the timeout value
        /// </summary>
        private static object lockTimeout = new object();

        /// <summary>
        /// Gets a value indicating whether the event matched with another event. 0 = false, 1 = true
        /// </summary>
        public bool HasMatched
        {
            get
            {
                if (System.Threading.Interlocked.Read(ref matched) == 1)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the event don't matched with another event. 0 = false, 1 = true
        /// </summary>
        public bool HasTimeOut
        {
            get
            {
                if (System.Threading.Interlocked.Read(ref timeout) == 1)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Set the matched value of the event to true
        /// </summary>
        public void SetMatched()
        {
            lock (lockMatch)
            {
                if (timeout == 0)
                {
                    matched = 1;
                }
            }

            /*if (System.Threading.Interlocked.Read(ref timeout) == 0)
            {
                System.Threading.Interlocked.Exchange(ref matched, 1);
            }*/
        }

        /// <summary>
        /// Set the timeout value of the event to true
        /// </summary>
        public void SetTimeout()
        {
            lock (lockTimeout)
            {
                if (matched == 0)
                {
                    timeout = 1;
                }
            }

            /*if (System.Threading.Interlocked.Read(ref matched) == 0)
            {
                System.Threading.Interlocked.Exchange(ref timeout, 1);
            }*/
        }

        /*
        /// <summary>
        /// Doc goes here.
        /// </summary>
        /// <param name="obj">Hello world.</param>
        /// <returns>True value</returns>
        public override bool Equals(object obj)
        {
            System.Diagnostics.Debug.WriteLine("&&&&& Equals {0} {1}", this.GetType().Name, obj.GetType().Name);
            //System.Diagnostics.Debug.WriteLine(System.Environment.StackTrace);
            return true;
        }

        /// <summary>
        /// Doc goes here.
        /// </summary>
        /// <returns>Cero value.</returns>
        public override int GetHashCode()
        {
            System.Diagnostics.Debug.WriteLine(string.Format("&&&&& GetHashCode {0}", this.GetType().Name));
            //System.Diagnostics.Debug.WriteLine(System.Environment.StackTrace);
            return 0;
        }
        */
    }
}
