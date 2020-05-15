namespace SSPreventer
{
    public interface ISSPreventerConfig
    {
        int IntervalSeconds { get; }
    }

    public class SSPreventerConfig : ISSPreventerConfig
    {
        public int IntervalSeconds { get; set; } = 60; // 1 min by default
    }
}
