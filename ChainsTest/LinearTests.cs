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
    /// Tests all synchronous functionality of the Chains library
    /// </summary>
    [TestClass]
    public class LinearTests
    {
        public LinearTests()
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
        public void TestSimpleChain()
        {
            // Create a new chain with 4 elements
            Chain_Accessor<int, int> chain = new Chain_Accessor<int, int>();
            chain.Link<int>(x => x + 1)
            .Link<int>(x => x + 1)
            .Link<int>(x => x + 1)
            .Link<int>(x => x + 1);

            Assert.AreEqual<int>(4, chain.index);

            int output = chain.Execute(0);
            Assert.AreEqual<int>(4, output);
        }

        [TestMethod]
        [DeploymentItem("Chains.dll")]
        public void TestTypeMismatch()
        {
            Chain<int, int> chain = new Chain<int, int>();
            chain.Link<int>(x => x + 1)
                .Link<string>(x => x)
                .Link<int>(x => x);

            LinkArgumentException laErr = null;
            try
            {
                int output = chain.Execute(0);
            }
            catch (LinkArgumentException lae)
            {
                laErr = lae;
            }

            Assert.IsNotNull(laErr);
        }

        [TestMethod]
        [DeploymentItem("Chains.dll")]
        public void TestExecutionError()
        {
            Chain<int, int> chain = new Chain<int, int>()
            .Link<int>(x => x)
            .Link<int>(x =>
            {
                if (x == 0)
                    throw new NullReferenceException(x.ToString());
                else
                    return x;
            })
            .Link<int>(x => 100);


            ChainExecutionException chEE = null;
            try
            {
                int outcome = chain.Execute(0);
            }
            catch (ChainExecutionException chee)
            {
                chEE = chee;
            }

            Assert.IsNotNull(chEE);
            Assert.AreEqual<string>("0", chEE.InnerException.Message);
        }

        [TestMethod]
        [DeploymentItem("Chains.dll")]
        public void TestMultiTypeChain()
        {
            Chain<int, string> chain = new Chain<int, string>()
            .Link<int>(x => Enumerable.Range(1, x).ToArray())
            .Link<int[]>(x => x.Sum())
            .Link<int>(x => x.ToString());

            string output = chain.Execute(5);

            Assert.AreEqual<string>("15", output);
        }
    }
}
