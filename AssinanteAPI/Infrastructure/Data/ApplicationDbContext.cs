using AssinanteAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssinanteAPI.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Assinante> Assinantes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Assinante>(entity =>
        {
            entity.ToTable("Assinantes");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.NomeCompleto)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.ValorMensal)
                .HasPrecision(10, 2);

            entity.Property(e => e.Plano)
                .HasConversion<int>();

            entity.Property(e => e.Status)
                .HasConversion<int>()
                .HasDefaultValue(Domain.Enums.StatusAssinatura.Ativo);

            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("IX_Assinantes_Email");

            entity.Property(e => e.DataInicioAssinatura)
                .IsRequired();
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assinante>().HasData(
            new Assinante
            {
                Id = 1,
                NomeCompleto = "João Silva",
                Email = "joao.silva@email.com",
                DataInicioAssinatura = new DateTime(2023, 01, 15),
                Plano = Domain.Enums.PlanoAssinatura.Basico,
                ValorMensal = 29.90m,
                Status = Domain.Enums.StatusAssinatura.Ativo
            },
            new Assinante
            {
                Id = 2,
                NomeCompleto = "Maria Santos",
                Email = "maria.santos@email.com",
                DataInicioAssinatura = new DateTime(2023, 06, 10),
                Plano = Domain.Enums.PlanoAssinatura.Padrao,
                ValorMensal = 49.90m,
                Status = Domain.Enums.StatusAssinatura.Ativo
            },
            new Assinante
            {
                Id = 3,
                NomeCompleto = "Pedro Costa",
                Email = "pedro.costa@email.com",
                DataInicioAssinatura = new DateTime(2022, 12, 01),
                Plano = Domain.Enums.PlanoAssinatura.Premium,
                ValorMensal = 99.90m,
                Status = Domain.Enums.StatusAssinatura.Ativo
            }
        );
    }
}
