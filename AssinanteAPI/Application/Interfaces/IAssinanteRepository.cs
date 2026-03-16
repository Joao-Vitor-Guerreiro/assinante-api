using AssinanteAPI.Application.DTOs;
using AssinanteAPI.Domain.Entities;

namespace AssinanteAPI.Application.Interfaces;

/// <summary>
/// Interface do repositório de assinantes
/// Implementei para desacopilar do Entity Framework e facilitar testes
/// </summary>
public interface IAssinanteRepository
{
    /// <summary>
    /// Busca assinante por ID - retorna null se não encontrar
    /// </summary>
    Task<Assinante?> ObterPorIdAsync(int id);
    
    /// <summary>
    /// Busca assinante por e-mail - útil para validação de duplicidade
    /// </summary>
    Task<Assinante?> ObterPorEmailAsync(string email);
    
    /// <summary>
    /// Lista todos os assinantes ATIVOS (não inclui inativos)
    /// </summary>
    Task<List<Assinante>> ObterTodosAtivosAsync();
    
    /// <summary>
    /// Lista paginada de assinantes ativos - essencial para performance
    /// Retorna tupla com dados e total count
    /// </summary>
    Task<(List<Assinante> assinantes, int totalCount)> ObterTodosAtivosPaginadosAsync(int pageNumber, int pageSize);
    
    /// <summary>
    /// Lista TODOS os assinantes (inclusive inativos) - uso administrativo
    /// </summary>
    Task<List<Assinante>> ObterTodosAsync();
    
    /// <summary>
    /// Adiciona novo assinante ao banco
    /// </summary>
    Task AdicionarAsync(Assinante assinante);
    
    /// <summary>
    /// Atualiza dados do assinante existente
    /// </summary>
    Task AtualizarAsync(Assinante assinante);
    
    /// <summary>
    /// Remove fisicamente o assinante (hard delete)
    /// </summary>
    Task RemoverAsync(Assinante assinante);
    
    /// <summary>
    /// Verifica se e-mail já existe no sistema
    /// Parâmetro id opcional para permitir atualização do mesmo assinante
    /// </summary>
    Task<bool> ExisteEmailAsync(string email, int? id = null);
}

/// <summary>
/// Interface do serviço de aplicação
/// Camada de orquestração entre Controllers e Repositories
/// </summary>
public interface IAssinanteService
{
    /// <summary>
    /// Cria novo assinante com todas as validações
    /// </summary>
    Task<AssinanteResponseDto> CriarAsync(AssinanteCreateDto dto);
    
    /// <summary>
    /// Busca assinante por ID - só retorna se estiver ATIVO
    /// </summary>
    Task<AssinanteResponseDto> ObterPorIdAsync(int id);
    
    /// <summary>
    /// Lista assinantes com paginação - só ativos
    /// </summary>
    Task<PaginatedResult<AssinanteListDto>> ObterTodosAsync(int pageNumber, int pageSize);
    
    /// <summary>
    /// Atualiza dados do assinante (campos opcionais)
    /// </summary>
    Task<AssinanteResponseDto> AtualizarAsync(int id, AssinanteUpdateDto dto);
    
    /// <summary>
    /// Desativa assinante (soft delete) - mantém no banco mas não lista mais
    /// </summary>
    Task DesativarAsync(int id);
    
    /// <summary>
    /// Exclui assinante permanentemente (hard delete)
    /// </summary>
    Task ExcluirAsync(int id);
}
