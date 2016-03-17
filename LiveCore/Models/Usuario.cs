using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LiveCore.Models
{
    [Table("USUARIO")]
    public class Usuario
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int UsuarioID { get; set; }

        [Display(Name = "Login")]
        public String Login { get; set; }

        [Display(Name = "Senha")]
        [Required(ErrorMessage="A senha é obrigtória")]
        public String Senha { get; set; }

        [Display(Name = "Confirmação da Senha")]
        [Compare("Senha", ErrorMessage = "A Senha e a Confirmação de Senha são diferentes")]
        [Required(ErrorMessage = "Confirme a senha")]
        public String ConfirmSenha { get; set; }

        public virtual ICollection<Permissao> Permissoes { get; set; }
    }
}