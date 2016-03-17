using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LiveCore.Models
{
    [Table("PROXIMOPASSOPROPOSTA")]
    public class ProximoPassoProposta
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ProximoPassoPropostaID { get; set; }

        [Display(Name = "Descrição")]
        public String Descricao { get; set; }

        [Display(Name = "Data da Criação")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataCriacao { get; set; }

        [Display(Name = "Data")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataAgendamento { get; set; }

        [Display(Name = "Status")]
        public String Status { get; set; }

        [Display(Name = "Proposta")]
        public int PropostaID { get; set; }

        [Display(Name = "Alerta")]
        public int TempoAlerta { get; set; }

        [Display(Name = "")]
        public String TipoAlerta { get; set; }


        [Display(Name = "Proposta")]
        public virtual Proposta Proposta { get; set; }
    }
}