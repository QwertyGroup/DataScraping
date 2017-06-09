using BigDataCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            TestBase test = new ToolsTesting();
            test.Main();
        }
    }

    abstract class TestBase
    {
        public abstract void Main();
    }

    class ToolsTesting : TestBase
    {
        private ScrapingTools _coreTools = new ScrapingTools();

        public override void Main()
        {
            var pages = _coreTools.GenPageSequence(100, 4);
            var spl1 = "  abc <<->> cba         <<->> kekes     <<->>     ".SplitBy("<<->>");
            var spl2 = "  abc <<->> cba         <<->> kekes     <<->>     ".SplitByAndTrim("<<->>");
        }
    }
}
