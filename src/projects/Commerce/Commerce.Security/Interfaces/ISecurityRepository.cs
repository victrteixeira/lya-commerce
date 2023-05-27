using System.Linq.Expressions;
using Commerce.Security.DTOs;
using Commerce.Security.Models;

namespace Commerce.Security.Interfaces;

public interface ISecurityRepository
{
    Task<IReadOnlyCollection<User>> GetAllUsers();
    Task<User?> GetSingleUserAsync(string id);
    Task AddUserAsync(User newUser);
    Task UpdateUserAsync(string id, User updatedUser);
    Task DeleteUserAsync(string id);
}
