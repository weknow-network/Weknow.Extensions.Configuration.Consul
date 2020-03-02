using Newtonsoft.Json.Linq;
using System;
using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;
using static Weknow.Extensions.Configuration.Consul.JsonUtils;

// https://stackoverflow.com/questions/58694837/system-text-json-merge-two-objects

namespace Weknow.Extensions.Configuration.Consul.UnitTests
{
    /// <summary>
    /// The needs for JSON merge is for override general path setting with specific one
    /// </summary>
    public class TextJsonMergeTests
    {
        private readonly ITestOutputHelper _outputHelper;

        #region Ctor

        public TextJsonMergeTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        #endregion // Ctor

        [Fact]
        public void JsonMerge_Test()
        {
            #region Json

            var dataObject1 = @"{
                ""data"": [{
                    ""field"": ""field1""
                }],
                ""paging"": {
                    ""prev"": ""link1""
                }
            }";
            var dataObject2 = @"{
                ""data"": [{
                    ""id"": ""id2"",
                    ""field"": ""field2""
                }],
                ""paging"": {
                    ""prev"": ""link2""
                }
            }";

            #endregion // Jsons

            var merged = Merge(dataObject1, dataObject2);

            _outputHelper.WriteLine(merged);
            string expected = @"
{
  ""data"": [
    {
      ""field"": ""field1""
    },
    {
      ""id"": ""id2"",
      ""field"": ""field2""
    }
  ],
  ""paging"": {
    ""prev"": ""link2""
  }
}";
            string expectedTrimed = Regex.Replace(expected, @"\s", "");
            string mergedTrimed = Regex.Replace(merged, @"\s", "");
            Assert.Equal(expectedTrimed, mergedTrimed);
        }

    }
}
