using Ardalis.GuardClauses;
using TebaTech.SharedKernel;

namespace Arb.Product.Core;

public class MarketPlace : BaseEntity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // ReSharper disable once UnusedMember.Local
    private MarketPlace()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {   
    }
    public MarketPlace(string marketPlaceName)
    {
        Guard.Against.NullOrWhiteSpace(marketPlaceName);
        
        MarketPlaceName = marketPlaceName;
    }
    
    public string MarketPlaceName { get; private set; }
}
