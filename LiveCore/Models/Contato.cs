using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LiveCore.Models
{
    [Table("CONTATO")]
    public class Contato
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ContatoID { get; set; }

        [Display(Name = "Nome")]
        [StringLength(50, ErrorMessage = "Máximo de 50 caracteres")]
        public String Nome { get; set; }

        [Display(Name = "Login")]
        public String Login { get; set; }

        [Display(Name = "Telefone")]
        public String Telefone { get; set; }

        [Display(Name = "E-mail")]
        [EmailAddress]
        public String Email { get; set; }

        [Display(Name = "Data de Registro")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DtRegistro { get; set; }


        public virtual ICollection<Empresa> Empresas { get; set; }
        public virtual ICollection<Entidade> Entidades { get; set; }
        public virtual ICollection<Proposta> LivePropostas { get; set; }
        public virtual ICollection<Proposta> ClientePropostas { get; set; }
        public virtual ICollection<PapelContato> PapelContatos { get; set; }

    }
}