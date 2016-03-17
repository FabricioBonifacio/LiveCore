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
    public class AreaEscopoController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /AreaEscopo/
        [PermissoesFiltro(Roles = "Área de Escopo")]
        public ActionResult Index(string ordem, string currentFilter, string nomeSearch, int? page)
        {
            ViewBag.CurrentSort = ordem;
            ViewBag.NomeSortParm = String.IsNullOrEmpty(ordem) ? "Área de Escopo_desc" : "";
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

            var areaEscopo = from s in db.AreaEscopo
                          select s;

            if (!String.IsNullOrEmpty(nomeSearch))
            {
                areaEscopo = areaEscopo.Where(s => s.Nome.ToUpper().Contains(nomeSearch.ToUpper()));
            }

            switch (ordem)
            {
                case "Descrição_desc":
                    areaEscopo = areaEscopo.OrderByDescending(s => s.Descricao);
                    break;
                case "Área de Escopo_desc":
                    areaEscopo = areaEscopo.OrderByDescending(s => s.Nome);
                    break;
                case "Descrição":
                    areaEscopo = areaEscopo.OrderBy(s => s.Descricao);
                    break;
                default:
                    areaEscopo = areaEscopo.OrderBy(s => s.Nome);
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
            return View(areaEscopo.ToPagedList(pageNumber, pageSize));
        }

        // GET: /AreaEscopo/Details/5
        [PermissoesFiltro(Roles = "Área de Escopo")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AreaEscopo areaescopo = db.AreaEscopo.Find(id);
            if (areaescopo == null)
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
            return View(areaescopo);
        }

        public JsonResult AutoCompleteAreaEscopo(String term)
        {
            var result = (from r in db.AreaEscopo
                          where r.Nome.ToLower().Contains(term.ToLower())
                          select new { r.Nome }).Distinct();
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperaAreaEscopoPorNome(String areaEscopo)
        {
            AreaEscopo ae = db.AreaEscopo.Where(p => p.Nome.ToUpper().Equals(areaEscopo.ToUpper())).FirstOrDefault();

            var result = 0;

            if (ae != null)
            {
                result = ae.AreaEscopoID;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperaAreaEscopoPorId(int areaEscopoID)
        {
            AreaEscopo tc = db.AreaEscopo.Find(areaEscopoID);

            var result = tc.Nome;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CriarAreaEscopo(String areaEscopoNovo)
        {
            AreaEscopo areaEscopo = new AreaEscopo();

            areaEscopo.Nome = areaEscopoNovo;

            var result = 0;

            try
            {
                db.AreaEscopo.Add(areaEscopo);
                db.SaveChanges();
                result = (db.AreaEscopo.Where(p => p.Nome.ToUpper().Equals(areaEscopoNovo.ToUpper())).FirstOrDefault()).AreaEscopoID;
            }
            catch (Exception ex)
            {
                //result = "Erro ao salvar Tipo de Contrato: " + ex.Message;   
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditAreaEscopoModal(int areaEscopoID, String nome, String descricao)
        {
            String retorno = "";
            AreaEscopo areaEscopo = new AreaEscopo();

            areaEscopo.AreaEscopoID = areaEscopoID;
            areaEscopo.Nome = nome;
            areaEscopo.Descricao = descricao;

            db.Entry(areaEscopo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                retorno = "Área de Escopo editada com sucesso.";
            }
            catch (DbUpdateException ex)
            {
                retorno = ex.Message;
            }
            catch (Exception ex)
            {
                retorno = "Não foi possível salvar a área de escopo " + areaEscopo.Nome + ": " + ex.Message;
            }

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        // GET: /AreaEscopo/Create
        [PermissoesFiltro(Roles = "Área de Escopo")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /AreaEscopo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="AreaEscopoID,Nome,Descricao")] AreaEscopo areaescopo)
        {
            if (ModelState.IsValid)
            {
                db.AreaEscopo.Add(areaescopo);
                try
                {
                    db.SaveChanges();
                    TempData["Msg"] = "Área de Escopo cadastrada com sucesso.";
                }
                catch (Exception ex)
                {
                    TempData["Erro"] = "Não foi possível salvar a área de escopo " + areaescopo.Nome + ": " + ex.Message;
                }
            }
            return RedirectToAction("Index", "AreaEscopo", new { id = areaescopo.AreaEscopoID });
        }

        // GET: /AreaEscopo/Edit/5
        [PermissoesFiltro(Roles = "Área de Escopo")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AreaEscopo areaescopo = db.AreaEscopo.Find(id);
            if (areaescopo == null)
            {
                return HttpNotFound();
            }
            return View(areaescopo);
        }

        // POST: /AreaEscopo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="AreaEscopoID,Nome,Descricao")] AreaEscopo areaescopo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(areaescopo).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    TempData["Msg"] = "Área de Escopo editada com sucesso.";
                }
                catch (Exception ex)
                {
                    TempData["Erro"] = "Não foi possível salvar a área de escopo " + areaescopo.Nome + ": " + ex.Message;
                }
            }
            return RedirectToAction("Details", "AreaEscopo", new { id = areaescopo.AreaEscopoID });
        }

        // GET: /AreaEscopo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AreaEscopo areaescopo = db.AreaEscopo.Find(id);
            if (areaescopo == null)
            {
                return HttpNotFound();
            }
            return View(areaescopo);
        }

        // POST: /AreaEscopo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AreaEscopo areaescopo = db.AreaEscopo.Find(id);
            db.AreaEscopo.Remove(areaescopo);
            try
            {
                db.SaveChanges();
                TempData["Msg"] = "Área de Escopo deletada com sucesso.";
            }
            catch (DbUpdateException ex)
            {
                TempData["Erro"] = "Não foi possível deletar a Área de Escopo " + areaescopo.Nome + ". Alguns registros fazem referência a essa Área de Escopo.";
                return RedirectToAction("Details", "AreaEscopo", new { id = areaescopo.AreaEscopoID });
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Não foi possível deletar Área de Escopo " + areaescopo.Nome + ": " + ex.Message;
                return RedirectToAction("Details", "AreaEscopo", new { id = areaescopo.AreaEscopoID });
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
