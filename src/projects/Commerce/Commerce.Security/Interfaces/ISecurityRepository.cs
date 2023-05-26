using System.Linq.Expressions;
using Commerce.Security.DTOs;
using Commerce.Security.Models;

namespace Commerce.Security.Interfaces;

public interface ISecurityRepository
{
    Task<IReadOnlyCollection<User>> GetAllUsers();
    Task<User?> GetSingleUserAsync(int id); // TODO -> Check out whether this will stay as integer.
    Task AddUserAsync(User newUser);
    Task UpdateUserAsync(int id, User updatedUser);
    Task DeleteUserAsync(int id);
}
