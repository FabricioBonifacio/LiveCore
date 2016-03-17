using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LiveCore.Models
{
    [Table("PERMISSAO")]
    public class Permissao
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int PermissaoID { get; set; }

        [Display(Name = "Nome")]
        public String Nome { get; set; }


        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}