# Project Setup Guide

[🇺🇸 English](#project-setup-guide) | [🇧🇷 Português](#guia-de-configuração-do-projeto)

## Prerequisites

- Docker and Docker Compose installed.
- .NET SDK installed.
- A web browser.

## Creating an Oracle Database Instance Using Docker

### `docker-compose.yml` Example

Create a `docker-compose.yml` file in the project root directory with the following configuration:

```yaml
services:
  oracle_db:
    image: gvenzl/oracle-free
    ports:
      - "1521:1521"
    environment:
      - ORACLE_PASSWORD=123
    volumes:
      - data:/opt/oracle/oradata

volumes:
  data:
```

## Configuring the Database Connection String

1. Open the `appsettings.json` file in the project.
2. Configure the `ConnectionStrings` section with your database credentials and connection details.

Example:

```json
{
  "ConnectionStrings": {
    "OracleDb": "User Id=<your_database_user>;Password=<your_database_password>;Data Source=localhost:1521/<your_database_name>;"
  }
}
```

Replace the following placeholders with your Oracle database configuration:

- `<your_database_user>`: Your Oracle database username.
- `<your_database_password>`: Your Oracle database password.
- `<your_database_name>`: Your Oracle database service name.

Make sure the connection string matches your Oracle database configuration.

## Configuring the `.env` File

The application requires environment variables for email configuration, API security, and encryption.

### 1. Create the `.env` File

Create a `.env` file in the project root directory.

On Linux or macOS, you can run:

```bash
touch .env
```

On Windows, you can create the file manually in the project root directory.

### 2. Define the Environment Variables

Add the following variables to your `.env` file:

```dotenv
EMAIL_FROM=<your_email>
EMAIL_PASSWORD=<your_email_password>
API_SECRECT_KEY=<your_secret_key>
ENCRYPTION_KEY=<your_encryption_key>
```

Replace the placeholders with your actual configuration values.

**Important:** Make sure the environment variable names match those expected by the application. In particular, verify whether `API_SECRECT_KEY` is the intended name or whether it should be `API_SECRET_KEY`.

Keep your `.env` file private and do not commit it to version control. Add `.env` to your `.gitignore` file to prevent accidentally exposing credentials and secrets.

## Running the Project

### 1. Start the Oracle Database

Start the database container using Docker Compose:

```bash
docker-compose up -d
```

### 2. Restore Dependencies

Restore the project's .NET dependencies:

```bash
dotnet restore
```

### 3. Run the Application

Start the application on port `8081`:

```bash
dotnet run --urls "http://0.0.0.0:8081"
```

### 4. Open the Application

Open your web browser and navigate to:

http://localhost:8081/login

## Notes

- The default Oracle database password in the Docker Compose example is `123`.
- The Oracle database is accessible on port `1521`.
- The application runs on port `8081`.
- Make sure Docker is running before starting the database.
- Ensure that the database connection string and environment variables are correctly configured before running the application.
- The example credentials are intended for local development only. Use secure credentials in production.

---

# Guia de Configuração do Projeto

[🇺🇸 English](#project-setup-guide) | [🇧🇷 Português](#guia-de-configuração-do-projeto)

## Pré-requisitos

- Docker e Docker Compose instalados.
- .NET SDK instalado.
- Um navegador web.

## Criando uma Instância do Oracle Database com Docker

### Exemplo de `docker-compose.yml`

Crie um arquivo `docker-compose.yml` no diretório raiz do projeto com a seguinte configuração:

```yaml
services:
  oracle_db:
    image: gvenzl/oracle-free
    ports:
      - "1521:1521"
    environment:
      - ORACLE_PASSWORD=123
    volumes:
      - data:/opt/oracle/oradata

volumes:
  data:
```

## Configurando a String de Conexão com o Banco de Dados

1. Abra o arquivo `appsettings.json` do projeto.
2. Configure a seção `ConnectionStrings` com as credenciais e os dados de conexão do seu banco de dados.

Exemplo:

```json
{
  "ConnectionStrings": {
    "OracleDb": "User Id=<seu_usuario_do_banco>;Password=<sua_senha_do_banco>;Data Source=localhost:1521/<nome_do_servico_do_banco>;"
  }
}
```

Substitua os seguintes campos pelos dados do seu banco de dados Oracle:

- `<seu_usuario_do_banco>`: Nome de usuário do banco de dados Oracle.
- `<sua_senha_do_banco>`: Senha do banco de dados Oracle.
- `<nome_do_servico_do_banco>`: Nome do serviço do banco de dados Oracle.

Certifique-se de que a string de conexão corresponde à configuração do seu banco de dados Oracle.

## Configurando o Arquivo `.env`

A aplicação utiliza variáveis de ambiente para configurar o envio de e-mails, a segurança da API e a criptografia.

### 1. Crie o Arquivo `.env`

Crie um arquivo `.env` no diretório raiz do projeto.

No Linux ou macOS, você pode executar:

```bash
touch .env
```

No Windows, você pode criar o arquivo manualmente no diretório raiz do projeto.

### 2. Defina as Variáveis de Ambiente

Adicione as seguintes variáveis ao arquivo `.env`:

```dotenv
EMAIL_FROM=<seu_email>
EMAIL_PASSWORD=<sua_senha_de_email>
API_SECRECT_KEY=<sua_chave_secreta>
ENCRYPTION_KEY=<sua_chave_de_criptografia>
```

Substitua os valores de exemplo pelas suas configurações reais.

**Importante:** Os nomes das variáveis de ambiente devem corresponder aos nomes esperados pela aplicação. Em particular, verifique se `API_SECRECT_KEY` é o nome correto ou se deveria ser `API_SECRET_KEY`.

Mantenha o arquivo `.env` privado e não o envie para o repositório. Adicione `.env` ao arquivo `.gitignore` para evitar a exposição acidental de credenciais e informações sensíveis.

## Executando o Projeto

### 1. Inicie o Banco de Dados Oracle

Inicie o contêiner do banco de dados usando o Docker Compose:

```bash
docker-compose up -d
```

### 2. Restaure as Dependências

Restaure as dependências .NET do projeto:

```bash
dotnet restore
```

### 3. Execute a Aplicação

Inicie a aplicação na porta `8081`:

```bash
dotnet run --urls "http://0.0.0.0:8081"
```

### 4. Acesse a Aplicação

Abra o navegador e acesse:

http://localhost:8081/login

## Observações

- A senha padrão do Oracle Database no exemplo do Docker Compose é `123`.
- O banco de dados Oracle estará acessível na porta `1521`.
- A aplicação será executada na porta `8081`.
- Certifique-se de que o Docker esteja em execução antes de iniciar o banco de dados.
- Verifique se a string de conexão e as variáveis de ambiente estão configuradas corretamente antes de executar a aplicação.
- As credenciais de exemplo são destinadas apenas ao ambiente de desenvolvimento local. Utilize credenciais seguras em produção.
