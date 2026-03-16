using AssinanteAPI.Application.DTOs;
using AssinanteAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AssinanteAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AssinantesController : ControllerBase
{
    private readonly IAssinanteService _assinanteService;

    public AssinantesController(IAssinanteService assinanteService)
    {
        _assinanteService = assinanteService;
    }

    /// <summary>
    /// Cadastra um novo assinante no sistema
    /// Validacoes importantes: e-mail unico, data nao futura, valor > 0
    /// </summary>
    /// <param name="dto">Dados do novo assinante</param>
    /// <returns>Assinante criado com ID gerado pelo banco</returns>
    [HttpPost]
    public async Task<ActionResult<AssinanteResponseDto>> CriarAssinante([FromBody] AssinanteCreateDto dto)
    {
        try
        {
            // Chama o service para criar com todas as validacoes
            var result = await _assinanteService.CriarAsync(dto);
            // Retorna 201 com location para o novo recurso
            return CreatedAtAction(nameof(ObterAssinantePorId), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            // Erros de validacao vao como 400 Bad Request
            return BadRequest(new { erro = ex.Message });
        }
    }

    /// <summary>
    /// Lista todos os assinantes ativos do sistema
    /// Implementei paginacao para nao sobrecarregar o banco com muitos dados
    /// </summary>
    /// <param name="pageNumber">Qual pagina quer ver (comeca em 1)</param>
    /// <param name="pageSize">Quantos registros por pagina (maximo 100)</param>
    /// <returns>Lista paginada com dados dos assinantes</returns>
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<AssinanteListDto>>> ListarAssinantes(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        // Validacao simples para evitar parametros absurdos
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        // Busca os dados paginados no service
        var result = await _assinanteService.ObterTodosAsync(pageNumber, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Busca um assinante especifico pelo ID
    /// So retorna assinantes ATIVOS - inativos nao aparecem
    /// </summary>
    /// <param name="id">ID unico do assinante</param>
    /// <returns>Dados completos do assinante ou erro se nao encontrado</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<AssinanteResponseDto>> ObterAssinantePorId(int id)
    {
        try
        {
            var result = await _assinanteService.ObterPorIdAsync(id);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    /// <summary>
    /// Edita um assinante ativo
    /// </summary>
    /// <param name="id">ID do assinante</param>
    /// <param name="dto">Dados atualizados do assinante</param>
    /// <returns>Assinante atualizado</returns>
    [HttpPatch("{id}")]
    public async Task<ActionResult<AssinanteResponseDto>> EditarAssinante(int id, [FromBody] AssinanteUpdateDto dto)
    {
        try
        {
            var result = await _assinanteService.AtualizarAsync(id, dto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            if (ex.Message.Contains("não encontrado"))
                return NotFound(new { erro = ex.Message });
            
            return BadRequest(new { erro = ex.Message });
        }
    }

    /// <summary>
    /// Desativa um assinante (soft delete)
    /// </summary>
    /// <param name="id">ID do assinante</param>
    /// <returns>Status da operacao</returns>
    [HttpPatch("{id}/desativar")]
    public async Task<ActionResult> DesativarAssinante(int id)
    {
        try
        {
            await _assinanteService.DesativarAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            // Tratamento personalizado dos erros de negocio
            if (ex.Message.Contains("nao encontrado"))
                return NotFound(new { erro = ex.Message });
            
            // Outros erros de validacao vao como Bad Request
            return BadRequest(new { erro = ex.Message });
        }
    }

    /// <summary>
    /// Remove um assinante permanentemente do sistema
    /// Use com cuidado - esta operacao nao pode ser desfeita!
    /// </summary>
    /// <param name="id">ID do assinante a ser excluido</param>
    /// <returns>NoContent se sucesso, ou erro se problema</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> ExcluirAssinante(int id)
    {
        try
        {
            // Chama o service para fazer a exclusao fisica
            await _assinanteService.ExcluirAsync(id);
            return NoContent(); // 204 - Sucesso sem conteudo
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }
}
