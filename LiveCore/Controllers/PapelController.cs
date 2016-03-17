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
    public class PapelController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /Papel/
        [PermissoesFiltro(Roles = "Papel da Entidade")]
        public ActionResult Index(string ordem, string currentFilter, string nomeSearch, int? page)
        {
            ViewBag.CurrentSort = ordem;
            ViewBag.NomeSortParm = String.IsNullOrEmpty(ordem) ? "Papel_desc" : "";
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

            var papel = from s in db.Papel
                          select s;

            if (!String.IsNullOrEmpty(nomeSearch))
            {
                papel = papel.Where(s => s.Nome.ToUpper().Contains(nomeSearch.ToUpper()));
            }

            switch (ordem)
            {
                case "Descrição_desc":
                    papel = papel.OrderByDescending(s => s.Descricao);
                    break;
                case "Papel_desc":
                    papel = papel.OrderByDescending(s => s.Nome);
                    break;
                case "Descrição":
                    papel = papel.OrderBy(s => s.Descricao);
                    break;
                default:
                    papel = papel.OrderBy(s => s.Nome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(papel.ToPagedList(pageNumber, pageSize));
        }

        // GET: /PapelContato/
        [PermissoesFiltro(Roles = "Entidade")]
        public ActionResult VincularPapel()
        {
            return View(db.Papel.ToList());
        }

        // GET: /Papel/Details/5
        [PermissoesFiltro(Roles = "Papel da Entidade")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Papel papel = db.Papel.Find(id);
            if (papel == null)
            {
                return HttpNotFound();
            }
            if (TempData["Erro"] != null && !TempData["Erro"].ToString().Equals(""))
            {
                ViewBag.Erro = TempData["Erro"];
            }
            return View(papel);
        }

        // GET: /Papel/Create
        [PermissoesFiltro(Roles = "Papel da Entidade")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Papel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="PapelID,Nome,Descricao")] Papel papel)
        {
            if (ModelState.IsValid)
            {
                db.Papel.Add(papel);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Não foi possível salvar o papel " + papel.Nome + ": " + ex.Message;
                    return View(papel);
                }
                return RedirectToAction("Index");
            }

            return View(papel);
        }

        // GET: /Papel/Edit/5
        [PermissoesFiltro(Roles = "Papel da Entidade")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Papel papel = db.Papel.Find(id);
            if (papel == null)
            {
                return HttpNotFound();
            }
            return View(papel);
        }

        // POST: /Papel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="PapelID,Nome,Descricao")] Papel papel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(papel).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Não foi possível salvar o papel " + papel.Nome + ": " + ex.Message;
                    return View(papel);
                }
                return RedirectToAction("Index");
            }
            return View(papel);
        }

        // GET: /Papel/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Papel papel = db.Papel.Find(id);
            if (papel == null)
            {
                return HttpNotFound();
            }
            return View(papel);
        }

        // POST: /Papel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Papel papel = db.Papel.Find(id);
            db.Papel.Remove(papel);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                TempData["Erro"] = "Não foi possível deletar o Papel " + papel.Nome + ". Alguns registros fazem referência a esse Papel.";
                return RedirectToAction("Details", "Papel", new { id = papel.PapelID});
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Não foi possível Deletar o Papel " + papel.Nome+ ": " + ex.Message;
                return RedirectToAction("Details", "Papel", new { id = papel.PapelID});
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
