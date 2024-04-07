using ControleEstoque.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
            
        }

        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=sql.bsite.net\MSSQL2016;Database=controleestoque_SampleDB;User Id=controleestoque_SampleDB;Password=controleestoque_SampleDB;;TrustServerCertificate=true");
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<FrenteServico> FrenteServicos { get; set; }
        public DbSet<Movimentacao> Movimentacoes { get; set; }
        public DbSet<Estoque> Estoque { get; set; }
    }
}
