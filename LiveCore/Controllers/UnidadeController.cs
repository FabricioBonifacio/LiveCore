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
    public class UnidadeController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /Unidade/
        [PermissoesFiltro(Roles = "Unidade")]
        public ActionResult Index(string ordem, string currentFilter, string nomeSearch, int? page)
        {
            ViewBag.CurrentSort = ordem;
            ViewBag.NomeSortParm = ordem == "Unidade" ? "Unidade_desc" : "Unidade";

            if (nomeSearch != null)
            {
                page = 1;
            }
            else
            {
                nomeSearch = currentFilter;
            }

            ViewBag.CurrentFilter = nomeSearch;

            var unidade = from s in db.Unidade
                               select s;

            if (!String.IsNullOrEmpty(nomeSearch))
            {
                unidade = unidade.Where(s => s.Nome.ToUpper().Contains(nomeSearch.ToUpper()));
            }

            switch (ordem)
            {
                case "Unidade_desc":
                    unidade = unidade.OrderByDescending(s => s.Nome);
                    break;
                default:
                    unidade = unidade.OrderBy(s => s.Nome);
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

            return View(unidade.ToPagedList(pageNumber, pageSize));
        }

        // GET: /Unidade/Details/5
        [PermissoesFiltro(Roles = "Unidade")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unidade unidade = db.Unidade.Find(id);
            if (unidade == null)
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

            return View(unidade);
        }

        // GET: /Unidade/Create
        [PermissoesFiltro(Roles = "Unidade")]
        public ActionResult Create()
        {
            return View();
        }

        // GET: /Unidade/Create
        public ActionResult CreateColapso()
        {
            return View();
        }

        // POST: /Unidade/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="UnidadeSigla,Nome")] Unidade unidade)
        {
            if (ModelState.IsValid)
            {
                db.Unidade.Add(unidade);
                try {
                    db.SaveChanges();
                    TempData["Msg"] = "Unidade cadastrada com sucesso.";
                }
                catch(DbUpdateException ex){
                    TempData["Erro"] = "Já existe uma unidade com a sigla " + unidade.UnidadeSigla;
                }
                catch(Exception ex)
                {
                    TempData["Erro"] = "Não foi possível salvar a unidade " + unidade.UnidadeSigla + ": " + ex.Message;
                }
            }
            return RedirectToAction("Index", "Unidade", new { id = unidade.UnidadeSigla });
        }

        // GET: /Unidade/Edit/5
        [PermissoesFiltro(Roles = "Unidade")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unidade unidade = db.Unidade.Find(id);
            if (unidade == null)
            {
                return HttpNotFound();
            }
            return View(unidade);
        }

        // GET: /Unidade/Edit/5
        public ActionResult EditColapso(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unidade unidade = db.Unidade.Find(id);
            if (unidade == null)
            {
                return HttpNotFound();
            }
            return View(unidade);
        }

        // POST: /Unidade/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="UnidadeSigla,Nome")] Unidade unidade)
        {

            bool retorno = false;

            if (ModelState.IsValid)
            {
                db.Entry(unidade).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    retorno = true;
                    TempData["Msg"] = "Unidade editada com sucesso.";
                }
                catch (DbUpdateException ex)
                {
                    TempData["Erro"] = "Já existe uma unidade com a sigla " + unidade.UnidadeSigla;
                }
                catch (Exception ex)
                {
                    TempData["Erro"] = "Não foi possível salvar a unidade " + unidade.UnidadeSigla + ": " + ex.Message;
                }
            }
            return RedirectToAction("Details", "Unidade", new { id = unidade.UnidadeSigla });
        }

        public JsonResult AutoCompleteUnidade(String term)
        {
            var result = (from r in db.Unidade
                          where r.Nome.ToLower().Contains(term.ToLower())
                          select new { r.Nome }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperaUnidadePorNome(String unidade)
        {
            Unidade un = db.Unidade.Where(p => p.Nome.ToUpper().Equals(unidade.ToUpper())).FirstOrDefault();

            var result = "";

            if (un != null)
            {
                result = un.UnidadeSigla;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperaUnidadePorId(String unidadeID)
        {
            var result = "";
            Unidade un = db.Unidade.Find(unidadeID);

            if(un != null) 
            {
                result = un.Nome;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CriarUnidade(String unidadeNovo)
        {
            Unidade unidade = new Unidade();

            unidade.Nome = unidadeNovo;
            unidade.UnidadeSigla = unidadeNovo.Substring(0, 2).ToUpper();

            var result = "";

            try
            {
                db.Unidade.Add(unidade);
                db.SaveChanges();
                result = (db.Unidade.Where(p => p.Nome.ToUpper().Equals(unidadeNovo.ToUpper())).FirstOrDefault()).UnidadeSigla;
            }
            catch (Exception ex)
            {
                //result = "Erro ao salvar Tipo de Contrato: " + ex.Message;   
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CriarUnidadeColapso(String unidadeSigla, String unidadeNome)
        {
            Unidade unidade = new Unidade();

            unidade.UnidadeSigla = unidadeSigla;
            unidade.Nome = unidadeNome;

            var result = "";

            try
            {
                db.Unidade.Add(unidade);
                db.SaveChanges();
                result = (db.Unidade.Where(p => p.UnidadeSigla.ToUpper().Equals(unidadeSigla.ToUpper())).FirstOrDefault()).UnidadeSigla;
            }
            catch (Exception ex)
            {
                //result = "Erro ao salvar Tipo de Contrato: " + ex.Message;   
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditUnidadeModal(String unidadeSigla, String nome)
        {
            String retorno = "";
            Unidade unidade = new Unidade();

            unidade.UnidadeSigla = unidadeSigla;
            unidade.Nome = nome;

            db.Entry(unidade).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                retorno = "Unidade editada com sucesso.";
            }
            catch (DbUpdateException ex)
            {
                retorno = ex.Message;
            }
            catch (Exception ex)
            {
                retorno = "Não foi possível salvar a unidade " + unidade.UnidadeSigla + ": " + ex.Message;
            }

            return Json(retorno, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Details", "Unidade", new { id = unidade.UnidadeSigla });
        }

        // GET: /Unidade/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unidade unidade = db.Unidade.Find(id);
            if (unidade == null)
            {
                return HttpNotFound();
            }
            return View(unidade);
        }

        // POST: /Unidade/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Unidade unidade = db.Unidade.Find(id);
            db.Unidade.Remove(unidade);
            try
            {
                db.SaveChanges();
                TempData["Msg"] = "Unidade deletada com sucesso.";
            }
            catch (DbUpdateException ex)
            {
                TempData["Erro"] = "Não foi possível deletar a Unidade " + unidade.UnidadeSigla + ". Alguns registros fazem referência a essa unidade.";
                return RedirectToAction("Details", "Unidade", new { id = unidade.UnidadeSigla });
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Não foi possível deletar a unidade " + unidade.UnidadeSigla + ": " + ex.Message;
                return RedirectToAction("Details", "Unidade", new { id = unidade.UnidadeSigla });
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
