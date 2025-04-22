namespace XUnitPostgreSQL;

/// <summary>
/// PostgreSQL Fixture Options
/// </summary>
public class PostgreSQLFixtureOptions
{
    /// <summary>
    /// Image name for PostgreSQL container
    /// </summary>
    public string? ImageName { get; set; }

    /// <summary>
    /// Database name for PostgreSQL container
    /// </summary>
    public string? DatabaseName { get; set; }

    /// <summary>
    /// Username for PostgreSQL container
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Password for PostgreSQL container
    /// </summary>
    public string? Password { get; set; }

    internal void Validate()
    {
        ImageName ??= "postgres:latest";
        DatabaseName ??= "chatdb";
        Username ??= "chatuser";
        Password ??= "chatpassword";
    }
}
