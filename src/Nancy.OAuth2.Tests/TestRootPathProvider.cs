using System.IO;
using System.Reflection;

namespace Nancy.OAuth2.Tests
{
    internal class TestRootPathProvider : IRootPathProvider
    {
        private static readonly string RootPath;

        static TestRootPathProvider()
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetAssembly(typeof(TestRootPathProvider)).CodeBase) ?? "";
            var assemblyPath = directoryName.Replace(@"file:\", "");

            RootPath = Path.Combine(assemblyPath, "..", "..");
        }

        public string GetRootPath()
        {
            return RootPath;
        }
    }
}
