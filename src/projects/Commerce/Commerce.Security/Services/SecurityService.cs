using AutoMapper;
using Commerce.Security.DTOs;
using Commerce.Security.Helpers.Exceptions;
using Commerce.Security.Interfaces;
using Commerce.Security.Models;

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

    public async Task<bool> ChangePasswordAsync(ChangePasswordUser command)
    {
        var user = await _repository.GetSingleUserByEmailAsync(command.Email);
        if (user == null)
            throw new KeyNotFoundException("Não existe usuário cadastrado com este e-mail.");

        if (user.Id == null)
            throw new ArgumentNullException();

        var userPwdChanged = await _pwdService.UpdatePasswordAsync(command.OldPassword, user, command.NewPassword);
        
        await _repository.UpdateUserAsync(user.Id, userPwdChanged);

        return true;
    }
}