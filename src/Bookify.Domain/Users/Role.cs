
namespace Bookify.Domain.Users;

public sealed class Role
{
    public static readonly Role Registered = new Role(1, "Registered");
    public Role(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public ICollection<User> Users { get; init; } = new List<User>();
    public ICollection<Permission> Permissions { get; init; } = new List<Permission>();
}