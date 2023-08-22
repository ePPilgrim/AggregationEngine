using SimCorp.AggregationEngine.Core.Domain;

namespace AggregationEngine.MonteCarlo;

public class AggregationPosition : IAggregationPosition
{
    public AggregationPosition()
    {}
    public AggregationPosition(IMetaData metaData, double nominal, double[] scenarious)
    {
        PositionIK = metaData.PositionIK;
        SecurityIK = metaData.SecurityIK;
        HoldingIK = metaData.HoldingIK;
        Currency = metaData.Currency;
        Portfolio = metaData.Portfolio;
        FreeCode1 = metaData.FreeCode1;
        FreeCode2 = metaData.FreeCode2;


        Nominal = nominal;
        Scenarious = scenarious.ToArray();
    }

    public AggregationPosition(AggregationPosition other)   
    {
        PositionIK = other.PositionIK;
        SecurityIK = other.SecurityIK;
        HoldingIK= other.HoldingIK;
        Currency = other.Currency;
        Portfolio = other.Portfolio;
        FreeCode1 = other.FreeCode1;
        FreeCode2 = other.FreeCode2;

        Nominal = other.Nominal;
        Scenarious = other.Scenarious.ToArray();
    }

    public double Nominal { get; set; }
    public double[]? Scenarious { get; set; }


    public int? PositionIK { get; set; }
    public int? SecurityIK { get; set; }
    public int? HoldingIK { get ; set ; }
    public string? Currency { get ; set ; }
    public string? Portfolio { get ; set; }
    public string? FreeCode1 { get; set ; }
    public string? FreeCode2 { get; set ; }
    public string? FreeCode3 { get; set; }


    public void DoAdditionOperation(IAggregationPosition aggregationPosition)
    {
        var aggrpos = aggregationPosition as AggregationPosition;
        if (aggrpos != null)
        {
            Nominal += aggrpos.Nominal;
            for(int i = 0; i < aggrpos.Scenarious?.Length; i++)
            {
                Scenarious[i] += aggrpos.Scenarious[i];
            }
        }

        Currency = (aggrpos.Currency == Currency) ? aggrpos.Currency : null;
        PositionIK = (aggrpos.PositionIK == PositionIK) ? aggrpos.PositionIK : null;
        Portfolio = (aggrpos.Portfolio == Portfolio) ? aggrpos.Portfolio : null;
        SecurityIK = (aggrpos.SecurityIK == SecurityIK) ? aggrpos.SecurityIK : null;
        HoldingIK = (aggrpos.HoldingIK == HoldingIK) ? aggrpos.HoldingIK : null;
        FreeCode1 = (aggrpos.FreeCode1 == FreeCode1) ? aggrpos.FreeCode1 : null;
        FreeCode2 = (aggrpos.FreeCode2 == FreeCode2) ? aggrpos.FreeCode2 : null;
    }
}
