﻿using Myte.Data;
using Myte.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Myte.Models
{
    public class WBS
    {
        public int WBSId { get; set; }

        [Required(ErrorMessage = "O código é obrigatório!")]
        [StringLength(10)]
        [RegularExpression(@"^[A-Za-z]{3}\d{7}$", ErrorMessage = "O código deve conter 3 letras seguidas de 7 números.")]
        [Display(Name = "Código WBS")]
        public string? Codigo { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória!")]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O Tipo é obrigatório!")]
        [Display(Name = "Tipo")] 
        public TiposWBS Tipos { get; set; }

        public ICollection<Registro>? Registros { get; set; }
    }
}
