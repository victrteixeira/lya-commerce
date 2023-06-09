using AutoMapper;
using Commerce.Security.DTOs;
using Commerce.Security.Helpers.Exceptions;
using Commerce.Security.Interfaces;
using Commerce.Security.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Commerce.Security.Services;

public class SecurityService : ISecurityService
{
    private readonly IMapper _mapper;
    private readonly ISecurityRepository _repository;
    private readonly IPasswordService _pwdService;
    private readonly ITokenService _tokenRequest;
    private readonly IEmailService _emailService;
    private readonly IMemoryCache _cache;

    public SecurityService(IMapper mapper, ISecurityRepository repository, IPasswordService pwdService, ITokenService tokenRequest, IEmailService emailService, IMemoryCache cache)
    {
        _mapper = mapper;
        _repository = repository;
        _pwdService = pwdService;
        _tokenRequest = tokenRequest;
        _emailService = emailService;
        _cache = cache;
    }

    public async Task<ReadUser?> RegisterAsync(CreateUser command)
    {
        var newUser = _mapper.Map<User>(command);
        newUser.HashedPassword = _pwdService.HashPassword(command.Password);
       
        if (await GetUserAsync(command.Email, IdentifierType.Email) != null)
        {
            throw new InvalidOperationException("Um usuário com este e-mail já existe.");
        }
        await _repository.AddUserAsync(newUser);

        var emailToken = _tokenRequest.GenerateEmailToken(newUser);

        await _emailService.SendEmailConfirmationAsync(newUser, emailToken);

        return _mapper.Map<ReadUser>(newUser);
    }

    public async Task<bool> ConfirmEmailAsync(string token)
    {
        if (!_cache.TryGetValue(token, out _))
        {
            throw new UnauthorizedAccessException("Token não válido ou não existe.");
        }

        string? email = _tokenRequest.ExtractEmailClaim(token);
        if (email is null)
        {
            throw new InvalidTokenException(nameof(token));
        }

        var user = await GetUserAsync(email, IdentifierType.Email);
        await SetEmailConfirmedAsync(user!);
        _cache.Remove(token);
        return true;
    }
    
    public async Task<string?> LoginAsync(LoginUser command)
    {
        var user = await GetUserAsync(command.Email, IdentifierType.Email);
        if (user is null)
        {
            throw new KeyNotFoundException("Usuário não encontrado para esta e-mail, tente realizar um cadastro.");
        }

        var isPasswordValid = _pwdService.ValidateHash(command.Password, user.HashedPassword);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("E-mail ou Senha incorretos.");
        }

        if (user.EmailConfirmed == false)
        {
            throw new UnauthorizedAccessException("Você precisa confirmar o seu e-mail.");
        }

        return _tokenRequest.GenerateToken(user);
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordUser command)
    {
        var user = await GetUserAsync(command.Email, IdentifierType.Email);
        if (user == null || user.Id is null)
        {
            throw new KeyNotFoundException("Não existe usuário cadastrado com este e-mail.");
        }

        var userPwdChanged = _pwdService.UpdateHash(command.OldPassword, user, command.NewPassword);
        
        await _repository.UpdateUserAsync(user.Id, userPwdChanged);

        return true;
    }

    public async Task ForgotPasswordRequest(string? email)
    {
        if (email is null)
        {
            throw new ArgumentException();
        }
        var user = await GetUserAsync(email, IdentifierType.Email);
        if (user is null)
        {
            throw new KeyNotFoundException("Não há nenhum usuário registrado para este e-mail.");
        }

        var token = _tokenRequest.GenerateEmailToken(user);
        await _emailService.SendEmailForgotPasswordAsync(user, token);
    }

    public async Task<bool> PasswordRecovery(string token, ResetPasswordUser command)
    {
        var userIdentifier = _tokenRequest.ExtractEmailClaim(token);
        var user = await GetUserAsync(userIdentifier, IdentifierType.Email);
        if (user is null || user.Id is null)
        {
            throw new ArgumentNullException();
        }

        var newHash = _pwdService.HashPassword(command.Password);
        user.HashedPassword = newHash;

        await _repository.UpdateUserAsync(user.Id, user);
        return true;
    }

    public async Task DeleteUserAsync(string id)
    {
        var user = await GetUserAsync(id, IdentifierType.Id);
        if (user is null || user.Id is null)
        {
            throw new KeyNotFoundException("Este usuário não existe.");
        }

        await _repository.DeleteUserAsync(user.Id);
    }

    public async Task<IReadOnlyCollection<User>> GetAllUsersAsync()
    {
        return await _repository.GetAllUsers();
    }

    private async Task<User?> GetUserAsync(string method, IdentifierType type)
    {
        switch (type)
        {
            case IdentifierType.Email:
                return await _repository.GetSingleUserByEmailAsync(method);
            case IdentifierType.Id:
                return await _repository.GetSingleUserAsync(method);
            default:
                return null;
        }
    }

    private Task SetEmailConfirmedAsync(User user)
    {
        if (user is null || user.Id is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        user.EmailConfirmed = true;
        _repository.UpdateUserAsync(user.Id, user);
        return Task.CompletedTask;
    }

    private enum IdentifierType
    {
        Email,
        Id
    }
}