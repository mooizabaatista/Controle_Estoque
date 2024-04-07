using System.ComponentModel.DataAnnotations.Schema;

namespace ControleEstoque.Models
{
    public class Estoque
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int QtdInicial { get; set; }
        public int QtdEntrada { get; set; }
        public int QtdSaida { get; set; }
        public int EstoqueFinal { get; set; }

        public int ProdutoId { get; set; }
        public virtual Produto? Produto { get; set; }
    }
}
