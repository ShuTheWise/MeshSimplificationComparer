using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace MeshSimplificationComparer
{
    public class TimeReportCollection<T> : Collection<T> where T : ITimeReporter
    {
        public TimeReportCollection(IList<T> list) : base(list)
        {
        }

        public struct Enumerator : IEnumerator<T>
        {
            private T[] array;
            private int currentIndex;
            private Stopwatch stopwatch;

            public Enumerator(IEnumerable<T> myCollection)
            {
                array = myCollection.ToArray();
                currentIndex = -1;
                Current = default;
                stopwatch = new Stopwatch();
            }

            public T Current { get; private set; }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (currentIndex == -1)
                {
                    stopwatch = Stopwatch.StartNew();
                }

                if (currentIndex > -1)
                {
                    Current.time = stopwatch.ElapsedMilliseconds;
                    Logger.Write($"Elapsed Time:\t{stopwatch.ElapsedMilliseconds} ms\n");
                    stopwatch.Restart();
                }

                if (++currentIndex >= array.Length)
                {
                    return false;
                }
                else
                {
                    // Set current element to next item in collection.
                    Current = array[currentIndex];
                    Logger.Write(Current.startMessage);
                }
                //if (Current != default)
                //{
                //}
                return true;
            }

            public void Reset()
            {
                currentIndex = -1;
            }
        }

        public new IEnumerator<T> GetEnumerator() => new Enumerator(this);
    }
}