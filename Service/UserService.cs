using System.Security.Cryptography;
using projeto.Models;
using projeto.Repository;

namespace projeto.Service;

public class UserService(
    UserRepository userRepository,
    CodeRepository codeRepository,
    EmailService emailService)
{
    private readonly UserRepository _userRepository = userRepository;
    private readonly CodeRepository _codeRepository = codeRepository;
    private readonly EmailService _emailService = emailService;

    public bool CreateUser(
        string name,
        string email,
        string password,
        string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Nome deve ser informado.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new Exception("E-mail deve ser informado.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new Exception("Senha deve ser informada.");
        }

        if (password != confirmPassword)
        {
            throw new Exception("As senhas devem ser iguais.");
        }

        if (_userRepository.ExistsByEmail(email))
        {
            throw new Exception("E-mail já cadastrado no sistema.");
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
        User? user = _userRepository.CheckUserLogin(email, password);

        return user != null;
    }

    public void SendEmailForRetrivePassword(string email)
    {
        User? user = _userRepository.FindByEmail(email);

        if (user == null)
        {
            throw new Exception("E-mail não cadastrado no sistema.");
        }

        int randomCode = RandomNumberGenerator.GetInt32(
            100000,
            1000000
        );

        _codeRepository.SaveCode(randomCode, user.Id);

        _emailService.Send(
            email,
            "Código de recuperação de senha",
            $@"
                <p>Prezado <b>{user.Name}</b>,</p>

                <p>
                    Recebemos uma solicitação para recuperação
                    da senha da sua conta.
                </p>

                <p>
                    Código de recuperação:
                    <b>{randomCode}</b>
                </p>

                <p>
                    O código é temporário e deve ser utilizado
                    para continuar com a recuperação da senha.
                </p>

                <p>
                    Caso você não tenha solicitado a recuperação,
                    ignore este e-mail.
                </p>
            "
        );
    }

    public bool CheckCode(int code, string email)
    {
        User? user = _userRepository.FindByEmail(email);

        if (user == null)
        {
            return false;
        }

        return _codeRepository.CheckIfCodeIsValid(
            code,
            user.Id
        );
    }

    public void ChangePass(
        string email,
        string password,
        string confirmPassword,
        int code)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new Exception("Senha deve ser informada.");
        }

        if (string.IsNullOrWhiteSpace(confirmPassword))
        {
            throw new Exception(
                "Confirmação da senha deve ser informada."
            );
        }

        if (password != confirmPassword)
        {
            throw new Exception(
                "As senhas informadas devem ser iguais."
            );
        }

        User? user = _userRepository.FindByEmail(email);

        if (user == null)
        {
            throw new Exception(
                "Não foi possível validar a recuperação de senha."
            );
        }

        bool codeIsValid =
            _codeRepository.CheckIfCodeIsValid(
                code,
                user.Id
            );

        if (!codeIsValid)
        {
            throw new Exception(
                "O código de recuperação é inválido ou expirou."
            );
        }

        _userRepository.ChangePass(
            email,
            password,
            true
        );

        _codeRepository.DeleteCodesByUser(user.Id);
    }
}