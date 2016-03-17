using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LiveCore.Models
{
    [Table("EMAILCONFIG")]
    public class EmailConfig
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int EmailConfigID { get; set; }

        [Display(Name = "Host")]
        [StringLength(100, ErrorMessage = "Máximo de 100 caracteres")]
        public String Host { get; set; }

        [Display(Name = "Porta")]
        public int Porta { get; set; }

        [Display(Name = "Login")]
        [StringLength(50, ErrorMessage = "Máximo de 50 caracteres")]
        public String Login { get; set; }

        [Display(Name = "Senha")]
        [StringLength(50, ErrorMessage = "Máximo de 50 caracteres")]
        public String Senha { get; set; }

        [Display(Name = "E-mail From")]
        [StringLength(100, ErrorMessage = "Máximo de 100 caracteres")]
        [EmailAddress]
        public String EmailFrom { get; set; }

        [Display(Name = "E-mail Padrão To")]
        [StringLength(100, ErrorMessage = "Máximo de 100 caracteres")]
        [EmailAddress]
        public String EmailDefaultTo { get; set; }
    }
}