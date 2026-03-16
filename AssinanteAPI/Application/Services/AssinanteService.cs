using AssinanteAPI.Application.DTOs;
using AssinanteAPI.Application.Interfaces;
using AssinanteAPI.Domain.Entities;

namespace AssinanteAPI.Application.Services;

public class GerenciadorAssinantesService : IAssinanteService
{
    private readonly IAssinanteRepository _assinanteRepository;

    // Decidi usar Repository Pattern para facilitar testes unitários
    // e desacopilar a lógica de negócio do Entity Framework
    public GerenciadorAssinantesService(IAssinanteRepository assinanteRepository)
    {
        _assinanteRepository = assinanteRepository;
    }

    public async Task<AssinanteResponseDto> CriarAsync(AssinanteCreateDto dto)
    {
        // Validação de e-mail único é crucial para evitar duplicidade
        // Esta validação acontece antes de criar a entidade para performance
        if (await _assinanteRepository.ExisteEmailAsync(dto.Email))
        {
            throw new ArgumentException("E-mail já cadastrado no sistema.");
        }

        // A entidade Assinante já contém suas próprias validações
        // Isso segue o princípio de entidade rica do DDD
        var assinante = new Assinante(
            dto.NomeCompleto,
            dto.Email,
            dto.DataInicioAssinatura,
            dto.Plano,
            dto.ValorMensal
        );

        await _assinanteRepository.AdicionarAsync(assinante);

        return MapearParaResponseDto(assinante);
    }

    public async Task<AssinanteResponseDto> ObterPorIdAsync(int id)
    {
        var assinante = await _assinanteRepository.ObterPorIdAsync(id);

        if (assinante == null || assinante.Status == Domain.Enums.StatusAssinatura.Inativo)
        {
            throw new ArgumentException("Assinante não encontrado ou inativo.");
        }

        return MapearParaResponseDto(assinante);
    }

    public async Task<PaginatedResult<AssinanteListDto>> ObterTodosAsync(int pageNumber, int pageSize)
    {
        // Implementei paginação para evitar sobrecarga do banco
        // e melhorar performance com grandes volumes de dados
        var (assinantes, totalCount) = await _assinanteRepository.ObterTodosAtivosPaginadosAsync(pageNumber, pageSize);

        // Mapeio manual para controle preciso dos dados expostos
        var dtos = assinantes.Select(MapearParaListDto).ToList();

        return new PaginatedResult<AssinanteListDto>
        {
            Data = dtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    public async Task<AssinanteResponseDto> AtualizarAsync(int id, AssinanteUpdateDto dto)
    {
        var assinante = await _assinanteRepository.ObterPorIdAsync(id);

        if (assinante == null || assinante.Status == Domain.Enums.StatusAssinatura.Inativo)
        {
            throw new ArgumentException("Assinante não encontrado ou inativo.");
        }

        if (await _assinanteRepository.ExisteEmailAsync(dto.Email, id))
        {
            throw new ArgumentException("E-mail já cadastrado para outro assinante.");
        }

        assinante.Atualizar(dto.NomeCompleto, dto.Email, dto.Plano, dto.ValorMensal);

        await _assinanteRepository.AtualizarAsync(assinante);

        return MapearParaResponseDto(assinante);
    }

    public async Task DesativarAsync(int id)
    {
        var assinante = await _assinanteRepository.ObterPorIdAsync(id);

        if (assinante == null || assinante.Status == Domain.Enums.StatusAssinatura.Inativo)
        {
            throw new ArgumentException("Assinante não encontrado ou já inativo.");
        }

        assinante.Desativar();

        await _assinanteRepository.AtualizarAsync(assinante);
    }

    public async Task ExcluirAsync(int id)
    {
        var assinante = await _assinanteRepository.ObterPorIdAsync(id);

        if (assinante == null)
        {
            throw new ArgumentException("Assinante não encontrado.");
        }

        await _assinanteRepository.RemoverAsync(assinante);
    }

    private static AssinanteResponseDto MapearParaResponseDto(Assinante assinante)
    {
        return new AssinanteResponseDto
        {
            Id = assinante.Id,
            NomeCompleto = assinante.NomeCompleto,
            Email = assinante.Email,
            DataInicioAssinatura = assinante.DataInicioAssinatura,
            Plano = assinante.Plano,
            ValorMensal = assinante.ValorMensal,
            Status = assinante.Status,
            TempoAssinaturaMeses = assinante.TempoAssinaturaMeses
        };
    }

    private static AssinanteListDto MapearParaListDto(Assinante assinante)
    {
        return new AssinanteListDto
        {
            Id = assinante.Id,
            NomeCompleto = assinante.NomeCompleto,
            Email = assinante.Email,
            DataInicioAssinatura = assinante.DataInicioAssinatura,
            Plano = assinante.Plano,
            ValorMensal = assinante.ValorMensal,
            Status = assinante.Status,
            TempoAssinaturaMeses = assinante.TempoAssinaturaMeses
        };
    }
}
