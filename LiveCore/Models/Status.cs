using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LiveCore.Models
{
    [Table("STATUS")]
    public class Status
    {
        [Key]
        [Display(Name = "Sigla")]
        public String StatusSigla { get; set; }

        [Display(Name = "Nome")]
        [StringLength(50, ErrorMessage = "Máximo de 50 caracteres")]
        public String Nome { get; set; } 
    }
}