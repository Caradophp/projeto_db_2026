using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using projeto.Models;
using projeto.Models.Enums;
using projeto.Repository;

namespace projeto.Service;

public class UserService(UserRepository userRepository, CodeRepository codeRepository, EmailService emailService, UtilRepository repository)
{
    
    private readonly UserRepository _userRepository = userRepository;
    private readonly CodeRepository _codeRepository = codeRepository;
    private readonly EmailService _emailService = emailService;
    private readonly UtilRepository _repository = repository;

    public IActionResult CreateUser(string name, string email, string password, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Informe seu nome.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
             throw new Exception("Informe seu e-mail.");
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
             throw new Exception("A senha deve ter pelo menos 6 caracteres.");
        }

        if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
        {
             throw new Exception("As senhas não coincidem.");
        }

        if (_userRepository.ExistsByEmail(email))
        {
            throw new Exception("E-mail já cadastrado");
        }
        User user = new()
        {
            Name = name,
            Email = email,
            Password = password
        };
        return _userRepository.CreateUser(user);
    }

    public bool CheckUserLogin(string email, string password)
    {
        User user = _userRepository.CheckUserLogin(email, password);

        if (user != null)
        {

            if (user.Status.Equals(UserStatusEnum.Blocked))
            {
                throw new Exception("Usuário Bloqueado");
            }

            if (user.Status.Equals(UserStatusEnum.Inactive))
            {
                throw new Exception("Usuário detativado");
            }

            if (user.Password.Equals(password))
            {
                return true;
            }   
        } else
        {
            throw new KeyNotFoundException("Usuário não encontrado");
        }

        return false;
    } 

    public void SendEmailForRetrivePassword(string email)
    {
        User user = _userRepository.FindByEmail(email);
        int randomCode = new Random().Next(100000, 999999);

        if (user == null)
        {
            throw new Exception("E-mail não cadastrado no sistema");
        }

        _codeRepository.SaveCode(randomCode, user.Id);

        _emailService.Send(email, 
            "Código de recuperação de senha", "Prezado <b>" + user.Name 
            + "</b>,  Segue abaixo o código para recuperação de senha do sistema<br><br>Código: <b>" + randomCode + "</b>");
    }

    public bool CheckCode(int code, string email)
    {
        User user = _userRepository.FindByEmail(email);
        return _codeRepository.CheckIfCodeIsValid(code, user.Id);
    }

    public void ChangePass(string email, string password, string confirmPassword)
    {
        
        if (password.Equals(confirmPassword))
        {
            _userRepository.ChangePass(email, password);
        } else
        {
            throw new Exception("As senha devem ser iguais");
        }

    }
}