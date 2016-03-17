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
    public class PermissaoController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /Permissao/
        public ActionResult Index(string ordem, string currentFilter, string nomeSearch, int? page)
        {
            ViewBag.CurrentSort = ordem;
            ViewBag.NomeSortParm = String.IsNullOrEmpty(ordem) ? "Nome_desc" : "";

            if (nomeSearch != null)
            {
                page = 1;
            }
            else
            {
                nomeSearch = currentFilter;
            }

            ViewBag.CurrentFilter = nomeSearch;

            var permissao = from s in db.Permissao
                        select s;

            if (!String.IsNullOrEmpty(nomeSearch))
            {
                permissao = permissao.Where(s => s.Nome.ToUpper().Contains(nomeSearch.ToUpper()));
            }

            switch (ordem)
            {
                case "Nome_desc":
                    permissao = permissao.OrderByDescending(s => s.Nome);
                    break;
                default:
                    permissao = permissao.OrderBy(s => s.Nome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(permissao.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult VincularPermissao()
        {
            return View(db.Permissao.ToList());
        }

        // GET: /Permissao/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permissao permissao = db.Permissao.Find(id);
            if (permissao == null)
            {
                return HttpNotFound();
            }
            return View(permissao);
        }

        // GET: /Permissao/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Permissao/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="PermissaoID,Nome")] Permissao permissao)
        {
            if (ModelState.IsValid)
            {
                db.Permissao.Add(permissao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(permissao);
        }

        // GET: /Permissao/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permissao permissao = db.Permissao.Find(id);
            if (permissao == null)
            {
                return HttpNotFound();
            }
            return View(permissao);
        }

        // POST: /Permissao/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="PermissaoID,Nome")] Permissao permissao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(permissao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(permissao);
        }

        // GET: /Permissao/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permissao permissao = db.Permissao.Find(id);
            if (permissao == null)
            {
                return HttpNotFound();
            }
            return View(permissao);
        }

        // POST: /Permissao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Permissao permissao = db.Permissao.Find(id);
            db.Permissao.Remove(permissao);
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
