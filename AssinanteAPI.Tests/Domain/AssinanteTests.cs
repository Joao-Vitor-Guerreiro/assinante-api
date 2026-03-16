using AssinanteAPI.Domain.Entities;
using AssinanteAPI.Domain.Enums;

namespace AssinanteAPI.Tests.Domain;

public class AssinanteTests
{
    [Fact]
    public void Assinante_Constructor_ComDadosValidados_DeveCriarAssinante()
    {
        // Arrange
        var nome = "João Silva";
        var email = "joao@teste.com";
        var dataInicio = new DateTime(2023, 01, 15);
        var plano = PlanoAssinatura.Basico;
        var valor = 29.90m;

        // Act
        var assinante = new Assinante(nome, email, dataInicio, plano, valor);

        // Assert
        Assert.Equal(nome, assinante.NomeCompleto);
        Assert.Equal(email.ToLower(), assinante.Email);
        Assert.Equal(dataInicio, assinante.DataInicioAssinatura);
        Assert.Equal(plano, assinante.Plano);
        Assert.Equal(valor, assinante.ValorMensal);
        Assert.Equal(StatusAssinatura.Ativo, assinante.Status);
        Assert.True(assinante.TempoAssinaturaMeses > 0);
    }

    [Fact]
    public void Assinante_Constructor_ComNomeVazio_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Assinante("", "joao@teste.com", DateTime.Now.AddMonths(-1), PlanoAssinatura.Basico, 29.90m));
    }

    [Fact]
    public void Assinante_Constructor_ComEmailInvalido_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Assinante("João Silva", "email-invalido", DateTime.Now.AddMonths(-1), PlanoAssinatura.Basico, 29.90m));
    }

    [Fact]
    public void Assinante_Constructor_ComDataFutura_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Assinante("João Silva", "joao@teste.com", DateTime.Now.AddDays(1), PlanoAssinatura.Basico, 29.90m));
    }

    [Fact]
    public void Assinante_Constructor_ComValorZero_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Assinante("João Silva", "joao@teste.com", DateTime.Now.AddMonths(-1), PlanoAssinatura.Basico, 0));
    }

    [Fact]
    public void Assinante_Constructor_ComDataAmanha_DeveLancarArgumentException()
    {
        // Arrange & Act & Assert
        // Data de amanhã deve lançar exceção na validação de data
        Assert.Throws<ArgumentException>(() => 
            new Assinante("João Silva", "joao@teste.com", DateTime.Today.AddDays(1), PlanoAssinatura.Basico, 29.90m));
    }

    [Fact]
    public void Assinante_Desativar_DeveMudarStatusParaInativo()
    {
        // Arrange
        var assinante = new Assinante("João Silva", "joao@teste.com", DateTime.Now.AddMonths(-1), PlanoAssinatura.Basico, 29.90m);

        // Act
        assinante.Desativar();

        // Assert
        Assert.Equal(StatusAssinatura.Inativo, assinante.Status);
    }

    [Fact]
    public void Assinante_Reativar_DeveMudarStatusParaAtivo()
    {
        // Arrange
        var assinante = new Assinante("João Silva", "joao@teste.com", DateTime.Now.AddMonths(-1), PlanoAssinatura.Basico, 29.90m);
        assinante.Desativar();

        // Act
        assinante.Reativar();

        // Assert
        Assert.Equal(StatusAssinatura.Ativo, assinante.Status);
    }

    [Fact]
    public void Assinante_TempoAssinaturaMeses_DeveCalcularCorretamente()
    {
        // Arrange
        var dataInicio = new DateTime(2023, 01, 15);
        var assinante = new Assinante("João Silva", "joao@teste.com", dataInicio, PlanoAssinatura.Basico, 29.90m);

        // Act
        var tempoMeses = assinante.TempoAssinaturaMeses;

        // Assert
        var mesesEsperados = (DateTime.Today.Year - dataInicio.Year) * 12 + DateTime.Today.Month - dataInicio.Month;
        if (DateTime.Today.Day < dataInicio.Day)
            mesesEsperados--;

        Assert.Equal(mesesEsperados, tempoMeses);
    }

    [Theory]
    [InlineData("joao@teste.com", true)]
    [InlineData("joao.silva@empresa.com.br", true)]
    [InlineData("joao@", false)]
    [InlineData("@teste.com", false)]
    [InlineData("joao.teste.com", false)]
    [InlineData("", false)]
    public void Assinante_ValidacaoEmail_DeveValidarFormato(string email, bool valido)
    {
        // Arrange & Act & Assert
        if (valido)
        {
            var assinante = new Assinante("João Silva", email, DateTime.Now.AddMonths(-1), PlanoAssinatura.Basico, 29.90m);
            Assert.Equal(email.ToLower(), assinante.Email);
        }
        else
        {
            Assert.Throws<ArgumentException>(() => 
                new Assinante("João Silva", email, DateTime.Now.AddMonths(-1), PlanoAssinatura.Basico, 29.90m));
        }
    }
}
