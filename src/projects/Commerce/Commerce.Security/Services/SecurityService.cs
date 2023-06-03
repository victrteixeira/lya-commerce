using AutoMapper;
using Commerce.Security.DTOs;
using Commerce.Security.Interfaces;
using Commerce.Security.Models;
using Commerce.Security.Utils;

namespace Commerce.Security.Services;

public class SecurityService : ISecurityService
{
    private readonly IMapper _mapper;
    private readonly ISecurityRepository _repository;
    private readonly IPasswordService _pwdService;
    private readonly ITokenService _tokenRequest;

    public SecurityService(IMapper mapper, ISecurityRepository repository, IPasswordService pwdService, ITokenService tokenRequest)
    {
        _mapper = mapper;
        _repository = repository;
        _pwdService = pwdService;
        _tokenRequest = tokenRequest;
    }

    public async Task<ReadUser?> RegisterAsync(CreateUser command)
    {
        var userExist = await _repository.GetSingleUserByEmailAsync(command.Email);

        if (userExist != null)
            throw new InvalidOperationException("Um usuário com este e-mail já existe.");
        
        if (command.Password != command.ConfirmPassword)
            throw new InvalidPasswordException("As senhas não são iguais. Tente novamente.");

        if(!_pwdService.ValidatePassword(command.Password))
            throw new InvalidPasswordException("Senha não é válida. Precisa conter ao menos" +
                                               " uma letra maiúscula, uma minúscula, um digito númerico," +
                                               " e precisa ser maior ou igual a 8 digitos.");

        command.Password = await _pwdService.EncryptPasswordAsync(command.Password);
        
        var newUser = _mapper.Map<User>(command);

        await _repository.AddUserAsync(newUser);

        return _mapper.Map<ReadUser>(newUser);
    }

    public async Task<ReadUser> RegisterAdminAsync(CreateUser command)
    {
        var userExist = await _repository.GetSingleUserByEmailAsync(command.Email);
        
        if (userExist != null)
            throw new InvalidOperationException("Um usuário com este e-mail já existe.");
        
        if (command.Password != command.ConfirmPassword)
            throw new InvalidPasswordException("As senhas não são iguais. Tente novamente.");

        if(!_pwdService.ValidatePassword(command.Password))
            throw new InvalidPasswordException("Senha não é válida. Precisa conter ao menos" +
                                               " uma letra maiúscula, uma minúscula, um digito númerico," +
                                               " e precisa ser maior ou igual a 8 digitos.");

        command.Password = await _pwdService.EncryptPasswordAsync(command.Password);

        var newAdmin = _mapper.Map<User>(command);
        newAdmin.UpdateRole("Admin");

        if (!newAdmin.Role.Equals("Admin"))
            throw new InvalidOperationException("Alguma coisa deu errado."); // TODO -> Change it later.

        await _repository.AddUserAsync(newAdmin);

        return _mapper.Map<ReadUser>(newAdmin);
    }

    public async Task<string?> LoginAsync(LoginUser command)
    {
        var user = await _repository.GetSingleUserByEmailAsync(command.Email);
        
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado para esta e-mail, tente realizar um cadastro.");

        var pwdEqual = _pwdService.VerifyHash(command.Password, user.HashedPassword);

        if (!pwdEqual)
            throw new UnauthorizedAccessException("E-mail ou Senha incorretos.");

        return _tokenRequest.GenerateToken(user);
    }
}