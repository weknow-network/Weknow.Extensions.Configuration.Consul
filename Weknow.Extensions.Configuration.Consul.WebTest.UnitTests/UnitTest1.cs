using System;
using Xunit;
using Xunit.Abstractions;

namespace Weknow.Extensions.Configuration.Consul.WebTest.UnitTests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _outputHelper;

        #region Ctor

        public UnitTest1(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        #endregion // Ctor

        [Fact]
        public void Test1()
        {

        }
    }
}
