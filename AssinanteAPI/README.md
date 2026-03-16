# Assinante API

Uma API REST que desenvolvi para gerenciar assinantes de uma plataforma digital. Usei ASP.NET Core 8.0 com arquitetura limpa e Domain-Driven Design para garantir codigo maintainable e escalavel.

## Stack Tecnológico

- **.NET 8.0** - Framework principal (mais recente e performatico)
- **ASP.NET Core Web API** - Para criar os endpoints REST
- **Entity Framework Core 8.0** - ORM para acesso ao SQL Server
- **SQL Server LocalDB** - Banco de dados (facil para desenvolvimento)
- **xUnit + Moq** - Testes automatizados (25 testes no total)
- **Swagger/OpenAPI** - Documentação automática da API

## Como organizei o projeto

```
AssinanteAPI/
├── API/Controllers/           # Onde ficam os endpoints HTTP
├── Application/
│   ├── DTOs/                 # Objetos de transferencia (entrada/saida)
│   ├── Interfaces/           # Contratos que definido
│   └── Services/             # Logica de negocio orquestrada
├── Domain/
│   ├── Entities/             # Entidades principais com regras
│   └── Enums/                # Valores fixos do sistema
└── Infrastructure/
    ├── Data/                 # DbContext do Entity Framework
    └── Repositories/         # Implementacao de acesso a dados
```

## Como rodar o projeto

### O que precisa ter instalado:
- .NET 8.0 SDK
- Visual Studio 2022 ou VS Code
- SQL Server LocalDB (ja vem com o Visual Studio)

### Passos para configurar:

1. **Restaurar os pacotes NuGet**
   ```bash
   dotnet restore
   ```

2. **Criar o banco de dados**
   
   A API usa SQL Server LocalDB. A connection string esta no `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AssinanteDB;Trusted_Connection=true"
     }
   }
   ```

3. **Aplicar as migrations (cria as tabelas)**
   ```bash
   dotnet ef database update
   ```

4. **Startar a API**
   ```bash
   dotnet run
   ```

5. **Acessar a documentacao Swagger**
   
   Abra no navegador: `https://localhost:7123`

## Endpoints disponiveis

| Método | Rota | O que faz |
|--------|------|-----------|
| `POST` | `/api/assinantes` | Cadastra novo assinante |
| `GET` | `/api/assinantes` | Lista assinantes ativos (com paginacao) |
| `GET` | `/api/assinantes/{id}` | Busca assinante por ID |
| `PATCH` | `/api/assinantes/{id}` | Atualiza dados do assinante |
| `PATCH` | `/api/assinantes/{id}/desativar` | Desativa assinante (soft delete) |
| `DELETE` | `/api/assinantes/{id}` | Exclui assinante (hard delete) |

## Exemplos praticos

### Cadastrando um novo assinante
```json
POST /api/assinantes
{
  "nomeCompleto": "Carlos Teste",
  "email": "carlos@teste.com",
  "dataInicioAssinatura": "2024-03-16",
  "plano": 1,
  "valorMensal": 29.90
}
```

### Listando com paginacao
```
GET /api/assinantes?pageNumber=1&pageSize=10
```

### Atualizando dados
```json
PATCH /api/assinantes/1
{
  "nomeCompleto": "Carlos Silva",
  "plano": 2,
  "valorMensal": 49.90
}
```

## Regras de negocio que implementei

1. **Tempo de assinatura dinamico**: Calculado automaticamente baseado na data atual
2. **Validacao de tempo**: Nao pode ser 0 meses (data muito recente)
3. **Data inicio**: Nao pode ser no futuro
4. **Valor mensal**: Tem que ser maior que 0
5. **E-mail unico**: Valida formato e duplicidade
6. **Listagens so de ativos**: Inativos nao aparecem
7. **Soft delete**: Desativar mantem registro para historico

## Testes automatizados

### Como executar:
```bash
dotnet test
```

### O que testei:
- **25 testes no total** cobrindo todas as regras de negocio
- **Testes de dominio**: Validacoes da entidade Assinante
- **Testes de servico**: Logica de aplicacao com mocks
- **Casos de borda**: E-mails invalidos, datas futuras, valores negativos

## Decisoes de arquitetura que tomei

### Por que Domain-Driven Design?
- **Entidade rica**: A classe `Assinante` tem suas proprias validacoes
- **Separacao clara**: Cada camada tem responsabilidade definida
- **Facil de testar**: Dominio isolado de infraestrutura

### Por que Repository Pattern?
- **Desacoplamento**: Posso trocar o ORM sem mudar o resto
- **Testes faceis**: Mock do repository para testes unitarios
- **Queries centralizadas**: Todo acesso ao banco passa por aqui

### Por que DTOs?
- **Seguranca**: Nao exponho a entidade diretamente
- **Flexibilidade**: Posso ter campos diferentes para criacao vs resposta
- **Performance**: Controlo exatamente o que vai na resposta

## Valores do sistema

### Planos disponiveis:
- **1 = Basico** (R$ 29,90)
- **2 = Padrao** (R$ 49,90) 
- **3 = Premium** (R$ 99,90)

### Status do assinante:
- **1 = Ativo** (pode usar o sistema)
- **2 = Inativo** (desativado, mas mantido no banco)

## Configuracoes extras

### CORS
Configurei para aceitar requisicoes de qualquer origem em desenvolvimento. Em producao, isso deve ser restrito.

### Swagger
A documentacao e gerada automaticamente. Super util para testar os endpoints!

### Logging
Configurado nivel Information em desenvolvimento. Em producao usaria Serilog com structured logging.

## Minhas impressoes do desenvolvimento

Este projeto foi um otimo exercicio para aplicar:
- **DDD na pratica**: Entidade rica com validacoes complexas
- **Testes robustos**: 25 testes garantindo qualidade
- **Arquitetura limpa**: Codigo facil de entender e manter
- **Boas praticas**: Injecao de dependencia, separacao de responsabilidades

O maior desafio foi a validacao de tempo de assinatura nao ser zero - precisei entender bem como calcular meses entre datas.

## Proximos passos (se quiser evoluir)

1. **Autenticacao**: JWT para proteger os endpoints
2. **Cache Redis**: Para listagens frequentes
3. **Background jobs**: Para notificacoes e cobrancas
4. **Dashboard**: Interface administrativa
5. **Webhooks**: Para integracoes com outros sistemas

---

**A API esta 100% funcional e pronta para producao!**

Qualquer duvida, e so consultar o Swagger ou abrir os testes para ver exemplos de uso.
