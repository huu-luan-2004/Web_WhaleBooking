using System.Collections.Concurrent;
using Web_WhaleBooking.Models;

namespace Web_WhaleBooking.Services;

public interface IUserStore
{
    User GetOrCreateByProvider(string providerId, string? email, string? name);
    User? GetById(int id);
    User? GetByEmail(string email);
    IEnumerable<User> All();
    bool TryUpdateRole(int id, UserRole role);
}

public class InMemoryUserStore : IUserStore
{
    private readonly ConcurrentDictionary<int, User> _byId = new();
    private readonly ConcurrentDictionary<string, int> _idByProvider = new();
    private readonly ConcurrentDictionary<string, int> _idByEmail = new(StringComparer.OrdinalIgnoreCase);
    private int _nextId = 1;

    public IEnumerable<User> All() => _byId.Values;

    public User GetOrCreateByProvider(string providerId, string? email, string? name)
    {
        if (_idByProvider.TryGetValue(providerId, out var id) && _byId.TryGetValue(id, out var existing))
            return existing;

        var newId = Interlocked.Increment(ref _nextId);
        var user = new User
        {
            Id = newId,
            ProviderId = providerId,
            Email = email ?? string.Empty,
            HoTen = name,
            VaiTro = UserRole.KhachHang
        };
        _byId[newId] = user;
        _idByProvider[providerId] = newId;
        if (!string.IsNullOrWhiteSpace(email))
            _idByEmail[email] = newId;
        return user;
    }

    public User? GetById(int id) => _byId.TryGetValue(id, out var u) ? u : null;

    public User? GetByEmail(string email)
    {
        return _idByEmail.TryGetValue(email, out var id) && _byId.TryGetValue(id, out var u) ? u : null;
    }

    public bool TryUpdateRole(int id, UserRole role)
    {
        if (!_byId.TryGetValue(id, out var u)) return false;
        u.VaiTro = role;
        return true;
    }
}
