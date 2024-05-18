﻿using System.ComponentModel.DataAnnotations;

namespace Myte.Models
{
    public class Registro
    {
        [Display(Name = "Registro-ID")]
        public int RegistroId { get; set; }

        [Display(Name = "Funcionário")]
        public Funcionario? Funcionario { get; set; }

        public int FuncionarioId { get; set; }

        [Display(Name = "Código WBS")]
        public WBS? WBS { get; set; }

        public int WBSId { get; set; }

        [Display(Name = "Horas Trabalhadas")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Range(8, 12, ErrorMessage = "O valor deve estar entre 8 e 12 horas.")]
        public double HorasTrab { get; set; }

        [Display(Name = "Dia da Semana")]
        public DateTime DiaSemana { get; set; }
    }
}
