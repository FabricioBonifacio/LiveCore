using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LiveCore.Models
{
    [Table("AREAESCOPO")]
    public class AreaEscopo
    {
        [Key]
        [Display(Name = "ID")]
        public int AreaEscopoID { get; set; }

        [Display(Name = "Nome")]
        public String Nome { get; set; }

        [Display(Name = "Descrição")]
        public String Descricao { get; set; }


        //public virtual ICollection<Equipamento> Equipamentos { get; set; }
        //public virtual ICollection<Servico> Servicos { get; set; }
    }
}