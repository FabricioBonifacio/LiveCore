using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LiveCore.Models
{
    [Table("PROPOSTA")]
    public class Proposta
    {
        
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int PropostaID { get; set; }

        [Display(Name = "Entidade")]
        public int EntidadeID { get; set; }

        [Display(Name = "Valor Total")]
        [DataType(DataType.Currency)]
        public Decimal ValorTotal { get; set; }

        [Display(Name = "Validade")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Validade { get; set; }

        [Display(Name = "Gerente de Conta")]
        public int RespLiveID { get; set; }

        [Display(Name = "Responsável Cliente")]
        public int ContatoID { get; set; }

        [Display(Name = "Status")]
        public String StatusSigla { get; set; }

        [Display(Name = "Anexo")]
        public Byte[] Anexo { get; set; }

        [Display(Name = "Content Type")]
        public String ContentType { get; set; }

        [Display(Name = "Nome do Arquivo")]
        public String NomeArquivo { get; set; }

        [Display(Name = "Identificador da Proposta")]
        public String IdentProposta { get; set; }

        [Display(Name = "Versão")]
        public Decimal Versao { get; set; }

        [Display(Name = "Data de Criação")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataCriacao { get; set; }

        [Display(Name = "Câmbio")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C4}")]
        public Decimal Cambio { get; set; }

        [Display(Name = "Valor Total Dólar")]
        [DataType(DataType.Currency)]
        public Decimal ValorTotalDolar { get; set; }

        [Display(Name = "Referência")]
        public String Referencia { get; set; }
               


        [Display(Name = "Cliente")]
        public virtual Entidade Entidade { get; set; }
        [Display(Name = "Comercial")]
        public virtual Contato ContatoLive { get; set; }
        [Display(Name = "Contato")]
        public virtual Contato Contato { get; set; }
        [Display(Name = "Estágio da Proposta")]
        public virtual Status Status { get; set; }


        public virtual ICollection<ItemPropostaEquipamento> ItemPropostaEquipamentos { get; set; }
        public virtual ICollection<ItemPropostaServico> ItemPropostaServicos { get; set; }
        public virtual ICollection<HistoricoProposta> HistoricoPropostas { get; set; }
    }
}