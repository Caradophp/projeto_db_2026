using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Models.Enums;
using projeto.Repository;

namespace projeto.Service;

public class UserService(UserRepository userRepository)
{
    
    private readonly UserRepository _userRepository = userRepository;

    public IActionResult CreateUser(string name, string email, string password)
    {
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
        }

        return false;
    } 
}