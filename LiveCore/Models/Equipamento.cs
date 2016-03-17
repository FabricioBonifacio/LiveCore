using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LiveCore.Models
{
    [Table("EQUIPAMENTO")]
    public class Equipamento
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int EquipamentoID { get; set; }

        [Display(Name = "Nome")]
        [StringLength(50, ErrorMessage = "Máximo de 50 caracteres")]
        public String Nome { get; set; }

        [Display(Name = "Descrição")]
        public String Descricao { get; set; }

        [Display(Name = "Cod. Patrimônio")]
        public int? CodPatrimonio { get; set; }

        [Display(Name = "Unidade")]
        public String UnidadeSigla { get; set; }

        [Display(Name = "Fabricante")]
        public String Marca { get; set; }

        [Display(Name = "Part Number")]
        public String Referencia { get; set; }

        [Display(Name = "NCM")]
        public int? NCM { get; set; }

        [Display(Name = "Preço de Custo")]
        [DataType(DataType.Currency)]
        public Decimal PrecoCusto { get; set; }

        [Display(Name = "Preço de Venda")]
        [DataType(DataType.Currency)]
        public Decimal PrecoVenda { get; set; }

        [Display(Name = "Valor do Aluguel")]
        [DataType(DataType.Currency)]
        public Decimal ValorAluguel { get; set; }

        [Display(Name = "Vencimento da Garantia")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DtVencGarantia { get; set; }

        [Display(Name = "Preço de Custo em Dólar")]
        [DataType(DataType.Currency)]
        public Decimal PrecoCustoDolar { get; set; }

        [Display(Name = "Preço de Venda em Dólar")]
        [DataType(DataType.Currency)]
        public Decimal PrecoVendaDolar { get; set; }

        [Display(Name = "Valor do Aluguel em Dólar")]
        [DataType(DataType.Currency)]
        public Decimal ValorAluguelDolar { get; set; }


        public virtual Unidade Unidade { get; set; }
        public virtual ICollection<ItemPropostaEquipamento> ItemPropostaEquipamentos { get; set; }

    }
}