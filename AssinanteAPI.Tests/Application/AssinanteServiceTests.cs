using AssinanteAPI.Application.DTOs;
using AssinanteAPI.Application.Interfaces;
using AssinanteAPI.Application.Services;
using AssinanteAPI.Domain.Entities;
using AssinanteAPI.Domain.Enums;
using Moq;

namespace AssinanteAPI.Tests.Application;

public class AssinanteServiceTests
{
    private readonly Mock<IAssinanteRepository> _repositoryMock;
    private readonly GerenciadorAssinantesService _service;

    public AssinanteServiceTests()
    {
        _repositoryMock = new Mock<IAssinanteRepository>();
        _service = new GerenciadorAssinantesService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CriarAsync_ComDadosValidados_DeveCriarAssinante()
    {
        // Preparando os dados para criar um assinante válido
        var dto = new AssinanteCreateDto
        {
            NomeCompleto = "João Silva",
            Email = "joao@teste.com",
            DataInicioAssinatura = new DateTime(2023, 01, 15),
            Plano = PlanoAssinatura.Basico,
            ValorMensal = 29.90m
        };

        // Configurando o mock para simular que o e-mail não existe
        _repositoryMock.Setup(r => r.ExisteEmailAsync(dto.Email, null)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<Assinante>())).Returns(Task.CompletedTask);

        // Executando a criação do assinante
        var result = await _service.CriarAsync(dto);

        // Verificando se o resultado está correto
        Assert.NotNull(result);
        Assert.Equal(dto.NomeCompleto, result.NomeCompleto);
        Assert.Equal(dto.Email.ToLower(), result.Email);
        Assert.Equal(dto.Plano, result.Plano);
        Assert.Equal(dto.ValorMensal, result.ValorMensal);
        Assert.Equal(StatusAssinatura.Ativo, result.Status);

        // Garantindo que o repository foi chamado corretamente
        _repositoryMock.Verify(r => r.ExisteEmailAsync(dto.Email, null), Times.Once);
        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Assinante>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ComEmailExistente_DeveLancarArgumentException()
    {
        // Preparando dados com e-mail duplicado para testar validação
        var dto = new AssinanteCreateDto
        {
            NomeCompleto = "João Silva",
            Email = "joao@teste.com",
            DataInicioAssinatura = new DateTime(2023, 01, 15),
            Plano = PlanoAssinatura.Basico,
            ValorMensal = 29.90m
        };

        // Mockando para simular que o e-mail já existe
        _repositoryMock.Setup(r => r.ExisteEmailAsync(dto.Email, null)).ReturnsAsync(true);

        // Executando e verificando se lança a exceção correta
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarAsync(dto));
        Assert.Equal("E-mail já cadastrado no sistema.", exception.Message);

        _repositoryMock.Verify(r => r.ExisteEmailAsync(dto.Email, null), Times.Once);
        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Assinante>()), Times.Never);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdValido_DeveRetornarAssinante()
    {
        // Preparando um assinante válido com ID definido
        var id = 1;
        var assinante = new Assinante("João Silva", "joao@teste.com", DateTime.Now.AddMonths(-6), PlanoAssinatura.Basico, 29.90m)
        {
            Id = id
        };

        // Mockando o repository para retornar o assinante
        _repositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(assinante);

        // Executando a busca por ID
        var result = await _service.ObterPorIdAsync(id);

        // Verificando se o resultado está correto
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(assinante.NomeCompleto, result.NomeCompleto);

        // Garantindo que o repository foi chamado uma vez
        _repositoryMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComAssinanteInativo_DeveLancarArgumentException()
    {
        // Preparando um assinante inativo para testar a validação
        var id = 1;
        var assinante = new Assinante("João Silva", "joao@teste.com", DateTime.Now.AddMonths(-6), PlanoAssinatura.Basico, 29.90m);
        assinante.Desativar(); // Deixando o assinante inativo

        // Mockando o repository para retornar o assinante inativo
        _repositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(assinante);

        // Executando e verificando se lança a exceção correta
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.ObterPorIdAsync(id));
        Assert.Equal("Assinante não encontrado ou inativo.", exception.Message);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdInexistente_DeveLancarArgumentException()
    {
        // Preparando um ID que não existe no sistema
        var id = 999;
        _repositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Assinante?)null);

        // Executando e verificando se lança a exceção correta
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.ObterPorIdAsync(id));
        Assert.Equal("Assinante não encontrado ou inativo.", exception.Message);
    }

    [Fact]
    public async Task AtualizarAsync_ComDadosValidados_DeveAtualizarAssinante()
    {
        // Preparando dados para atualizar um assinante
        var id = 1;
        var assinante = new Assinante("João Silva", "joao@teste.com", DateTime.Now.AddMonths(-6), PlanoAssinatura.Basico, 29.90m);
        var dto = new AssinanteUpdateDto
        {
            NomeCompleto = "João Silva Santos",
            Email = "joao.santos@teste.com",
            Plano = PlanoAssinatura.Padrao,
            ValorMensal = 49.90m
        };

        // Configurando os mocks para simular o fluxo de atualização
        _repositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(assinante);
        _repositoryMock.Setup(r => r.ExisteEmailAsync(dto.Email, id)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<Assinante>())).Returns(Task.CompletedTask);

        // Executando a atualização
        var result = await _service.AtualizarAsync(id, dto);

        // Verificando se o resultado está correto
        Assert.NotNull(result);
        Assert.Equal(dto.NomeCompleto, result.NomeCompleto);
        Assert.Equal(dto.Email.ToLower(), result.Email);
        Assert.Equal(dto.Plano, result.Plano);
        Assert.Equal(dto.ValorMensal, result.ValorMensal);

        // Garantindo que o repository foi chamado corretamente
        _repositoryMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
        _repositoryMock.Verify(r => r.ExisteEmailAsync(dto.Email, id), Times.Once);
        _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Assinante>()), Times.Once);
    }

    [Fact]
    public async Task DesativarAsync_ComIdValido_DeveDesativarAssinante()
    {
        // Preparando um assinante para desativar
        var id = 1;
        var assinante = new Assinante("João Silva", "joao@teste.com", DateTime.Now.AddMonths(-6), PlanoAssinatura.Basico, 29.90m);

        // Configurando os mocks para simular a desativação
        _repositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(assinante);
        _repositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<Assinante>())).Returns(Task.CompletedTask);

        // Executando a desativação
        await _service.DesativarAsync(id);

        // Verificando se o status foi alterado para inativo
        Assert.Equal(StatusAssinatura.Inativo, assinante.Status);

        _repositoryMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
        _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Assinante>()), Times.Once);
    }

    [Fact]
    public async Task ExcluirAsync_ComIdValido_DeveExcluirAssinante()
    {
        // Preparando o cenário para excluir um assinante
        var id = 1;
        var assinante = new Assinante("João Silva", "joao@teste.com", DateTime.Now.AddMonths(-6), PlanoAssinatura.Basico, 29.90m);

        // Mockando o repository para encontrar o assinante e permitir exclusão
        _repositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(assinante);
        _repositoryMock.Setup(r => r.RemoverAsync(It.IsAny<Assinante>())).Returns(Task.CompletedTask);

        // Executando a exclusão
        await _service.ExcluirAsync(id);

        // Verificando se o repository foi chamado corretamente
        _repositoryMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
        _repositoryMock.Verify(r => r.RemoverAsync(It.IsAny<Assinante>()), Times.Once);
    }

    [Fact]
    public async Task ObterTodosAsync_ComPaginacao_DeveRetornarResultadoPaginado()
    {
        // Preparando dados de teste para paginação
        var pageNumber = 1;
        var pageSize = 10;
        var assinantes = new List<Assinante>
        {
            new("João Silva", "joao@teste.com", DateTime.Now.AddMonths(-6), PlanoAssinatura.Basico, 29.90m),
            new("Maria Santos", "maria@teste.com", DateTime.Now.AddMonths(-3), PlanoAssinatura.Padrao, 49.90m)
        };

        // Configurando o mock para retornar os dados paginados
        _repositoryMock.Setup(r => r.ObterTodosAtivosPaginadosAsync(pageNumber, pageSize))
            .ReturnsAsync((assinantes, assinantes.Count));

        // Executando o método de listagem paginada
        var result = await _service.ObterTodosAsync(pageNumber, pageSize);

        // Verificando se o resultado está correto
        Assert.NotNull(result);
        Assert.Equal(assinantes.Count, result.Data.Count);
        Assert.Equal(pageNumber, result.PageNumber);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Equal(assinantes.Count, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
        Assert.False(result.HasPreviousPage);
        Assert.False(result.HasNextPage);

        _repositoryMock.Verify(r => r.ObterTodosAtivosPaginadosAsync(pageNumber, pageSize), Times.Once);
    }
}
