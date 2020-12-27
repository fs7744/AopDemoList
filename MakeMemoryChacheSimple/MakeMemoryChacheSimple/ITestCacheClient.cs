namespace MakeMemoryChacheSimple
{
    public interface ITestCacheClient
    {
        string Say(string v);
    }

    public class TestCacheClient : ITestCacheClient
    {
        [Cache(nameof(Say), "00:00:03")]
        public string Say(string v) => v;
    }
}