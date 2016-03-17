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
using PagedList;
using System.Data.Entity.Infrastructure;
using LiveCore.Security;

namespace LiveCore.Controllers
{
    public class TipoContratoController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /TipoContrato/
        [PermissoesFiltro(Roles = "Tipo de Contrato")]
        public ActionResult Index(string ordem, string currentFilter, string nomeSearch, int? page)
        {
            ViewBag.CurrentSort = ordem;
            ViewBag.NomeSortParm = String.IsNullOrEmpty(ordem) ? "Tipo de Contrato_desc" : "";
            ViewBag.DescSortParm = ordem == "Descrição" ? "Descrição_desc" : "Descrição";

            if (nomeSearch != null)
            {
                page = 1;
            }
            else
            {
                nomeSearch = currentFilter;
            }

            ViewBag.CurrentFilter = nomeSearch;

            var tipoContrato = from s in db.TipoContrato
                           select s;

            if (!String.IsNullOrEmpty(nomeSearch))
            {
                tipoContrato = tipoContrato.Where(s => s.Nome.ToUpper().Contains(nomeSearch.ToUpper()));
            }

            switch (ordem)
            {
                case "Descrição_desc":
                    tipoContrato = tipoContrato.OrderByDescending(s => s.Descricao);
                    break;
                case "Descrição":
                    tipoContrato = tipoContrato.OrderBy(s => s.Descricao);
                    break;
                case "Tipo de Contrato_desc":
                    tipoContrato = tipoContrato.OrderByDescending(s => s.Nome);
                    break;
                default:
                    tipoContrato = tipoContrato.OrderBy(s => s.Nome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (TempData["Erro"] != null && !TempData["Erro"].ToString().Equals(""))
            {
                ViewBag.Erro = TempData["Erro"];
            }

            if (TempData["Msg"] != null && !TempData["Msg"].ToString().Equals(""))
            {
                ViewBag.Msg = TempData["Msg"];
            }

            return View(tipoContrato.ToPagedList(pageNumber, pageSize));
        }

        // GET: /TipoContrato/Details/5
        [PermissoesFiltro(Roles = "Tipo de Contrato")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoContrato tipocontrato = db.TipoContrato.Find(id);
            if (tipocontrato == null)
            {
                return HttpNotFound();
            }
            if (TempData["Erro"] != null && !TempData["Erro"].ToString().Equals(""))
            {
                ViewBag.Erro = TempData["Erro"];
            }

            if (TempData["Msg"] != null && !TempData["Msg"].ToString().Equals(""))
            {
                ViewBag.Msg = TempData["Msg"];
            }
            return View(tipocontrato);
        }

        // GET: /TipoContrato/Create
        [PermissoesFiltro(Roles = "Tipo de Contrato")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /TipoContrato/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="TipoContratoID,Nome,Descricao,SNServico,SNEquipamento")] TipoContrato tipocontrato)
        {
            if (ModelState.IsValid)
            {
                db.TipoContrato.Add(tipocontrato);
                try
                {
                    db.SaveChanges();
                    TempData["Msg"] = "Tipo de Contrato cadastrado com sucesso.";
                }
                catch (Exception ex)
                {
                    TempData["Erro"] = "Não foi possível salvar o Tipo de Contrato " + tipocontrato.Nome + ": " + ex.Message;
                }
            }
            return RedirectToAction("Index", "TipoContrato", new { id = tipocontrato.TipoContratoID });
        }

