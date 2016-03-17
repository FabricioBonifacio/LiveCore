using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LiveCore.Models
{
    [Table("HISTORICOPROPOSTA")]
    public class HistoricoProposta
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int HistoricoPropostaID { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime DataCriacao { get; set; }

        [Display(Name = "Descrição")]
        public String Descricao { get; set; }

        [Display(Name = "Proposta")]
        public int PropostaID { get; set; }

        [Display(Name = "Criado Por")]
        public String CriadoPor { get; set; }
        //public virtual Proposta Proposta { get; set; }

    }
}