using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tzkt
{
    public class Quote
    {
        public int level { get; set; }
        public DateTime timestamp { get; set; }
        public decimal btc { get; set; }
        public decimal eur { get; set; }
        public decimal usd { get; set; }
        public decimal cny { get; set; }
        public decimal jpy { get; set; }
        public decimal krw { get; set; }
        public decimal eth { get; set; }
        public decimal gbp { get; set; }
    }

    public class Stat
    {
        public int level { get; set; }
        public DateTime timestamp { get; set; }
        public long totalSupply { get; set; }
        public long circulatingSupply { get; set; }
        public long totalBootstrapped { get; set; }
        public long totalCommitments { get; set; }
        public long totalActivated { get; set; }
        public long totalCreated { get; set; }
        public long totalBurned { get; set; }
        public long totalBanished { get; set; }
        public long totalVested { get; set; }
        public long totalFrozen { get; set; }
    }

    public class VotingPeriod
    {
        public int index { get; set; }
        public int epoch { get; set; }
        public int firstLevel { get; set; }
        public DateTime startTime { get; set; }
        public int lastLevel { get; set; }
        public DateTime endTime { get; set; }
        public string kind { get; set; }
        public string status { get; set; }
        public int totalBakers { get; set; }
        public int totalRolls { get; set; }
        public int upvotesQuorum { get; set; }
        public int proposalsCount { get; set; }
        public int topUpvotes { get; set; }
        public int topRolls { get; set; }
        public double ballotsQuorum { get; set; }
        public int supermajority { get; set; }
        public int yayBallots { get; set; }
        public int yayRolls { get; set; }
        public int nayBallots { get; set; }
        public int nayRolls { get; set; }
        public int passBallots { get; set; }
        public int passRolls { get; set; }
    }
}
