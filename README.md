# LudoLoca API

API do projeto **LudoLoca**, uma plataforma de aluguel de jogos de tabuleiro.

Este repositório contém o backend construído com ASP.NET Core Web API, autenticação via Identity + JWT + OAuth (Google/GitHub), e banco de dados PostgreSQL com Entity Framework Core.

## Arquitetura
por que? --> navegar até arquivo Backend\ARQUIVOS\decisoes.docx

| Camada         | Tecnologia                           | Justificativa                                                                  |
|----------------|--------------------------------------|--------------------------------------------------------------------------------|
| Frontend       | React + Next.js                      | SSR/SSG, grande comunidade, login social fácil                                |
| Backend        | ASP.NET Core Web API (C#)            | Seguro, performático, integração com EF Core e Identity                       |
| Banco de Dados | PostgreSQL (via Supabase ou Neon)    | Relacional, suporte a JSONB, gratuito para começar                            |
| Autenticação   | ASP.NET Identity + OAuth (Google/GitHub) | Controle total sobre contas, integração direta, seguro                       |
| CI/CD          | GitHub Actions                       | Fluxo main → release → develop, simples de configurar                         |
| Hospedagem     | Railway (API) + Vercel (Frontend)    | Deploy automático, gratuito para pequeno tráfego                              |

## Funcionalidades

- Autenticação via e-mail/senha ou login social
- Geração de JWT com claims de roles
- Controle de acesso via `[Authorize(Roles = "...")]`
- Documentação via Swagger com suporte a JWT
- Suporte pronto para OAuth (Google/GitHub)
- Enumeração traduzida de status de aluguel via endpoint protegido

## Configuração Local

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- PostgreSQL local ou Supabase (banco ainda não criado)
- Git

### Passos Iniciais

Clone o repositório:

```bash
git clone https://github.com/juliocsx/LudoLocaApi.git
cd Backend
```

Crie uma nova branch para desenvolvimento:

```bash
git checkout -b v0.1
```

Configure seu usuário local (apenas se ainda não tiver feito):

```bash
git config --global user.name "Seu Nome"
git config --global user.email "seu@email.com"
```

### Instale os pacotes necessários:

```bash
dotnet restore
```

### Configure o banco de dados

1. Crie um banco PostgreSQL local ou online (Supabase, Neon etc.)
2. Adicione a `ConnectionString` em `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=LudoLocaDb;Username=usuario;Password=senha"
}
```

### Variáveis JWT

Em `appsettings.json`, configure:

```json
"Jwt": {
  "Key": "CHAVE",
  "Issuer": "LudoLocaAPI"
}
```

### Rodar a aplicação:

```bash
dotnet ef database update
dotnet run
```

A API estará disponível em: https://localhost:5001 (HTTPS)

### Testar com Swagger

Acesse `/swagger` no navegador para visualizar e testar os endpoints com JWT.

## Autenticação e Autorização

- Endpoints protegidos usam `[Authorize]` e `[Authorize(Roles = "Admin")]`
- Tokens são gerados via `/api/account/login` e enviados no header:  
  `Authorization: Bearer SEU_TOKEN`

## Fluxo de Branches (CI/CD) (FUTURO)

- `develop`: branch principal de desenvolvimento
- `release`: preparação para releases
- `main`: branch de produção
- CI/CD: configurável via GitHub Actions (não incluído ainda)

## Próximos Passos

1. Criar as tabelas restantes no banco (jogos, usuários, aluguéis)
2. Criar os controllers REST (GameController, RentalController, UserController)
3. Adicionar validações com DTOs
4. Proteger os endpoints conforme roles
5. Criar o banco de dados real (seja local ou Supabase)
6. Escrever testes (opcional)
7. Configurar pipeline no GitHub Actions

## Contribuindo 

- Crie uma nova branch a partir de `develop`
- Submeta PR com descrição clara das mudanças

## Licença

Este projeto é de código aberto e segue os termos da licença MIT.