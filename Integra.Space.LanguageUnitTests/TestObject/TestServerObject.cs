namespace Integra.Space.LanguageUnitTests.TestObject
{
    public class TestServerObject
    {
        public TestServerObject(string serverId, string serverName)
        {
            this.ServerId = serverId;
            this.ServerName = serverName;
        }

        public string ServerId { get; private set; }
        public string ServerName { get; private set; }
    }
}
