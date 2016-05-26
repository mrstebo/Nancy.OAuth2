namespace Nancy.OAuth2.IntegrationTests
{
    class TestRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return "../..";
        }
    }
}
