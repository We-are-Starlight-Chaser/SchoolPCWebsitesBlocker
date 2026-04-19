namespace SPCWB.Models
{
    public readonly record struct BlockerItem
    {
        public readonly string Name { get; init; }
        public readonly string Description { get; init; }
        public readonly string[] Url { get; init; }
        public readonly bool IsBlocked { get; init; }
    }
}
