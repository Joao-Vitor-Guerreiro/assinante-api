using AssinanteAPI.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssinanteAPI.Domain.Entities;

public class Assinante
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string NomeCompleto { get; set; }
    
    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public DateTime DataInicioAssinatura { get; set; }
    
    [Required]
    public PlanoAssinatura Plano { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal ValorMensal { get; set; }
    
    [Required]
    public StatusAssinatura Status { get; set; }
    
    // Propriedade calculada - não persistida no banco
    // Usei Math.Max para garantir resultado não negativo
    [NotMapped]
    public int TempoAssinaturaMeses
    {
        get
        {
            var hoje = DateTime.Today;
            var meses = (hoje.Year - DataInicioAssinatura.Year) * 12 + hoje.Month - DataInicioAssinatura.Month;
            
            // Ajuste fino para considerar dias exatos
            if (hoje.Day < DataInicioAssinatura.Day)
            {
                meses--;
            }
            
            return Math.Max(0, meses);
        }
    }

    public Assinante() { }

    public Assinante(string nomeCompleto, string email, DateTime dataInicioAssinatura, 
                    PlanoAssinatura plano, decimal valorMensal)
    {
        ValidarDados(nomeCompleto, email, dataInicioAssinatura, valorMensal);
        
        NomeCompleto = nomeCompleto.Trim();
        Email = email.ToLower().Trim();
        DataInicioAssinatura = dataInicioAssinatura;
        Plano = plano;
        ValorMensal = valorMensal;
        Status = StatusAssinatura.Ativo;
    }

    public void Atualizar(string nomeCompleto, string email, PlanoAssinatura plano, decimal valorMensal)
    {
        ValidarDados(nomeCompleto, email, DataInicioAssinatura, valorMensal);
        
        NomeCompleto = nomeCompleto.Trim();
        Email = email.ToLower().Trim();
        Plano = plano;
        ValorMensal = valorMensal;
    }

    public void Desativar()
    {
        Status = StatusAssinatura.Inativo;
    }

    public void Reativar()
    {
        Status = StatusAssinatura.Ativo;
    }

    private void ValidarDados(string nomeCompleto, string email, DateTime dataInicioAssinatura, decimal valorMensal)
    {
        // Validações básicas de negócio
        if (string.IsNullOrWhiteSpace(nomeCompleto))
            throw new ArgumentException("Nome completo é obrigatório.");
            
        if (nomeCompleto.Length > 100)
            throw new ArgumentException("Nome completo não pode ter mais de 100 caracteres.");
            
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-mail é obrigatório.");
            
        if (email.Length > 100)
            throw new ArgumentException("E-mail não pode ter mais de 100 caracteres.");
            
        // Validação de e-mail foi um desafio - usei MailAddress para robustez
        if (!IsValidEmail(email))
            throw new ArgumentException("E-mail em formato inválido.");
            
        // Regra crítica: não pode assinar no futuro
        if (dataInicioAssinatura > DateTime.Today)
            throw new ArgumentException("Data de início da assinatura não pode ser maior que a data atual.");
            
        // Validação de valor é straightforward mas essencial
        if (valorMensal <= 0)
            throw new ArgumentException("Valor mensal deve ser maior que 0.");
            
        // Esta validação foi a mais complexa - depende do cálculo dinâmico
        if (TempoAssinaturaMeses == 0)
            throw new ArgumentException("Tempo de assinatura não pode ser igual a 0.");
    }

    // Extraí validação de e-mail para método separado para reuso e testes
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
