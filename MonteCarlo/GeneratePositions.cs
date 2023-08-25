namespace AggregationEngine.MonteCarlo;

public static class GeneratePositions
{
    private static int NumberOfLeaves = 10;
    private static int[] positionDomain = Enumerable.Range(1, NumberOfLeaves).ToArray();
    private static int[] holdingDomain = Enumerable.Range(1, NumberOfLeaves / 3).ToArray();
    private static int[] securityDomain = Enumerable.Range(1, NumberOfLeaves / 2).ToArray();
    private static string[] portfolioDomain = {"Portfolio1", "Portfolio2", "Portfolio3"};
    private static string[] currencyDomain = { "UAH", "USD", "EUR", "CZ", "DEN" };
    private static int[] instrumentTypeDomain = { 1, 2, 3 };

    public static List<AggregationPosition> GetPositionsRandom()
    {
        var random = new Random();
        var res = new List<AggregationPosition>();
        foreach(var item in positionDomain)
        {
            var metaData = new MetaData()
            {
                PositionIK = item,
                SecurityIK = securityDomain[random.Next(0, securityDomain.Length)],
                HoldingIK = holdingDomain[random.Next(0, holdingDomain.Length)],
                Currency = currencyDomain[random.Next(0, currencyDomain.Length)],
                Portfolio = portfolioDomain[random.Next(0, portfolioDomain.Length)],
                InstrumentType = instrumentTypeDomain[random.Next(0,3)],
                FreeCode1 = $"FreeCode1_{item}",
                FreeCode2 = $"FreeCode2_{item}",
                FreeCode3 = $"FreeCode3_{item}"
            };
            var scaleData = new ScalingData()
            {
                Nominal = 100 * item
            };
            var values = new Vector()
            {
                Scenarious = Enumerable.Range(1, 7).Select(x => 3.14 * x).ToArray()
            };

            var position = new AggregationPosition(metaData, scaleData.Nominal, values.Scenarious);
            res.Add(position);
        }
        return res;
    }

    
}
