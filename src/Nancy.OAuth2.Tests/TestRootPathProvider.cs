namespace Nancy.OAuth2.Tests
{
    internal class TestRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return "../..";
        }
    }
}
