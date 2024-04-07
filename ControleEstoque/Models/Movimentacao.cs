using System.ComponentModel.DataAnnotations.Schema;

namespace ControleEstoque.Models
{
    public class Movimentacao
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public string TipoMovimentacao { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public int ProdutoId { get; set; }
        public virtual Produto? Produto { get; set; }
        public int FrenteServicoId { get; set; }
        public virtual FrenteServico? FrenteServico { get; set; }
    }
}
