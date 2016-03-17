using LiveCore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace LiveCore.DAL
{
    public class LiveCoreContext : DbContext
    {

        public LiveCoreContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Unidade> Unidade { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<AreaEscopo> AreaEscopo { get; set; }
        public DbSet<TipoContrato> TipoContrato { get; set; }
        public DbSet<Servico> Servico { get; set; }
        public DbSet<Contato> Contato { get; set; }
        public DbSet<Entidade> Entidade { get; set; }
        public DbSet<Papel> Papel { get; set; }
        public DbSet<Equipamento> Equipamento { get; set; }
        public DbSet<ItemPropostaEquipamento> ItemPropostaEquipamento { get; set; }
        public DbSet<ItemPropostaServico> ItemPropostaServico { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Permissao> Permissao { get; set; }
        public DbSet<PapelContato> PapelContato { get; set; }
        public DbSet<ProximoPassoProposta> ProximoPassoProposta { get; set; }
        public DbSet<HistoricoProposta> HistoricoPropostas { get; set; }
        public DbSet<PropostaImpressa> PropostaImpressa { get; set; }
        public DbSet<Empresa> Empresa { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            
            modelBuilder.Entity<Proposta>()
                    .HasRequired(m => m.ContatoLive)
                    .WithMany(t => t.LivePropostas)
                    .HasForeignKey(m => m.RespLiveID)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Proposta>()
                    .HasRequired(m => m.Contato)
                    .WithMany(t => t.ClientePropostas)
                    .HasForeignKey(m => m.ContatoID)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Usuario>()
                    .HasMany(c => c.Permissoes)
                    .WithMany(s => s.Usuarios)
                    .Map (mc =>
                        {
                           mc.ToTable("USUARIOPERMISSAO");
                           mc.MapLeftKey("USUARIOID");
                           mc.MapRightKey("PERMISSAOID");
                       });

            modelBuilder.Entity<Entidade>()
                    .HasMany(c => c.Contato)
                    .WithMany(s => s.Entidades)
                    .Map(mc =>
                    {
                        mc.ToTable("ENTIDADECONTATO");
                        mc.MapLeftKey("ENTIDADEID");
                        mc.MapRightKey("CONTATOID");
                    });

            modelBuilder.Entity<Entidade>()
                    .HasMany(c => c.Papeis)
                    .WithMany(s => s.Entidades)
                    .Map(mc =>
                    {
                        mc.ToTable("ENTIDADEPAPEL");
                        mc.MapLeftKey("ENTIDADEID");
                        mc.MapRightKey("PAPELID");
                    });

            modelBuilder.Entity<Contato>()
                    .HasMany(c => c.PapelContatos)
                    .WithMany(s => s.Contatos)
                    .Map(mc =>
                    {
                        mc.ToTable("CONTATOPAPEL");
                        mc.MapLeftKey("CONTATOID");
                        mc.MapRightKey("PAPELCONTATOID");
                    });

            modelBuilder.Entity<Empresa>()
                    .HasMany(c => c.Colaboradores)
                    .WithMany(s => s.Empresas)
                    .Map(mc =>
                    {
                        mc.ToTable("EMPRESACONTATO");
                        mc.MapLeftKey("EMPRESAID");
                        mc.MapRightKey("CONTATOID");
                    });
        }

        public System.Data.Entity.DbSet<LiveCore.Models.Proposta> Propostas { get; set; }

        public System.Data.Entity.DbSet<LiveCore.Models.EmailConfig> EmailConfigs { get; set; }

        public System.Data.Entity.DbSet<LiveCore.Models.RelatorioProposta> RelatorioPropostas { get; set; }

    }
}