using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LiveCore.Models
{
    [Table("TIPOCONTRATO")]
    public class TipoContrato
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int TipoContratoID { get; set; }

        [Display(Name = "Nome")]
        [StringLength(50, ErrorMessage = "Máximo de 50 caracteres")]
        public String Nome { get; set; }

        [Display(Name = "Descrição")]
        public String Descricao { get; set; }

        [Display(Name = "Exibir em Item de Serviço")]
        public Boolean SNServico { get; set; }

        [Display(Name = "Exibir em Item de Equipamento")]
        public Boolean SNEquipamento { get; set; }


        //public virtual ICollection<Equipamento> Equipamentos { get; set; }
        //public virtual ICollection<Servico> Servicos { get; set; }
    }
}