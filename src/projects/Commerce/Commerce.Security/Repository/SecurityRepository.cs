using Commerce.Security.Database;
using Commerce.Security.DTOs;
using Commerce.Security.Interfaces;
using Commerce.Security.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Commerce.Security.Repository;

public class SecurityRepository : ISecurityRepository
{
    private readonly IMongoCollection<User> _userCollection;

    public SecurityRepository(IOptions<MongoDbSettings> securityDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            securityDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            securityDatabaseSettings.Value.DatabaseName);

        _userCollection = mongoDatabase.GetCollection<User>(
            securityDatabaseSettings.Value.UserCollectionName);
    }


    public async Task<IReadOnlyCollection<User>> GetAllUsers() =>
        await _userCollection.Find(_ => true).ToListAsync();

    public async Task<User?> GetSingleUserAsync(string id) =>
        await _userCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task AddUserAsync(User newUser) =>
        await _userCollection.InsertOneAsync(newUser);

    public async Task UpdateUserAsync(string id, User updatedUser) =>
        await _userCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

    public async Task DeleteUserAsync(string id) =>
        await _userCollection.DeleteOneAsync(x => x.Id == id);
}