namespace MinimalAPI;

public class SafeServersSetup
{
    public IReadOnlyCollection<Regulator> Regulators { get; init; } = new List<Regulator>();
}

public class Regulator
{
    public int Id { get; init; }
    public string Name { get; init; } = String.Empty;
    public int OperatorId { get; init; }
    public int[] ProductTypes { get; init; } = Array.Empty<int>();
    public List<RequiredProperty> RequiredProperties { get; set; } = new List<RequiredProperty>();
}

public enum RequiredProperty
{
    CategoryId = 0,
    GameTypeId = 1,
    CertItl = 2,
    CertNumber = 3,
    MinBet = 4,
    MaxBet = 5,
    ProfitRatio = 6,
    FilesList = 7,
    HashList = 8,
    SupplierRegNumber = 9,
}
