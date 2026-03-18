namespace MatchMaking.Models
{
    public enum MatchState
    {
        Matching,
        Matched
    }

    public enum HumanType
    {
        Bot,
        Human
    }

    public enum Tier
    {
        Tier1,
        Tier2,
        Tier3,
        Tier4,
        Tier5,
        Tier6,
        Tier7,
        Tier8,
        Tier9,
        Tier10
    }

    public class UserInfo
    {
        public string Id { get; set; }
        public Tier Tier { get; set; }
        public MatchState MatchState { get; set; }
        public HumanType HumanType { get; set; }
    }
}
