using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiveCore.Models
{
    [Table("EMPRESA")]
    public class Empresa
    {
        public Empresa()
        {
            UFs = new List<SelectListItem>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int EmpresaID { get; set; }

        [Display(Name = "Razão Social")]
        [StringLength(100, ErrorMessage = "Máximo de 100 caracteres")]
        public String RazaoSocial { get; set; }

        [Display(Name = "Nome Fantasia")]
        [StringLength(100, ErrorMessage = "Máximo de 100 caracteres")]
        public String NomeFantasia { get; set; }

        [Display(Name = "Endereço")]
        public String Endereco { get; set; }

        [Display(Name = "Bairro")]
        public String Bairro { get; set; }

        [Display(Name = "CEP")]
        public String CEP { get; set; }

        [Display(Name = "Cidade")]
        public String Cidade { get; set; }

        [Display(Name = "UF")]
        public String UF { get; set; }

        [Display(Name = "Telefone")]
        public String Telefone { get; set; }

        [Display(Name = "E-mail")]
        public String Email { get; set; }

        [Display(Name = "CNPJ")]
        public String CNPJ { get; set; }

        [Display(Name = "Inscrição Estadual")]
        public String InscEstadual { get; set; }

        [Display(Name = "E-mail do Financeiro")]
        public String EmailFinanceiro { get; set; }

        [Display(Name = "E-mail do Comercial")]
        public String EmailComercial { get; set; }

        public virtual ICollection<Contato> Colaboradores { get; set; }
        public IEnumerable<SelectListItem> UFs { get; set; }
        	
    }
}