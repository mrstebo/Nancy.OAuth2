using System.IO;

namespace Nancy.OAuth2.Tests
{
    internal class TestRootPathProvider : IRootPathProvider
    {
        private static string _rootPath;

        public string GetRootPath()
        {
            var rootPath = _rootPath ?? (_rootPath = Path.GetFullPath(Directory.GetCurrentDirectory() + "/../.."));

            return rootPath;
        }
    }
}
