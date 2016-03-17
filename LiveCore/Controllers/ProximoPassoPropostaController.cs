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
    public class ProximoPassoPropostaController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /ProximoPassoProposta/
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Index()
        {
            return View(db.ProximoPassoProposta.ToList());
        }

        // GET: /ProximoPassoProposta/Details/5
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProximoPassoProposta proximopassoproposta = db.ProximoPassoProposta.Find(id);
            if (proximopassoproposta == null)
            {
                return HttpNotFound();
            }
            return View(proximopassoproposta);
        }

        public JsonResult CriarProximoPasso(String descricao, String dataAgendamento, String horaAgendamento, int propostaID, String status, int tempoAlerta, String tipoAlerta)
        {
            ProximoPassoProposta proximoPasso = new ProximoPassoProposta();

            proximoPasso.DataCriacao = DateTime.Now;

            proximoPasso.Descricao = descricao;
            proximoPasso.DataAgendamento = Convert.ToDateTime(dataAgendamento);
            proximoPasso.DataAgendamento = proximoPasso.DataAgendamento.Add(TimeSpan.Parse(horaAgendamento));
            proximoPasso.PropostaID = propostaID;
            proximoPasso.Status = status;
            proximoPasso.TempoAlerta = tempoAlerta;
            proximoPasso.TipoAlerta = tipoAlerta;
            
            var result = 0;

            try
            {
                db.ProximoPassoProposta.Add(proximoPasso);
                db.SaveChanges();
                result = (db.ProximoPassoProposta.Where(p => p.Descricao.ToUpper().Equals(descricao.ToUpper())).FirstOrDefault()).ProximoPassoPropostaID;
                ViewBag.ProximoPassoProposta = result;
            }
            catch (Exception ex)
            {
                //result = "Erro ao salvar Tipo de Contrato: " + ex.Message;   
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditarProximoPasso(int proximoPassoID, String descricao, String dataAgendamento, String horaAgendamento, int propostaID, String status, int tempoAlerta, String tipoAlerta)
        {
            String retorno = "";

            ProximoPassoProposta proximoPasso = new ProximoPassoProposta();

            proximoPasso.DataCriacao = DateTime.Now;

            proximoPasso.ProximoPassoPropostaID = proximoPassoID;
            proximoPasso.Descricao = descricao;
            proximoPasso.DataAgendamento = Convert.ToDateTime(dataAgendamento);
            proximoPasso.DataAgendamento = proximoPasso.DataAgendamento.Add(TimeSpan.Parse(horaAgendamento));
            proximoPasso.PropostaID = propostaID;
            proximoPasso.Status = status;

            if(tempoAlerta > 0){
                proximoPasso.TempoAlerta = tempoAlerta;
            }

            if (tipoAlerta != null && !tipoAlerta.Equals(""))
            {
                proximoPasso.TipoAlerta = tipoAlerta;
            }

            if (proximoPasso.Status.Equals("FI"))
            {
                HistoricoProposta historico = new HistoricoProposta();
                historico.DataCriacao = DateTime.Now;
                historico.Descricao = proximoPasso.Descricao;
                historico.CriadoPor = "LiveCore - Próximo Passo";
                historico.PropostaID = proximoPasso.PropostaID;

                HistoricoPropostaController historicoController = new HistoricoPropostaController();
                historicoController.Create(historico);

                try
                {
                    db.Entry(proximoPasso).State = EntityState.Deleted;
                    db.ProximoPassoProposta.Remove(proximoPasso);
                    db.SaveChanges();
                    retorno = "Próximo Passo finalizado com sucesso.";
                }
                catch (Exception ex)
                {
                    retorno = "Não foi possível finalizar o próximo passo: " + ex.Message;
                }
            }
            else
            {
                db.Entry(proximoPasso).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                    retorno = "Próximo Passo editado com sucesso.";
                }
                catch (Exception ex)
                {
                    retorno = "Não foi possível salvar o próximo passo: " + ex.Message;
                }
            }

            return Json(retorno, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Details", "Unidade", new { id = unidade.UnidadeSigla });
        }

        // GET: /ProximoPassoProposta/Create
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Create(int id)
        {
            ProximoPassoProposta proximoPasso = new ProximoPassoProposta();
            proximoPasso.PropostaID = id;
            return View(proximoPasso);
        }

        // POST: /ProximoPassoProposta/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProximoPassoPropostaID,Descricao,DataCriacao,DataAgendamento,Status,PropostaID,tempoAlerta,tipoAlerta")] ProximoPassoProposta proximopassoproposta)
        {
            proximopassoproposta.DataCriacao = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.ProximoPassoProposta.Add(proximopassoproposta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(proximopassoproposta);
        }

        // GET: /ProximoPassoProposta/Edit/5
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProximoPassoProposta proximopassoproposta = db.ProximoPassoProposta.Find(id);
            if (proximopassoproposta.DataAgendamento != null && !proximopassoproposta.DataAgendamento.ToString().Trim().Equals(""))
            {
                ViewBag.HoraAgendamento = proximopassoproposta.DataAgendamento.ToString("HH:mm");
            }
            if (proximopassoproposta == null)
            {
                return HttpNotFound();
            }
            return View(proximopassoproposta);
        }

        // POST: /ProximoPassoProposta/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProximoPassoPropostaID,Descricao,DataCriacao,DataAgendamento,Status,PropostaID,tempoAlerta,tipoAlerta")] ProximoPassoProposta proximopassoproposta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(proximopassoproposta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(proximopassoproposta);
        }

        // GET: /ProximoPassoProposta/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProximoPassoProposta proximopassoproposta = db.ProximoPassoProposta.Find(id);
            if (proximopassoproposta == null)
            {
                return HttpNotFound();
            }
            return View(proximopassoproposta);
        }

        // POST: /ProximoPassoProposta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProximoPassoProposta proximopassoproposta = db.ProximoPassoProposta.Find(id);
            db.ProximoPassoProposta.Remove(proximopassoproposta);
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
