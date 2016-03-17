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
    public class ServicoController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /Servico/
        [PermissoesFiltro(Roles = "Serviço")]
        public ActionResult Index(string ordem, string currentFilter, string nomeSearch, int? page)
        {
            ViewBag.CurrentSort = ordem;
            ViewBag.NomeSortParm = String.IsNullOrEmpty(ordem) ? "Serviço_desc" : "";
            ViewBag.DescSortParm = ordem == "Descrição" ? "Descrição_desc" : "Descrição";
            ViewBag.UnidadeSortParm = ordem == "Unidade" ? "Unidade_desc" : "Unidade";
            ViewBag.ValorSortParm = ordem == "Valor" ? "Valor_desc" : "Valor";

            if (nomeSearch != null)
            {
                page = 1;
            }
            else
            {
                nomeSearch = currentFilter;
            }

            ViewBag.CurrentFilter = nomeSearch;

            var servico = from s in db.Servico
                          select s;

            if (!String.IsNullOrEmpty(nomeSearch))
            {
                servico = servico.Where(s => s.Nome.ToUpper().Contains(nomeSearch.ToUpper()));
            }

            switch (ordem)
            {
                case "Descrição_desc":
                    servico = servico.OrderByDescending(s => s.Descricao);
                    break;
                case "Serviço_desc":
                    servico = servico.OrderByDescending(s => s.Nome);
                    break;
                case "Descrição":
                    servico = servico.OrderBy(s => s.Descricao);
                    break;
                case "Unidade_desc":
                    servico = servico.OrderByDescending(s => s.Unidade.UnidadeSigla);
                    break;
                case "Unidade":
                    servico = servico.OrderBy(s => s.Unidade.UnidadeSigla);
                    break;
                case "Valor_desc":
                    servico = servico.OrderByDescending(s => s.Valor);
                    break;
                case "Valor":
                    servico = servico.OrderBy(s => s.Valor);
                    break;
                default:
                    servico = servico.OrderBy(s => s.Nome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(servico.ToPagedList(pageNumber, pageSize));
        }

        public JsonResult AutoCompleteServico(String term)
        {
            var result = (from r in db.Servico
                          where r.Nome.ToLower().Contains(term.ToLower())
                          select new { r.Nome }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperaServicoPorNome(String servico)
        {
            Servico ae = db.Servico.Where(p => p.Nome.ToUpper().Equals(servico.ToUpper())).FirstOrDefault();

            var result = 0;

            if (ae != null)
            {
                result = ae.ServicoID;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperaServicoPorId(int servicoID)
        {
            Servico tc = db.Servico.Find(servicoID);

            var result = tc.Nome;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CriarServico(String servicoNovo)
        {
            Servico servico = new Servico();

            servico.Nome = servicoNovo;

            var result = 0;

            try
            {
                db.Servico.Add(servico);
                db.SaveChanges();
                result = (db.Servico.Where(p => p.Nome.ToUpper().Equals(servicoNovo.ToUpper())).FirstOrDefault()).ServicoID;
            }
            catch (Exception ex)
            {
                //result = "Erro ao salvar Tipo de Contrato: " + ex.Message;   
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditServicoModal(int servicoID, String nome, String descricao, String unidade, String valor, String valorDolar)
        {
            String retorno = "";
            Servico servico = new Servico();

            servico.ServicoID = servicoID;
            servico.Nome = nome;
            servico.Descricao = descricao;
            servico.UnidadeSigla = unidade;
            servico.Valor = Convert.ToDecimal(valor.Replace(".",""));
            servico.ValorDolar = Convert.ToDecimal(valorDolar.Replace(".", ""));

            db.Entry(servico).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                retorno = "Serviço editado com sucesso.";
            }
            catch (DbUpdateException ex)
            {
                retorno = ex.Message;
            }
            catch (Exception ex)
            {
                retorno = "Não foi possível salvar o serviço " + servico.Nome + ": " + ex.Message;
            }

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        // GET: /Servico/Details/5
        [PermissoesFiltro(Roles = "Serviço")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Servico servico = db.Servico.Find(id);
            if (servico == null)
            {
                return HttpNotFound();
            }
            if (TempData["Erro"] != null && !TempData["Erro"].ToString().Equals(""))
            {
                ViewBag.Erro = TempData["Erro"];
            }
            return View(servico);
        }

        // GET: /Servico/Create
        [PermissoesFiltro(Roles = "Serviço")]
        public ActionResult Create()
        {
            ViewBag.UnidadeSigla = new SelectList(db.Unidade, "UnidadeSigla", "Nome");
            return View();
        }

        // POST: /Servico/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ServicoID,Nome,Descricao,UnidadeSigla,Valor,ValorDolar")] Servico servico)
        {
            if (ModelState.IsValid)
            {
                db.Servico.Add(servico);
                Servico serv = db.Servico.Where(p => p.Nome.ToUpper().Equals(servico.Nome.Trim().ToUpper())).FirstOrDefault();
                if(serv != null)
                {
                    ViewBag.UnidadeSigla = new SelectList(db.Unidade, "UnidadeSigla", "Nome", servico.UnidadeSigla);
                    ViewBag.Erro = "O serviço " + servico.Nome + " já existe.";
                    return View(servico);
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.UnidadeSigla = new SelectList(db.Unidade, "UnidadeSigla", "Nome", servico.UnidadeSigla);
                    ViewBag.Erro = "Não foi possível salvar o Serviço " + servico.Nome + ": " + ex.Message;
                    return View(servico);
                }
                return RedirectToAction("Index");
            }

            ViewBag.UnidadeSigla = new SelectList(db.Unidade, "UnidadeSigla", "Nome", servico.UnidadeSigla);
            return View(servico);
        }

        // GET: /Servico/Edit/5
        [PermissoesFiltro(Roles = "Serviço")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Servico servico = db.Servico.Find(id);
            if (servico == null)
            {
                return HttpNotFound();
            }
            ViewBag.UnidadeSigla = new SelectList(db.Unidade, "UnidadeSigla", "Nome", servico.UnidadeSigla);
            return View(servico);
        }

        // GET: /Servico/Edit/5
        [PermissoesFiltro(Roles = "Serviço")]
        public ActionResult EditPopUp(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Servico servico = db.Servico.Find(id);
            if (servico == null)
            {
                return HttpNotFound();
            }
            ViewBag.UnidadeSigla = new SelectList(db.Unidade, "UnidadeSigla", "Nome", servico.UnidadeSigla);
            return View(servico);
        }

        // POST: /Servico/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ServicoID,Nome,Descricao,UnidadeSigla,Valor,ValorDolar")] Servico servico)
        {
            if (ModelState.IsValid)
            {
                db.Entry(servico).State = EntityState.Modified;
                Servico serv = db.Servico.Where(p => p.Nome.ToUpper().Equals(servico.Nome.Trim().ToUpper())).FirstOrDefault();
                if (serv != null && serv.ServicoID != servico.ServicoID)
                {
                    ViewBag.UnidadeSigla = new SelectList(db.Unidade, "UnidadeSigla", "Nome", servico.UnidadeSigla);
                    ViewBag.Erro = "O serviço " + servico.Nome + " já existe.";
                    return View(servico);
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Não foi possível salvar o Serviço " + servico.Nome + ": " + ex.Message;
                    return View(servico);
                }
                return RedirectToAction("Index");
            }
            ViewBag.UnidadeSigla = new SelectList(db.Unidade, "UnidadeSigla", "Nome", servico.UnidadeSigla);
            return View(servico);
        }

        // GET: /Servico/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Servico servico = db.Servico.Find(id);
            if (servico == null)
            {
                return HttpNotFound();
            }
            return View(servico);
        }

        // POST: /Servico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Servico servico = db.Servico.Find(id);
            db.Servico.Remove(servico);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                TempData["Erro"] = "Não foi possível deletar o Serviço " + servico.Nome + ". Alguns registros fazem referência a esse Serviço.";
                return RedirectToAction("Details", "Servico", new { id = servico.ServicoID });
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Não foi possível deletar o Serviço " + servico.Nome + ": " + ex.Message;
                return RedirectToAction("Details", "Servico", new { id = servico.ServicoID });
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
