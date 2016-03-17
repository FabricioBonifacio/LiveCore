using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LiveCore.Models;
using LiveCore.DAL;
using LiveCore.Security;

namespace LiveCore.Controllers
{
    public class RelatorioPropostaController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /RelatorioProposta/
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Index()
        {
            if (db.RelatorioPropostas.ToList().Count() == 0)
            {
                return RedirectToAction("Create");
            }
            else
            {
                RelatorioProposta relatorioProposta = db.RelatorioPropostas.ToList().FirstOrDefault();
                return RedirectToAction("Edit", new { id = relatorioProposta.RelatorioPropostaID });
            }
        }

        // GET: /RelatorioProposta/Details/5
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RelatorioProposta relatorioproposta = db.RelatorioPropostas.Find(id);
            if (relatorioproposta == null)
            {
                return HttpNotFound();
            }
            return View(relatorioproposta);
        }

        // GET: /RelatorioProposta/Create
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /RelatorioProposta/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RelatorioPropostaID,Empresa,Endereco,Telefone,Introducao,AcordoConfiab,Objetivos,ConfigSolucao,EscopoProjeto,Metodologia,Investimentos,Cond_Valores,Cond_Impostos,Cond_Frete,Cond_Faturamento,Cond_Validade,Cond_PrazoEntrega,Cond_Despesas,TermoAceite,Cond_Garantia,Cond_CampoLivre")] RelatorioProposta relatorioproposta)
        {
            if (ModelState.IsValid)
            {
                db.RelatorioPropostas.Add(relatorioproposta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(relatorioproposta);
        }

        // GET: /RelatorioProposta/Edit/5
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RelatorioProposta relatorioproposta = db.RelatorioPropostas.Find(id);
            if (relatorioproposta == null)
            {
                return HttpNotFound();
            }
            return View(relatorioproposta);
        }

        // POST: /RelatorioProposta/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RelatorioPropostaID,Empresa,Endereco,Telefone,Introducao,AcordoConfiab,Objetivos,ConfigSolucao,EscopoProjeto,Metodologia,Investimentos,Cond_Valores,Cond_Impostos,Cond_Frete,Cond_Faturamento,Cond_Validade,Cond_PrazoEntrega,Cond_Despesas,TermoAceite,Cond_Garantia,Cond_CampoLivre")] RelatorioProposta relatorioproposta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(relatorioproposta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(relatorioproposta);
        }

        // GET: /RelatorioProposta/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RelatorioProposta relatorioproposta = db.RelatorioPropostas.Find(id);
            if (relatorioproposta == null)
            {
                return HttpNotFound();
            }
            return View(relatorioproposta);
        }

        // POST: /RelatorioProposta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RelatorioProposta relatorioproposta = db.RelatorioPropostas.Find(id);
            db.RelatorioPropostas.Remove(relatorioproposta);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
