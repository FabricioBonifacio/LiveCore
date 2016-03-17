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
    public class StatusController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /Status/
        [PermissoesFiltro(Roles = "Estágio da Proposta")]
        public ActionResult Index(string ordem, string currentFilter, string nomeSearch, int? page)
        {
            ViewBag.CurrentSort = ordem;
            ViewBag.NomeSortParm = ordem == "Estágio da Proposta" ? "Estágio da Proposta_desc" : "Estágio da Proposta";

            if (nomeSearch != null)
            {
                page = 1;
            }
            else
            {
                nomeSearch = currentFilter;
            }

            ViewBag.CurrentFilter = nomeSearch;

            var status = from s in db.Status
                               select s;

            if (!String.IsNullOrEmpty(nomeSearch))
            {
                status = status.Where(s => s.Nome.ToUpper().Contains(nomeSearch.ToUpper()));
            }

            switch (ordem)
            {
                case "Estágio da Proposta_desc":
                    status = status.OrderByDescending(s => s.Nome);
                    break;
                default:
                    status = status.OrderBy(s => s.Nome);
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

            return View(status.ToPagedList(pageNumber, pageSize));
        }

        // GET: /Status/Details/5
        [PermissoesFiltro(Roles = "Estágio da Proposta")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Status status = db.Status.Find(id);
            if (status == null)
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
            return View(status);
        }

        // GET: /Status/Create
        [PermissoesFiltro(Roles = "Estágio da Proposta")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Status/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="StatusSigla,Nome")] Status status)
        {
            if (ModelState.IsValid)
            {
                db.Status.Add(status);
                try
                {
                    db.SaveChanges();
                    TempData["Msg"] = "Estágio da Proposta cadastrado com sucesso.";
                }
                catch (DbUpdateException ex)
                {
                    TempData["Erro"] = "Já existe um estágio com a sigla " + status.StatusSigla;
                }
                catch (Exception ex)
                {
                    TempData["Erro"] = "Não foi possível salvar o estágio " + status.StatusSigla + ": " + ex.Message;
                }
            }
            return RedirectToAction("Index", "Status", new { id = status.StatusSigla });
        }

        // GET: /Status/Edit/5
        [PermissoesFiltro(Roles = "Estágio da Proposta")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Status status = db.Status.Find(id);
            if (status == null)
            {
                return HttpNotFound();
            }
            return View(status);
        }

        // POST: /Status/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="StatusSigla,Nome")] Status status)
        {
            if (ModelState.IsValid)
            {
                db.Entry(status).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    TempData["Msg"] = "Estágio da Proposta editado com sucesso.";
                }
                catch (DbUpdateException ex)
                {
                    TempData["Erro"] = "Já existe um estágio com a sigla " + status.StatusSigla;
                }
                catch (Exception ex)
                {
                    TempData["Erro"] = "Não foi possível salvar o estágio " + status.StatusSigla + ": " + ex.Message;
                }
            }
            return RedirectToAction("Details", "Status", new { id = status.StatusSigla });
        }

        // GET: /Status/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Status status = db.Status.Find(id);
            if (status == null)
            {
                return HttpNotFound();
            }
            return View(status);
        }

        // POST: /Status/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Status status = db.Status.Find(id);
            db.Status.Remove(status);
            try
            {
                db.SaveChanges();
                TempData["Msg"] = "Estágio da Proposta deletado com sucesso.";
            }
            catch (DbUpdateException ex)
            {
                TempData["Erro"] = "Não foi possível deletar o Estágio " + status.Nome + ". Alguns registros fazem referência a esse estágio de proposta.";
                return RedirectToAction("Details", "Status", new { id = status.StatusSigla });
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Não foi possível deletar o Estágio " + status.Nome + ": " + ex.Message;
                return RedirectToAction("Details", "Status", new { id = status.StatusSigla });
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
