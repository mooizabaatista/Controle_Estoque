using System.ComponentModel.DataAnnotations.Schema;

namespace ControleEstoque.Models
{
    public class FrenteServico
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
}
