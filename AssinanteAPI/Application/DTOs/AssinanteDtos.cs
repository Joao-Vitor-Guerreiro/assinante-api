using AssinanteAPI.Domain.Enums;

namespace AssinanteAPI.Application.DTOs;

/// <summary>
/// DTO para criação de novos assinantes
/// Não inclui ID porque é gerado pelo banco
/// </summary>
public class AssinanteCreateDto
{
    /// <summary>
    /// Nome completo do assinante - obrigatório
    /// </summary>
    public string NomeCompleto { get; set; }
    
    /// <summary>
    /// E-mail para contato - será validado e verificado duplicidade
    /// </summary>
    public string Email { get; set; }
    
    /// <summary>
    /// Data de início da assinatura - não pode ser futura
    /// </summary>
    public DateTime DataInicioAssinatura { get; set; }
    
    /// <summary>
    /// Plano escolhido: 1=Básico, 2=Padrão, 3=Premium
    /// </summary>
    public PlanoAssinatura Plano { get; set; }
    
    /// <summary>
    /// Valor mensal - deve ser maior que 0
    /// </summary>
    public decimal ValorMensal { get; set; }
}

/// <summary>
/// DTO para atualização de assinantes existentes
/// Todos os campos são opcionais - atualiza só o que for informado
/// </summary>
public class AssinanteUpdateDto
{
    /// <summary>
    /// Nome completo (opcional)
    /// </summary>
    public string NomeCompleto { get; set; }
    
    /// <summary>
    /// Novo e-mail (opcional) - será verificado duplicidade
    /// </summary>
    public string Email { get; set; }
    
    /// <summary>
    /// Novo plano (opcional)
    /// </summary>
    public PlanoAssinatura Plano { get; set; }
    
    /// <summary>
    /// Novo valor mensal (opcional) - deve ser maior que 0
    /// </summary>
    public decimal ValorMensal { get; set; }
}

/// <summary>
/// DTO completo para resposta da API
/// Inclui todos os dados do assinante mais o tempo calculado
/// </summary>
public class AssinanteResponseDto
{
    /// <summary>
    /// ID único do assinante gerado pelo banco
    /// </summary>
    public int Id { get; set; }
    
    public string NomeCompleto { get; set; }
    public string Email { get; set; }
    public DateTime DataInicioAssinatura { get; set; }
    public PlanoAssinatura Plano { get; set; }
    public decimal ValorMensal { get; set; }
    public StatusAssinatura Status { get; set; }
    
    /// <summary>
    /// Tempo de assinatura em meses - calculado dinamicamente
    /// </summary>
    public int TempoAssinaturaMeses { get; set; }
}

/// <summary>
/// DTO otimizado para listagens
/// Mesmos campos do Response mas sem comentários detalhados
/// </summary>
public class AssinanteListDto
{
    public int Id { get; set; }
    public string NomeCompleto { get; set; }
    public string Email { get; set; }
    public DateTime DataInicioAssinatura { get; set; }
    public PlanoAssinatura Plano { get; set; }
    public decimal ValorMensal { get; set; }
    public StatusAssinatura Status { get; set; }
    public int TempoAssinaturaMeses { get; set; }
}

/// <summary>
/// Classe genérica para resultados paginados
/// Reuso para qualquer listagem paginada da API
/// </summary>
/// <typeparam name="T">Tipo dos dados being paginados</typeparam>
public class PaginatedResult<T>
{
    /// <summary>
    /// Dados da página atual
    /// </summary>
    public List<T> Data { get; set; }
    
    /// <summary>
    /// Número da página atual (começa em 1)
    /// </summary>
    public int PageNumber { get; set; }
    
    /// <summary>
    /// Quantos itens por página
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// Total de itens em todas as páginas
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Total de páginas calculado
    /// </summary>
    public int TotalPages { get; set; }
    
    /// <summary>
    /// Tem página anterior?
    /// </summary>
    public bool HasPreviousPage { get; set; }
    
    /// <summary>
    /// Tem próxima página?
    /// </summary>
    public bool HasNextPage { get; set; }
}
