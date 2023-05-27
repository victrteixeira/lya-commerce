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

    public SecurityService(IMapper mapper, ISecurityRepository repository, IPasswordService pwdService)
    {
        _mapper = mapper;
        _repository = repository;
        _pwdService = pwdService;
    }

    public async Task<ReadUser?> RegisterAsync(CreateUser command)
    {
        if (command.Password != command.ConfirmPassword)
            throw new InvalidPasswordException("As senhas não são iguais. Tente novamente.");

        if(!_pwdService.ValidatePassword(command.Password))
            throw new InvalidPasswordException("Senha não é válida. Precisa conter ao menos" +
                                               " uma letra maiúscula, uma minúscula, um digito númerico," +
                                               " e precisa ser maior ou igual a 8 digitos.");
        
        command.Password = await _pwdService.EncryptPassword(command.Password);
        
        var newUser = _mapper.Map<User>(command);

        await _repository.AddUserAsync(newUser);

        return _mapper.Map<ReadUser>(newUser);
    }

    public async Task<bool> LoginAsync(LoginUser command)
    {
        throw new NotImplementedException();
    }
}