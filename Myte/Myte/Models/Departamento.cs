using System.ComponentModel.DataAnnotations;

namespace Myte.Models
{
    public class Departamento
    {
        [Required]
        [Display(Name = "ID")]
        public int DepartamentoId { get; set; }

        [Required(ErrorMessage = "O nome do departamento é obrigatório")]
        [Display(Name = "Departamento")]
        public string? DepartamentoNome { get; set; }
    }
}
