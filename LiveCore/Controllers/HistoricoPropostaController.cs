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
    public class HistoricoPropostaController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /HistoricoProposta/
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Index(int? id)
        {
            if(id != null && id > 0){
                return View(db.HistoricoPropostas.Where(p => p.PropostaID == id).ToList());
            }
            return View(db.HistoricoPropostas.ToList());
        }

        // GET: /HistoricoProposta/Details/5
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HistoricoProposta historicoproposta = db.HistoricoPropostas.Find(id);
            if (historicoproposta == null)
            {
                return HttpNotFound();
            }
            return View(historicoproposta);
        }

        // GET: /HistoricoProposta/Create
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Create(int? id)
        {
            HistoricoProposta historicoProposta = new HistoricoProposta();
            
            if (id != null && id > 0)
            {
                historicoProposta.PropostaID = Convert.ToInt32(id);
            }
            historicoProposta.DataCriacao = DateTime.Now;
            String criador = "";
            if(!User.Identity.Name.Equals(""))
            {
                Contato contato = db.Contato.Where(p => p.Login == User.Identity.Name).FirstOrDefault();
                criador = contato.Nome;
            }
            historicoProposta.CriadoPor = criador;
            return View(historicoProposta);
        }

        // POST: /HistoricoProposta/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool Create([Bind(Include = "HistoricoPropostaID,DataCriacao,Descricao,PropostaID,CriadoPor")] HistoricoProposta historicoproposta)
        {
            if (ModelState.IsValid)
            {
                db.HistoricoPropostas.Add(historicoproposta);
                db.SaveChanges();
                //return RedirectToAction("Details", "Proposta", new { id = historicoproposta.PropostaID });
                return true;
            }

            //return View(historicoproposta);
            return false;
        }

        public ActionResult PopUpCriarHistorico(String descricao, int propostaID, String criadoPor)
        {
            var result = false;

            if (ModelState.IsValid)
            {
                HistoricoProposta hp = new HistoricoProposta();
                hp.DataCriacao = DateTime.Now;
                hp.Descricao = descricao;
                hp.PropostaID = propostaID;
                hp.CriadoPor = criadoPor;
                if (this.Create(hp))
                {
                    result = true;
                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeletarHistorico(int id)
        {
            var result = false;

            if (ModelState.IsValid)
            {
                if (this.DeleteConfirmed(id))
                {
                    result = true;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PopUpEditarHistorico(int id, String descricao)
        {
            var result = false;

            if (ModelState.IsValid)
            {
                HistoricoProposta hp = db.HistoricoPropostas.Find(id);

                if (hp == null)
                {
                    return HttpNotFound();
                }

                hp.Descricao = descricao;

                if (this.Edit(hp))
                {
                    result = true;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet); ;
        }

        public ActionResult BuscaHistoricoProposta(int id)
        {
            var historico = db.HistoricoPropostas.Where(p => p.PropostaID == id).ToList().OrderByDescending(p => p.DataCriacao);
            var result = "";

            foreach (var item in historico)
            {
                result += "<P><div class=\"btn-group\">";
                result += "<button type=\"button\" id=\"editarHistorico"+ item.HistoricoPropostaID +"\" class=\"btn btn-default button-historico\" location.href=\"#myModal\" data-toggle=\"modal\" data-target=\"#myModal\"><p><b>" + item.DataCriacao + "</b></p><p><b>Criado por: </b>" + item.CriadoPor + "</p><p>" + item.Descricao + "</p></button>";
                result += "<button type=\"button\" id=\"excluirHistorico"+ item.HistoricoPropostaID +"\" class=\"btn btn-default close-button\"><span class=\"glyphicon glyphicon-remove\"></span></button>";
                result += "</div></p>";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: /HistoricoProposta/Edit/5
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HistoricoProposta historicoproposta = db.HistoricoPropostas.Find(id);
            if (historicoproposta == null)
            {
                return HttpNotFound();
            }
            return View(historicoproposta);
        }

        // POST: /HistoricoProposta/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool Edit([Bind(Include = "HistoricoPropostaID,DataCriacao,Descricao,PropostaID,CriadoPor")] HistoricoProposta historicoproposta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(historicoproposta).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            return false;
        }

        // GET: /HistoricoProposta/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HistoricoProposta historicoproposta = db.HistoricoPropostas.Find(id);
            if (historicoproposta == null)
            {
                return HttpNotFound();
            }
            return View(historicoproposta);
        }

        // POST: /HistoricoProposta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public bool DeleteConfirmed(int id)
        {
            HistoricoProposta historicoproposta = db.HistoricoPropostas.Find(id);
            try 
	        {	        
		        db.HistoricoPropostas.Remove(historicoproposta);
                db.SaveChanges();
                return true;
	        }
	        catch (Exception)
	        {
                return false;
	        }
           
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
