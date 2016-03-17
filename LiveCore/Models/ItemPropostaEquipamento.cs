using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LiveCore.Models
{
    [Table("ITEMPROPOSTAEQUIP")]
    public class ItemPropostaEquipamento 
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ItemPropostaEquipID { get; set; }

        [Display(Name = "Tipo de Contrato")]
        public int TipoContratoID { get; set; }

        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; }

        [Display(Name = "Valor Unitário")]
        [DataType(DataType.Currency)]
        public Decimal ValorUnitario { get; set; }

        [Display(Name = "Equipamento")]
        [Range(typeof(Int32), "1", "9999", ErrorMessage = "Favor inserir um número positivo.")]
        public int EquipamentoID { get; set; }

        [Display(Name = "Proposta")]
        public int PropostaID { get; set; }

        [Display(Name = "Area de Escopo")]
        public int AreaEscopoID { get; set; }

        [Display(Name = "Moeda")]
        public String Moeda { get; set; }

        [Display(Name = "Tipo de Contrato")]
        public virtual TipoContrato TipoContrato { get; set; }
        [Display(Name = "Equipamento")]
        public virtual Equipamento Equipamento { get; set; }
        public virtual AreaEscopo AreaEscopo { get; set; }
        //public virtual Proposta Proposta { get; set; }
    }
}