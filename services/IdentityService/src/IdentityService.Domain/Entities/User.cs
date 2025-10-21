namespace IdentityService.Domain.Entities;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nickname { get; private set; }

    private User() { } // EF
    public User(string nickname)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            throw new ArgumentException("Nickname cannot be empty.");

        Nickname = nickname.Trim();
    }
}
