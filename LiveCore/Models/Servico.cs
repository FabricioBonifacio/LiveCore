using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LiveCore.Models
{
    [Table("SERVICO")]
    public class Servico
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ServicoID { get; set; }

        [Display(Name = "Nome")]
        [StringLength(50, ErrorMessage = "Máximo de 50 caracteres")]
        public String Nome { get; set; }

        [Display(Name = "Descrição")]
        public String Descricao { get; set; }

        [Display(Name = "Unidade")]
        public String UnidadeSigla { get; set; }

        [Display(Name = "Valor")]
        [DataType(DataType.Currency)]
        public Decimal Valor { get; set; }

        [Display(Name = "Valor em Dólar")]
        public Decimal ValorDolar { get; set; }


        public virtual Unidade Unidade { get; set; }
        public virtual ICollection<ItemPropostaServico> ItemPropostaServicos { get; set; }

    }
}