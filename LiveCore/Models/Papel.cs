using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LiveCore.Models
{
    [Table("PAPEL")]
    public class Papel
    {
        [Key]
        [Display(Name = "ID")]
        public int PapelID { get; set; }

        [Display(Name = "Nome")]
        public String Nome { get; set; }

        [Display(Name = "Descrição")]
        public String Descricao { get; set; }


        public virtual ICollection<Entidade> Entidades { get; set; }
    }
}