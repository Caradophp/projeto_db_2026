# Project Setup Guide

[🇺🇸 English](#project-setup-guide) | [🇧🇷 Português](#guia-de-configuração-do-projeto)

## Prerequisites

- Docker with Docker Compose installed.
- .NET SDK 8 installed.
- A web browser.

## Apresentation Video

Youtube: [https://youtu.be/Zcs_KK_hEX8](https://youtu.be/Zcs_KK_hEX8)

## Creating the Oracle Database

The project already includes a `compose.yml` file in its root directory. There is no need to recreate it; from the project root, start the database container with:

```bash
docker compose up -d
```

The Oracle Free image uses the default database service name `FREE` when the container is created. Use `FREE` in the connection string's data source (for example, `localhost:1521/FREE`). The compose file sets the example database password to `123`; change it for any environment beyond local development and keep the connection-string password in sync.

## Configuring the Database Connection String

1. Open `appsettings.json` in the project.
2. Configure the `ConnectionStrings` section with your database credentials and connection details.

Example:

```json
{
  "ConnectionStrings": {
    "OracleDb": "User Id=<your_database_user>;Password=<your_database_password>;Data Source=localhost:1521/FREE;"
  }
}
```

Replace `<your_database_user>` and `<your_database_password>` with the credentials configured for your Oracle database. The service name for the included compose setup is `FREE`.
The default `<your_database_user>` is `system`.

## Configuring the `.env` File

The application uses environment variables for email, API security, and encryption.

### 1. Create the `.env` file

Create `.env` in the project root. On Linux or macOS:

```bash
touch .env
```

On Windows, create the file manually in the project root.

### 2. Obtain and set the values

Add the following variables, using the exact names expected by the application:

```dotenv
EMAIL_FROM=<your_personal_email_address>
EMAIL_PASSWORD=<email_provider_app_password_or_smtp_password>
API_SECRECT_KEY=<your_random_api_secret>
ENCRYPTION_KEY=<your_random_encryption_key>
```

- **`EMAIL_FROM`**: Use an email address you personally own and can access. The sending account must allow SMTP sending. For Gmail or another provider that uses two-step verification, create an **app password** in the provider's account security settings and use that value for `EMAIL_PASSWORD`—not your normal account password. If your provider does not offer app passwords, follow its SMTP authentication instructions or use an approved SMTP credential. Never send your email password to anyone.
- **`API_SECRECT_KEY`**: Generate a unique, unpredictable secret for this application; do not reuse a password or publish the value. Confirm the application actually expects this spelling: `SECRECT` appears misspelled and the code may instead require `API_SECRET_KEY`. Use the name referenced by the application's configuration/code, and update both the application and this file consistently if correcting it.
- **`ENCRYPTION_KEY`**: Generate a cryptographically random key using the length and encoding required by the encryption implementation in the code. For example, `openssl rand -base64 32` generates 32 random bytes encoded as Base64, but use that format only if the application expects Base64 and a 32-byte key. Do not use an ordinary phrase or a key copied from another environment. Store a secure backup: losing this key may make data encrypted with it unrecoverable.

Check the application code/configuration for required key length, encoding, SMTP host, port, and TLS settings; those requirements are application- and provider-specific. Do not commit `.env` or share its contents. Ensure `.env` is listed in `.gitignore`; use a secrets manager or deployment environment variables outside local development. If a secret is exposed, revoke/rotate it.

## Before Running the Project

Run the `00-create-migrations-table.sql` script manually against the database before proceeding. It is located in the `Database/` directory.

## Running the Project

### 1. Start the Oracle database

From the project root, start the existing compose configuration:

```bash
docker compose up -d
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Run the application

```bash
dotnet run --urls "http://0.0.0.0:8081"
```

### 4. Open the application

Navigate to [http://localhost:8081/login](http://localhost:8081/login).

## Notes

- The Oracle service name in the included compose setup is `FREE`.
- The compose example database password is `123` and is for local development only; replace it with a strong password where appropriate.
- Oracle is exposed on port `1521`; the application runs on port `8081`.
- Make sure Docker is running and the database container is ready before starting the application.
- Ensure the connection string and environment variables match the application's configuration.

---

# Guia de Configuração do Projeto

[🇺🇸 English](#project-setup-guide) | [🇧🇷 Português](#guia-de-configuração-do-projeto)

## Pré-requisitos

- Docker com Docker Compose instalado.
- SDK do .NET na versão 8 instalado.
- Um navegador web.

## Video de apresentação

Youtube: [https://youtu.be/Zcs_KK_hEX8](https://youtu.be/Zcs_KK_hEX8)

## Criando o Banco de Dados Oracle

O projeto já contém o arquivo `compose.yml` na raiz. Não é necessário criá-lo novamente; na raiz do projeto, inicie o contêiner do banco com:

```bash
docker compose up -d
```

A imagem Oracle Free usa o nome de serviço padrão `FREE` quando o contêiner é criado. Use `FREE` na origem de dados da string de conexão (por exemplo, `localhost:1521/FREE`). O arquivo compose define a senha de exemplo do banco como `123`; altere-a para qualquer ambiente além do desenvolvimento local e mantenha a senha da string de conexão sincronizada.

## Configurando a String de Conexão com o Banco de Dados

1. Abra o arquivo `appsettings.json` do projeto.
2. Configure a seção `ConnectionStrings` com as credenciais e os dados de conexão do banco.

Exemplo:

```json
{
  "ConnectionStrings": {
    "OracleDb": "User Id=<seu_usuario_do_banco>;Password=<sua_senha_do_banco>;Data Source=localhost:1521/FREE;"
  }
}
```

Substitua `<seu_usuario_do_banco>` e `<sua_senha_do_banco>` pelas credenciais configuradas no Oracle. O nome do serviço no compose fornecido é `FREE`.
O `<your_database_user>` padrão é `system`.

## Configurando o Arquivo `.env`

A aplicação usa variáveis de ambiente para email, segurança da API e criptografia.

### 1. Crie o arquivo `.env`

Crie `.env` na raiz do projeto. No Linux ou macOS:

```bash
touch .env
```

No Windows, crie o arquivo manualmente na raiz do projeto.

### 2. Obtenha e configure os valores

Adicione as variáveis abaixo, usando exatamente os nomes esperados pela aplicação:

```dotenv
EMAIL_FROM=<seu_email_pessoal>
EMAIL_PASSWORD=<senha_de_aplicativo_ou_senha_smtp_do_email>
API_SECRECT_KEY=<sua_chave_secreta_aleatoria_da_api>
ENCRYPTION_KEY=<sua_chave_aleatoria_de_criptografia>
```

- **`EMAIL_FROM`**: Use um endereço de email pessoal que pertença a você e ao qual tenha acesso. A conta precisa permitir envio por SMTP. No Gmail ou em outro provedor que use verificação em duas etapas, crie uma **senha de app/senha de aplicativo** nas configurações de segurança da conta e use esse valor em `EMAIL_PASSWORD` — não a senha normal da conta. Se o provedor não oferecer senhas de app, siga as instruções dele para autenticação SMTP ou use uma credencial SMTP aprovada. Nunca envie sua senha de email para outras pessoas.
- **`API_SECRECT_KEY`**: Gere um segredo exclusivo e imprevisível para esta aplicação; não reutilize uma senha nem publique o valor. Confirme se o código realmente espera essa grafia: `SECRECT` parece um erro de digitação e talvez o nome correto no código seja `API_SECRET_KEY`. Use o nome referenciado na configuração/código da aplicação e, se corrigir a grafia, atualize o código e este arquivo de forma consistente.
- **`ENCRYPTION_KEY`**: Gere uma chave criptograficamente aleatória com o tamanho e a codificação exigidos pela implementação de criptografia no código. Por exemplo, `openssl rand -base64 32` gera 32 bytes aleatórios codificados em Base64, mas use esse formato somente se a aplicação esperar Base64 e uma chave de 32 bytes. Não use uma frase comum nem uma chave copiada de outro ambiente. Guarde uma cópia segura: perder essa chave pode impossibilitar a recuperação dos dados criptografados com ela.

Confira no código/configuração os requisitos de tamanho e codificação das chaves, além do host, porta e TLS do SMTP; esses requisitos dependem da aplicação e do provedor. Não envie o `.env` para o repositório nem compartilhe seu conteúdo. Confirme que `.env` está listado no `.gitignore`; fora do desenvolvimento local, prefira um gerenciador de segredos ou variáveis de ambiente do ambiente de implantação. Se algum segredo for exposto, revogue-o e gere outro.

## Antes de Executar o Projeto

Execute manualmente o script `00-create-migrations-table.sql` no banco de dados antes de prosseguir. O arquivo está no diretório `Database/`.

## Executando o Projeto

### 1. Inicie o banco Oracle

Na raiz do projeto, inicie a configuração compose existente:

```bash
docker compose up -d
```

### 2. Restaure as dependências

```bash
dotnet restore
```

### 3. Execute a aplicação

```bash
dotnet run --urls "http://0.0.0.0:8081"
```

### 4. Acesse a aplicação

Abra [http://localhost:8081/login](http://localhost:8081/login) no navegador.

## Observações

- O nome de serviço Oracle na configuração compose incluída é `FREE`.
- A senha de exemplo do banco no compose é `123` e serve apenas para desenvolvimento local; substitua-a por uma senha forte quando apropriado.
- O Oracle fica disponível na porta `1521`; a aplicação é executada na porta `8081`.
- Certifique-se de que o Docker esteja em execução e que o contêiner do banco esteja pronto antes de iniciar a aplicação.
- Confira se a string de conexão e as variáveis de ambiente correspondem à configuração da aplicação.
