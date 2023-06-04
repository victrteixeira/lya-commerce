using AutoMapper;
using Commerce.Security.DTOs;
using Commerce.Security.Interfaces;
using Commerce.Security.Models;

namespace Commerce.Security.Services;

public class SecurityService : ISecurityService
{
    private readonly IMapper _mapper;
    private readonly ISecurityRepository _repository;
    private readonly IPasswordService _pwdService;
    private readonly ITokenService _tokenRequest;
    private readonly IEmailService _emailService;

    public SecurityService(IMapper mapper, ISecurityRepository repository, IPasswordService pwdService, ITokenService tokenRequest, IEmailService emailService)
    {
        _mapper = mapper;
        _repository = repository;
        _pwdService = pwdService;
        _tokenRequest = tokenRequest;
        _emailService = emailService;
    }

    public async Task<ReadUser?> RegisterAsync(CreateUser command)
    {
        var newUser = _mapper.Map<User>(command);
        newUser.HashedPassword = _pwdService.HashPassword(command.Password);
       
        if (await GetUser(command.Email, IdentifierType.Email) != null)
        {
            throw new InvalidOperationException("Um usuário com este e-mail já existe.");
        }
        await _repository.AddUserAsync(newUser);

        await _emailService.SendEmailConfirmationAsync(command.Email, "adsjlkjkasd");

        return _mapper.Map<ReadUser>(newUser);
    }

    public async Task<bool> ConfirmEmailAsync(string token)
    {
        return true;
    }
    
    public async Task<string?> LoginAsync(LoginUser command)
    {
        var user = await GetUser(command.Email, IdentifierType.Email);
        if (user is null)
        {
            throw new KeyNotFoundException("Usuário não encontrado para esta e-mail, tente realizar um cadastro.");
        }

        var isPasswordValid = _pwdService.ValidateHash(command.Password, user.HashedPassword);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("E-mail ou Senha incorretos.");
        }

        return _tokenRequest.GenerateToken(user);
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordUser command)
    {
        var user = await GetUser(command.Email, IdentifierType.Email);
        if (user == null || user.Id is null)
        {
            throw new KeyNotFoundException("Não existe usuário cadastrado com este e-mail.");
        }

        var userPwdChanged = _pwdService.UpdateHash(command.OldPassword, user, command.NewPassword);
        
        await _repository.UpdateUserAsync(user.Id, userPwdChanged);

        return true;
    }

    private async Task<User?> GetUser(string method, IdentifierType type)
    {
        switch (type)
        {
            case IdentifierType.Email:
                var userByEmail = await _repository.GetSingleUserByEmailAsync(method);
                return userByEmail;
            case IdentifierType.Id:
                var userBydId = await _repository.GetSingleUserAsync(method);
                return userBydId;
            default:
                return null;
        }
    }

    private enum IdentifierType
    {
        Email,
        Id
    }
}