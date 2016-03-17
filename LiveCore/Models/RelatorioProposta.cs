using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiveCore.Models
{
    [Table("RELATORIOPROPOSTA")]
    public class RelatorioProposta
    {
        [Key]
        [Display(Name = "ID")]
        public int RelatorioPropostaID { get; set; }

        [Display(Name = "Empresa")]
        public String Empresa { get; set; }

        [Display(Name = "Endereço")]
        public String Endereco { get; set; }

        [Display(Name = "Telefone")]
        public String Telefone { get; set; }

        [AllowHtml]
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

    }
}