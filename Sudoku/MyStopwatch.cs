using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class MyStopwatch : Stopwatch
    {
        public TimeSpan StartOffset { get; private set; }

        public MyStopwatch(TimeSpan startOffset)
        {
            StartOffset = startOffset;
        }

        public MyStopwatch()
        {
            StartOffset = new TimeSpan();
        }

        public new void Reset()
        {
            base.Reset();
            StartOffset = new TimeSpan();
        }

        public new TimeSpan Elapsed
        {
            get
            {
                return base.Elapsed.Add(StartOffset);
            }
        }

        public new long ElapsedMilliseconds
        {
            get
            {
                return base.ElapsedMilliseconds + (long)StartOffset.TotalMilliseconds;
            }
        }

        public new long ElapsedTicks
        {
            get
            {
                return base.ElapsedTicks + StartOffset.Ticks;
            }
        }
    }
}
