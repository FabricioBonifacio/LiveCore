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
using System.Data.Entity.Infrastructure;
using PagedList;
using LiveCore.Security;

namespace LiveCore.Controllers
{
    public class PapelContatoController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /PapelContato/
        [PermissoesFiltro(Roles = "Papel do Contato")]
        public ActionResult Index(string ordem, string currentFilter, string nomeSearch, int? page)
        {
            ViewBag.CurrentSort = ordem;
            ViewBag.NomeSortParm = String.IsNullOrEmpty(ordem) ? "Papel do Contato_desc" : "";
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

            var papelContato = from s in db.PapelContato
                          select s;

            if (!String.IsNullOrEmpty(nomeSearch))
            {
                papelContato = papelContato.Where(s => s.Nome.ToUpper().Contains(nomeSearch.ToUpper()));
            }

            switch (ordem)
            {
                case "Descrição_desc":
                    papelContato = papelContato.OrderByDescending(s => s.Descricao);
                    break;
                case "Descrição":
                    papelContato = papelContato.OrderBy(s => s.Descricao);
                    break;
                case "Papel do Contato_desc":
                    papelContato = papelContato.OrderByDescending(s => s.Nome);
                    break;
                default:
                    papelContato = papelContato.OrderBy(s => s.Nome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(papelContato.ToPagedList(pageNumber, pageSize));
        }

        // GET: /PapelContato/
        [PermissoesFiltro(Roles = "Contato")]
        public ActionResult VincularPapel(int? id)
        {
            return View(db.PapelContato.ToList());
        }

        // GET: /PapelContato/Details/5
        [PermissoesFiltro(Roles = "Papel do Contato")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PapelContato papelcontato = db.PapelContato.Find(id);
            if (papelcontato == null)
            {
                return HttpNotFound();
            }
            if (TempData["Erro"] != null && !TempData["Erro"].ToString().Equals(""))
            {
                ViewBag.Erro = TempData["Erro"];
            }
            return View(papelcontato);
        }

        // GET: /PapelContato/Create
        [PermissoesFiltro(Roles = "Papel do Contato")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /PapelContato/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="PapelContatoID,Nome,Descricao")] PapelContato papelcontato)
        {
            if (ModelState.IsValid)
            {
                db.PapelContato.Add(papelcontato);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Não foi possível salvar o papel " + papelcontato.Nome + ": " + ex.Message;
                    return View(papelcontato);
                }
                return RedirectToAction("Index");
            }

            return View(papelcontato);
        }

        // GET: /PapelContato/Edit/5
        [PermissoesFiltro(Roles = "Papel do Contato")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PapelContato papelcontato = db.PapelContato.Find(id);
            if (papelcontato == null)
            {
                return HttpNotFound();
            }
            return View(papelcontato);
        }

        // POST: /PapelContato/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="PapelContatoID,Nome,Descricao")] PapelContato papelcontato)
        {
            if (ModelState.IsValid)
            {
                db.Entry(papelcontato).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Não foi possível salvar o papel " + papelcontato.Nome + ": " + ex.Message;
                    return View(papelcontato);
                }
                return RedirectToAction("Index");
            }
            return View(papelcontato);
        }

        // GET: /PapelContato/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PapelContato papelcontato = db.PapelContato.Find(id);
            if (papelcontato == null)
            {
                return HttpNotFound();
            }
            return View(papelcontato);
        }

        // POST: /PapelContato/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PapelContato papelcontato = db.PapelContato.Find(id);
            db.PapelContato.Remove(papelcontato);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                TempData["Erro"] = "Não foi possível deletar o papel " + papelcontato.Nome + ". Alguns registros fazem referência a esse papel.";
                return RedirectToAction("Details", "PapelContato", new { id = papelcontato.PapelContatoID });
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Não foi possível deletar o papel " + papelcontato.Nome + ": " + ex.Message;
                return RedirectToAction("Details", "PapelContato", new { id = papelcontato.PapelContatoID });
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
