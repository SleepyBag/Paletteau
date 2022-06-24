using System.Collections.Generic;
using NUnit.Framework;
using Paletteau.Core.Plugin;
using Paletteau.Plugin;

namespace Paletteau.Test
{
    public class QueryBuilderTest
    {
        [Test]
        public void ExclusivePluginQueryTest()
        {
            var nonGlobalPlugins = new Dictionary<string, PluginPair>
            {
                {">", new PluginPair {Metadata = new PluginMetadata {ActionKeywords = new List<string> {">"}}}}
            };

            Query q = QueryBuilder.Build(">   file.txt    file2 file3", null, nonGlobalPlugins);

            Assert.AreEqual("file.txt file2 file3", q.Search);
            Assert.AreEqual(">", q.ActionKeyword);
        }

        [Test]
        public void ExclusivePluginQueryIgnoreDisabledTest()
        {
            var nonGlobalPlugins = new Dictionary<string, PluginPair>
            {
                {">", new PluginPair {Metadata = new PluginMetadata {ActionKeywords = new List<string> {">"}, Disabled = true}}}
            };

            Query q = QueryBuilder.Build(">   file.txt    file2 file3", null, nonGlobalPlugins);

            Assert.AreEqual("> file.txt file2 file3", q.Search);
        }

        [Test]
        public void GenericPluginQueryTest()
        {
            Query q = QueryBuilder.Build("file.txt file2 file3", null, new Dictionary<string, PluginPair>());

            Assert.AreEqual("file.txt file2 file3", q.Search);
            Assert.AreEqual("", q.ActionKeyword);

            Assert.AreEqual("file.txt", q.FirstSearch);
            Assert.AreEqual("file2", q.SecondSearch);
            Assert.AreEqual("file3", q.ThirdSearch);
            Assert.AreEqual("file2 file3", q.SecondToEndSearch);
        }
    }
}
