using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiveCore.Models
{
    [Table("ENTIDADE")]
    public class Entidade
    {
        public Entidade()
        {
            UFs = new List<SelectListItem>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int EntidadeID { get; set; }

        [Display(Name = "Nome Fantasia")]
        [StringLength(50, ErrorMessage = "Máximo de 100 caracteres")]
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

        [Display(Name = "Nome")]
        [StringLength(50, ErrorMessage = "Máximo de 50 caracteres")]
        public String Nome { get; set; }

        [Display(Name = "CPF")]
        public String CPF { get; set; }

        [Display(Name = "Tipo de Entidade")]
        public String TipoEntidade { get; set; }

        [Display(Name = "Empresa Proprietária")]
        public Boolean SNProprietaria { get; set; }

        public virtual ICollection<Contato> Contato { get; set; }
        public virtual ICollection<Papel> Papeis { get; set; }
        public IEnumerable<SelectListItem> UFs { get; set; }
        	
    }
}