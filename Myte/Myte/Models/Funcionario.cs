using System.ComponentModel.DataAnnotations;

namespace Myte.Models
{
    public class Funcionario
    {
        [Display(Name = "ID")]
        public int FuncionarioId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [Display(Name = "Nome")]
        public string? FuncionarioNome { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "O email é obrigatório")]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "A senha é obrigatória")]
        public string? Senha { get; set; }

        [Required(ErrorMessage = "A data de contratação é obrigatória")]
        [Display(Name = "Data De Contratação")]
        public DateTime DataContratacao { get; set; }

        [Display(Name = "Departamento")]
        public Departamento? Departamento { get; set; }

        [Display(Name = "DepartamentoId")]
        public int DepartamentoId { get; set; }

        [Display(Name = "Nível de Acesso")]
        public string? NivelAcesso { get; set; } = "Funcionario";

        [Display(Name = "Status")]
        public string? StatusFunc { get; set; } = "Ativo";

        public ICollection<Registro>? Registros { get; set; }
    }
}
