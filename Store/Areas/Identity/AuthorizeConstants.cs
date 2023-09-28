namespace Store.Areas.Identity;

public static class Roles {
    public const string Admin = "Admin";
}

public static class Policies {
    public const string Admin = Roles.Admin;
}