using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace projeto.Service;

public class EncryptionService
{
    private readonly string _encryptionKey;

    public EncryptionService()
    {
        // Recupera a chave de criptografia do arquivo .env
        _encryptionKey = DotNetEnv.Env.GetString("ENCRYPTION_KEY")
            ?? throw new InvalidOperationException("A chave 'ENCRYPTION_KEY' não foi configurada no ambiente.");
    }

    #region Password Hashing (BCrypt)

    /// <summary>
    /// Gera um hash seguro para a senha utilizando BCrypt.
    /// </summary>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Verifica se uma senha em texto plano corresponde ao hash armazenado.
    /// </summary>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch (BCrypt.Net.BCryptException)
        {
            // Retorna falso se o hash estiver em formato inválido (útil para migração de texto plano)
            return false;
        }
    }

    #endregion

    #region Data Encryption (AES-256-GCM)

    /// <summary>
    /// Criptografa dados sensíveis usando AES-256-GCM (Authenticated Encryption).
    /// </summary>
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return plainText;

        byte[] key = GetKeyBytes();
        byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        RandomNumberGenerator.Fill(nonce);

        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] ciphertext = new byte[plaintextBytes.Length];
        byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];

        using var aesGcm = new AesGcm(key);
        aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);

        // Formato de armazenamento: Nonce (12) + Tag (16) + Ciphertext
        byte[] combined = new byte[nonce.Length + tag.Length + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, combined, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, combined, nonce.Length, tag.Length);
        Buffer.BlockCopy(ciphertext, 0, combined, nonce.Length + tag.Length, ciphertext.Length);

        return Convert.ToBase64String(combined);
    }

    /// <summary>
    /// Descriptografa dados cifrados via AES-256-GCM.
    /// </summary>
    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return cipherText;

        byte[] combined = Convert.FromBase64String(cipherText);
        byte[] key = GetKeyBytes();

        int nonceSize = AesGcm.NonceByteSizes.MaxSize;
        int tagSize = AesGcm.TagByteSizes.MaxSize;
        int cipherSize = combined.Length - nonceSize - tagSize;

        byte[] nonce = new byte[nonceSize];
        byte[] tag = new byte[tagSize];
        byte[] ciphertext = new byte[cipherSize];

        Buffer.BlockCopy(combined, 0, nonce, 0, nonceSize);
        Buffer.BlockCopy(combined, nonceSize, tag, 0, tagSize);
        Buffer.BlockCopy(combined, nonceSize + tagSize, ciphertext, 0, cipherSize);

        byte[] plaintextBytes = new byte[cipherSize];

        using var aesGcm = new AesGcm(key);
        aesGcm.Decrypt(nonce, ciphertext, tag, plaintextBytes);

        return Encoding.UTF8.GetString(plaintextBytes);
    }

    private byte[] GetKeyBytes()
    {
        // Garante que a chave tenha exatamente 32 bytes para AES-256
        byte[] keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);
        byte[] hashedKey = new byte[32];

        using var sha256 = SHA256.Create();
        byte[] fullHash = sha256.ComputeHash(keyBytes);
        Buffer.BlockCopy(fullHash, 0, hashedKey, 0, 32);

        return hashedKey;
    }

    #endregion
}
