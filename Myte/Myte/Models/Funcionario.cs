using Myte.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Myte.Models
{
    public class Funcionario
    {
        public int FuncionarioId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [Display(Name = "Nome Completo")]
        public string? FuncionarioNome { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [Display(Name = "E-mail")]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "A senha é obrigatória")]
        public string? Senha { get; set; }

        [Required(ErrorMessage = "A data de contratação é obrigatória")]
        [Display(Name = "Data De Contratação")]
        [DataType(DataType.Date)]
        public DateTime DataContratacao { get; set; }



        [Display(Name = "Departamento")]
        public Departamento? Departamento { get; set; }

        [Display(Name = "DepartamentoId")]
        public int DepartamentoId { get; set; }



        [Display(Name = "Nível de Acesso")]
        public string? NivelAcesso { get; set; } = "Funcionario";



        [Required(ErrorMessage = "O status do funcionário é obrigatório")]
        [Display(Name = "Status")]
        public FuncionarioStatus Status { get; set; }


        public ICollection<Registro>? Registros { get; set; }
    }
}
