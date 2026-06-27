namespace Damoor.Infrastructure.Identity;

public static class RoleNames
{
    public const string User = "User";
    public const string Admin = "Admin";

    public static readonly string[] All = [User, Admin];
}

public static class PolicyNames
{
    public const string AdminOnly = "AdminOnly";
}
