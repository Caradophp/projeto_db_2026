using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Models.Enums;
using projeto.Repository;

namespace projeto.Service;

public class UserService(UserRepository userRepository)
{
    
    private readonly UserRepository _userRepository = userRepository;

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
}