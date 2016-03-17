using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiveCore.Models
{
    [Table("PROPOSTAIMPRESSA")]
    public class PropostaImpressa
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int PropostaImpressaID { get; set; }

        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        [Display(Name = "Introdução")]
        public String Introducao { get; set; }

        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        [Display(Name = "Acordo de Confiablidade")]
        public String AcordoConfiab { get; set; }

        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        [Display(Name = "Objetivos")]
        public String Objetivos { get; set; }

        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        [Display(Name = "Configuração da Solução")]
        public String ConfigSolucao { get; set; }

        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        [Display(Name = "Escopo do Projeto")]
        public String EscopoProjeto { get; set; }

        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        [Display(Name = "Metodologia")]
        public String Metodologia { get; set; }

        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        [Display(Name = "Investimentos")]
        public String Investimentos { get; set; }

        [Display(Name = "Valores")]
        public String Cond_Valores { get; set; }

        [Display(Name = "Impostos")]
        public String Cond_Impostos { get; set; }

        [Display(Name = "Frete")]
        public String Cond_Frete { get; set; }

        [Display(Name = "Faturamento")]
        public String Cond_Faturamento { get; set; }

        [Display(Name = "Validade")]
        public String Cond_Validade { get; set; }

        [Display(Name = "Prazo de Entrega")]
        public String Cond_PrazoEntrega { get; set; }

        [Display(Name = "Despesas")]
        public String Cond_Despesas { get; set; }

        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        [Display(Name = "Termo de Aceite")]
        public String TermoAceite { get; set; }

        [Display(Name = "Garantia")]
        public String Cond_Garantia { get; set; }

        [AllowHtml]
        [Display(Name = "Campo Livre")]
        public String Cond_CampoLivre { get; set; }

        [Display(Name = "Proposta")]
        public int PropostaID { get; set; }

        public Boolean IntroducaoSN { get; set; }
        public Boolean AcordoConfiabSN { get; set; }
        public Boolean ObjetivosSN { get; set; }
        public Boolean ConfigSolucaoSN { get; set; }
        public Boolean EscopoProjetoSN { get; set; }
        public Boolean MetodologiaSN { get; set; }
        public Boolean InvestimentosSN { get; set; }
        public Boolean Cond_ValoresSN { get; set; }
        public Boolean Cond_ImpostosSN { get; set; }
        public Boolean Cond_FreteSN { get; set; }
        public Boolean Cond_FaturamentoSN { get; set; }
        public Boolean Cond_ValidadeSN { get; set; }
        public Boolean Cond_PrazoEntregaSN { get; set; }
        public Boolean Cond_DespesasSN { get; set; }
        public Boolean TermoAceiteSN { get; set; }
        public Boolean Cond_GarantiaSN { get; set; }
        public Boolean Cond_CampoLivreSN { get; set; }

        [Display(Name = "Proposta")]
        public virtual Proposta Proposta { get; set; }
    }
}