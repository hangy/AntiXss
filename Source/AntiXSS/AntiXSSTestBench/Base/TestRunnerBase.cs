using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Security.Application.AntiXSSTestBench
{
    public class TestRunnerBase
    {
        protected delegate void DoWork(string text);
        
        long _Start;
        long _Stop;
        double _Total = 0;
        double _TotalSecs;
        double _TotalAvgSecs;
        int _Repeats;
        protected int _RepeatsToDo; // = 1000;

        public TestRunnerBase(int repeats)
        {
            _RepeatsToDo = repeats;
        }

        private void Init()
        {
            _Total = 0;
            _TotalSecs = 0;
            _TotalAvgSecs = 0;
            _Repeats = _RepeatsToDo;
        }

        private void ReportResults()
        {
            _TotalSecs = _Total / 10000000;
            _TotalAvgSecs = (_Total / 10000000) / _RepeatsToDo;

            //Output.WriteLine("<AvgTime>" + _TotalAvgSecs + " Seconds</AvgTime>");
            Output.WriteLine("<totalTime>" + _TotalSecs + " Seconds</totalTime>");
            Output.WriteLine("----");
        }

        protected void RunBenchmark(DoWork func, string text)
        {
            if (null != func)
            {
                Init();
                while (_Repeats-- > 0)
                {
                    _Start = DateTime.Now.Ticks;
                    func(text);
                    _Stop = DateTime.Now.Ticks;
                    _Total += _Stop - _Start;
                }
                ReportResults();
            }
        }
    }
}