        public JsonResult AutoCompleteTipoContrato(String term)
        {
            var result = (from r in db.TipoContrato
                              where r.Nome.ToLower().Contains(term.ToLower())
                              select new { r.Nome }).Distinct();
            
            if (Request.UrlReferrer.ToString().Contains("ItemPropostaServico"))
            {
                result = (from r in db.TipoContrato
                              where r.Nome.ToLower().Contains(term.ToLower())
                              where r.SNServico
                              select new { r.Nome }).Distinct();
            }
            if (Request.UrlReferrer.ToString().Contains("ItemPropostaEquipamento"))
            {
                result = (from r in db.TipoContrato
                              where r.Nome.ToLower().Contains(term.ToLower())
                              where r.SNEquipamento
                              select new { r.Nome }).Distinct();
            }
            

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperaTipoContratoPorNome(String tipoContrato)
        {
            TipoContrato tc = db.TipoContrato.Where(p => p.Nome.ToUpper().Equals(tipoContrato.ToUpper())).FirstOrDefault();

            var result = 0;

            if (tc != null)
            {
                result = tc.TipoContratoID;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperaTipoContratoPorId(int tipoContratoID)
        {
            TipoContrato tc = db.TipoContrato.Find(tipoContratoID);

            var result = tc.Nome;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CriarTipoContrato(String tipoContratoNovo)
        {
            TipoContrato tipoContrato = new TipoContrato();

            tipoContrato.Nome = tipoContratoNovo;
            tipoContrato.SNServico = true;
            tipoContrato.SNEquipamento = true;

            var result = 0;

            try
            {
                db.TipoContrato.Add(tipoContrato);
                db.SaveChanges();
                result = (db.TipoContrato.Where(p => p.Nome.ToUpper().Equals(tipoContratoNovo.ToUpper())).FirstOrDefault()).TipoContratoID;
            }
            catch (Exception ex)
            {
                //result = "Erro ao salvar Tipo de Contrato: " + ex.Message;   
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditTipoContratoModal(int tipoContratoID, String nome, String descricao, bool SNServico, bool SNEquipamento)
        {
            String retorno = "";
            TipoContrato tipoContrato = new TipoContrato();

            tipoContrato.TipoContratoID = tipoContratoID;
            tipoContrato.Nome = nome;
            tipoContrato.Descricao = descricao;
            tipoContrato.SNEquipamento = SNEquipamento;
            tipoContrato.SNServico = SNServico;

            db.Entry(tipoContrato).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                retorno = "Tipo Contrato editado com sucesso.";
            }
            catch (DbUpdateException ex)
            {
                retorno = ex.Message;
            }
            catch (Exception ex)
            {
                retorno = "Não foi possível salvar o tipo de contrato " + tipoContrato.Nome + ": " + ex.Message;
            }

            return Json(retorno, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Details", "Unidade", new { id = unidade.UnidadeSigla });
        }

        // GET: /TipoContrato/Edit/5
        [PermissoesFiltro(Roles = "Tipo de Contrato")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoContrato tipocontrato = db.TipoContrato.Find(id);
            if (tipocontrato == null)
            {
                return HttpNotFound();
            }
            return View(tipocontrato);
        }

        // POST: /TipoContrato/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TipoContratoID,Nome,Descricao,SNServico,SNEquipamento")] TipoContrato tipocontrato)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipocontrato).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    TempData["Msg"] = "Tipo de Contrato editado com sucesso.";
                }
                catch (Exception ex)
                {
                    TempData["Erro"] = "Não foi possível salvar o Tipo de Contrato " + tipocontrato.Nome + ": " + ex.Message;
                }
            }
            return RedirectToAction("Details", "TipoContrato", new { id = tipocontrato.TipoContratoID });
        }

        // GET: /TipoContrato/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoContrato tipocontrato = db.TipoContrato.Find(id);
            if (tipocontrato == null)
            {
                return HttpNotFound();
            }
            return View(tipocontrato);
        }

        // POST: /TipoContrato/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoContrato tipocontrato = db.TipoContrato.Find(id);
            db.TipoContrato.Remove(tipocontrato);
            try
            {
                db.SaveChanges();
                TempData["Msg"] = "Tipo de Contrato deletado com sucesso.";
            }
            catch (DbUpdateException ex)
            {
                TempData["Erro"] = "Não foi possível deletar o Tipo de Contrato " + tipocontrato.Nome + ". Alguns registros fazem referência a esse Tipo de Contrato.";
                return RedirectToAction("Details", "TipoContrato", new { id = tipocontrato.TipoContratoID });
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Não foi possível deletar o Tipo de Contrato " + tipocontrato.Nome + ": " + ex.Message;
                return RedirectToAction("Details", "TipoContrato", new { id = tipocontrato.TipoContratoID });
            }
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
