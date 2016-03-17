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
using System.Data.Entity.Core;

namespace LiveCore.Controllers
{
    public class PropostaImpressaController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /PropostaImpressa/
        public ActionResult Index(int id)
        {
            PropostaImpressa propostaImpressa = db.PropostaImpressa.Where(p => p.PropostaID == id).FirstOrDefault();

            if (propostaImpressa == null)
            {
                return RedirectToAction("Create", new { id = id });
            }
            else
            {
                return RedirectToAction("Details", new { id = propostaImpressa.PropostaImpressaID });
            }
        }

        // GET: /PropostaImpressa/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PropostaImpressa propostaimpressa = db.PropostaImpressa.Find(id);
            if (propostaimpressa == null)
            {
                return HttpNotFound();
            }
            Proposta proposta = db.Propostas.Find(propostaimpressa.PropostaID);
            ViewBag.IdentProposta = proposta.IdentProposta;
            return View(propostaimpressa);
        }

        // GET: /PropostaImpressa/Create
        public ActionResult Create(int? id)
        {
            PropostaImpressa propostaImpressa = new PropostaImpressa();
            if (id != null && id > 0)
            {
                propostaImpressa.PropostaID = Convert.ToInt32(id);
            }
            copiarPropostaPadrao(propostaImpressa);
            Proposta proposta = db.Propostas.Find(propostaImpressa.PropostaID);
            ViewBag.IdentProposta = proposta.IdentProposta;
            return View(propostaImpressa);
        }

        private void copiarPropostaPadrao(PropostaImpressa propostaImpressa)
        {
            RelatorioProposta relatorioProposta = db.RelatorioPropostas.FirstOrDefault();
            if(relatorioProposta != null)
            {
                propostaImpressa.AcordoConfiab = relatorioProposta.AcordoConfiab;
                propostaImpressa.Cond_Despesas = relatorioProposta.Cond_Despesas;
                propostaImpressa.Cond_Faturamento = relatorioProposta.Cond_Faturamento;
                propostaImpressa.Cond_Frete = relatorioProposta.Cond_Frete;
                propostaImpressa.Cond_Impostos = relatorioProposta.Cond_Impostos;
                propostaImpressa.Cond_PrazoEntrega = relatorioProposta.Cond_PrazoEntrega;
                propostaImpressa.Cond_Validade = relatorioProposta.Cond_Validade;
                propostaImpressa.Cond_Valores = relatorioProposta.Cond_Valores;
                propostaImpressa.ConfigSolucao = relatorioProposta.ConfigSolucao;
                propostaImpressa.EscopoProjeto = relatorioProposta.EscopoProjeto;
                propostaImpressa.Introducao = relatorioProposta.Introducao;
                propostaImpressa.Investimentos = relatorioProposta.Investimentos;
                propostaImpressa.Metodologia = relatorioProposta.Metodologia;
                propostaImpressa.Objetivos = relatorioProposta.Objetivos;
                propostaImpressa.TermoAceite = relatorioProposta.TermoAceite;
                propostaImpressa.Cond_Garantia = relatorioProposta.Cond_Garantia;
                propostaImpressa.Cond_CampoLivre = relatorioProposta.Cond_CampoLivre;

            }
        }

        // POST: /PropostaImpressa/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PropostaImpressaID,Introducao,AcordoConfiab,Objetivos,ConfigSolucao,EscopoProjeto,Metodologia,Investimentos,Cond_Valores,Cond_Impostos,Cond_Frete,Cond_Faturamento,Cond_Validade,Cond_PrazoEntrega,Cond_Despesas,TermoAceite,Cond_Garantia,Cond_CampoLivre,PropostaID," +
            "IntroducaoSN,AcordoConfiabSN,ObjetivosSN,ConfigSolucaoSN,EscopoProjetoSN,MetodologiaSN,InvestimentosSN,Cond_ValoresSN,Cond_ImpostosSN,Cond_FreteSN,Cond_FaturamentoSN,Cond_ValidadeSN,Cond_PrazoEntregaSN,Cond_DespesasSN,TermoAceiteSN,Cond_GarantiaSN,Cond_CampoLivreSN")] PropostaImpressa propostaimpressa)
        {
            if (ModelState.IsValid)
            {
                db.PropostaImpressa.Add(propostaimpressa);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = propostaimpressa.PropostaID });
            }

            return View(propostaimpressa);
        }

        // GET: /PropostaImpressa/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PropostaImpressa propostaimpressa = db.PropostaImpressa.Find(id);
            if (propostaimpressa == null)
            {
                return HttpNotFound();
            }
            Proposta proposta = db.Propostas.Find(propostaimpressa.PropostaID);
            ViewBag.IdentProposta = proposta.IdentProposta;
            return View(propostaimpressa);
        }

        // POST: /PropostaImpressa/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PropostaImpressaID,Introducao,AcordoConfiab,Objetivos,ConfigSolucao,EscopoProjeto,Metodologia,Investimentos,Cond_Valores,Cond_Impostos,Cond_Frete,Cond_Faturamento,Cond_Validade,Cond_PrazoEntrega,Cond_Despesas,TermoAceite,Cond_Garantia,Cond_CampoLivre,PropostaID," +
            "IntroducaoSN,AcordoConfiabSN,ObjetivosSN,ConfigSolucaoSN,EscopoProjetoSN,MetodologiaSN,InvestimentosSN,Cond_ValoresSN,Cond_ImpostosSN,Cond_FreteSN,Cond_FaturamentoSN,Cond_ValidadeSN,Cond_PrazoEntregaSN,Cond_DespesasSN,TermoAceiteSN,Cond_GarantiaSN,Cond_CampoLivreSN")] PropostaImpressa propostaimpressa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(propostaimpressa).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = propostaimpressa.PropostaID });
                }
                catch (OptimisticConcurrencyException)
                {
                    
                }
            }
            return View(propostaimpressa);
        }

        // GET: /PropostaImpressa/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PropostaImpressa propostaimpressa = db.PropostaImpressa.Find(id);
            if (propostaimpressa == null)
            {
                return HttpNotFound();
            }
            return View(propostaimpressa);
        }

        // POST: /PropostaImpressa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PropostaImpressa propostaimpressa = db.PropostaImpressa.Find(id);
            db.PropostaImpressa.Remove(propostaimpressa);
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
