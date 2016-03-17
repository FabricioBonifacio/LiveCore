using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LiveCore.Models
{
    [Table("UNIDADE")]
    public class Unidade
    {
        [Key]
        [Display(Name = "Sigla")]
        public String UnidadeSigla { get; set; }

        [Display(Name = "Nome")]
        [StringLength(50, ErrorMessage = "Máximo de 50 caracteres")]
        public String Nome { get; set; }


        public virtual ICollection<Servico> Servicos { get; set; }
        public virtual ICollection<Equipamento> Equipamentos { get; set; }
    }
}