using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chains;
using System.Threading;

namespace ChainsTest
{
    /// <summary>
    /// Tests asynchronous functionality of the Chains library
    /// </summary>
    [TestClass]
    public class AsyncTests
    {
        public AsyncTests()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        [DeploymentItem("Chains.dll")]
        public void TestAsyncExecution()
        {
            Chain<int, int> chain = new Chain<int, int>()
            .Link<int>(x => x + 1)
            .Link<int>(x => x + 1)
            .Link<int>(x => x + 1)
            .Link<int>(x => x + 1);

            int output = -1;
            chain.ExecuteFinished += (sender, e) => output = e.Output;

            chain.ExecuteAsync(0);

            SpinWait.SpinUntil(() => output != -1, 250);

            Assert.AreEqual<int>(4, output);
        }

        [TestMethod]
        [DeploymentItem("Chains.dll")]
        public void TestAsyncTypeError()
        {
            Chain<int, int> chain = new Chain<int, int>()
            .Link<int>(x => x + 1)
            .Link<int>(x => x + "1")
            .Link<int>(x => x + 1)
            .Link<int>(x => x + 1);

            Exception ex = null;
            chain.ExecuteFinished += (sender, e) =>
            {
                ex = e.Exception;
            };

            chain.ExecuteAsync(0);

            SpinWait.SpinUntil(() => ex != null, 250);

            Assert.IsNotNull(ex);
            Assert.AreEqual<bool>(true, ex is LinkArgumentException);
        }

        [TestMethod]
        [DeploymentItem("Chains.dll")]
        public void TestAsyncExecutionError()
        {
            Chain<int, int> chain = new Chain<int, int>()
            .Link<int>(x => x + 1)
            .Link<int>(x =>
            {
                if (x == 1)
                    throw new Exception(x.ToString());
                else
                    return x + 1;
            })
            .Link<int>(x => x + 1)
            .Link<int>(x => x + 1);

            Exception ex = null;
            chain.ExecuteFinished += (sender, e) =>
            {
                ex = e.Exception;
            };

            chain.ExecuteAsync(0);

            SpinWait.SpinUntil(() => ex != null, 250);

            Assert.IsNotNull(ex);
            Assert.AreEqual<bool>(true, ex is ChainExecutionException);
        }
    }
}
