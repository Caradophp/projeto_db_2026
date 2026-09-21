# Project Setup Guide

## Prerequisites

- Docker and Docker Compose installed
- .NET SDK installed
- A web browser

## Create an Oracle DB Instance Using Docker

### docker-compose.yml Example

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

## Running the Project

1. **Start the Oracle database:**
   ```bash
   docker-compose up -d
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Run the application:**
   ```bash
   dotnet run --urls "http://0.0.0.0:8081"
   ```

4. **Open in browser:**
   Navigate to `http://localhost:8081/login`

## Notes

- Default Oracle password: `123`
- Database will be accessible on port `1521`
- Application runs on port `8081`