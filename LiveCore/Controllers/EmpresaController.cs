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
    public class EmpresaController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /Empresa/
        [PermissoesFiltro(Roles = "Empresa")]
        public ActionResult Index()
        {
            if (db.Empresa.ToList().Count() == 0)
            {
                return RedirectToAction("Create");
            }
            else
            {
                Empresa empresa = db.Empresa.ToList().FirstOrDefault();
                return RedirectToAction("Details", new { id = empresa.EmpresaID });
            }
        }

        // GET: /Empresa/Details/5
        [PermissoesFiltro(Roles = "Empresa")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresa.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            if (TempData["Msg"] != null && TempData["Msg"] != "")
            {
                ViewBag.Msg = TempData["Msg"];
            }
            if (TempData["Erro"] != null && TempData["Erro"] != "")
            {
                ViewBag.Erro = TempData["Erro"];
            }
            return View(empresa);
        }

        // GET: /Empresa/Create
        [PermissoesFiltro(Roles = "Empresa")]
        public ActionResult Create()
        {
            Empresa model = new Empresa();
            IEnumerable<UF> ufs = Enum.GetValues(typeof(UF))
                                                       .Cast<UF>();
            model.UFs = from action in ufs
                        select new SelectListItem
                        {
                            Text = action.ToString(),
                            Value = action.ToString()
                        };
            return View(model);
        }

        // POST: /Empresa/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="EmpresaID,RazaoSocial,NomeFantasia,Endereco,Bairro,CEP,Cidade,UF,Telefone,Email,CNPJ,InscEstadual,EmailFinanceiro,EmailComercial")] Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Empresa.Add(empresa);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Erro"] = "Não foi possível salvar a empresa " + empresa.RazaoSocial + ": " + ex.Message;
                }
            }
            return View(empresa);
        }

        // GET: /Empresa/Edit/5
        [PermissoesFiltro(Roles = "Empresa")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresa.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }

            IEnumerable<UF> ufs = Enum.GetValues(typeof(UF))
                                                       .Cast<UF>();
            empresa.UFs = from action in ufs
                        select new SelectListItem
                        {
                            Text = action.ToString(),
                            Value = action.ToString()
                        };
            return View(empresa);
        }

        // POST: /Empresa/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="EmpresaID,RazaoSocial,NomeFantasia,Endereco,Bairro,CEP,Cidade,UF,Telefone,Email,CNPJ,InscEstadual,EmailFinanceiro,EmailComercial")] Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(empresa).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Msg"] = "A empresa " + empresa.RazaoSocial +" foi gravada com sucesso.";
                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    TempData["Erro"] = "Não foi possível salvar a empresa " + empresa.RazaoSocial + ": " + ex.Message;
                }
            }
            return View(empresa);
        }

        // GET: /Empresa/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresa.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            return View(empresa);
        }

        // POST: /Empresa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Empresa empresa = db.Empresa.Find(id);
            db.Empresa.Remove(empresa);
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

        //Colaboradores

        // GET: /Cliente/Edit/5
        [PermissoesFiltro(Roles = "Empresa")]
        public ActionResult Colaboradores(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresa.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            return View(empresa);
        }

        // GET: /Contato/Details/5
        [PermissoesFiltro(Roles = "Empresa")]
        public ActionResult Selecao(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TempData["ContatoID"] = Convert.ToInt32(id);
            ViewData["Contato"] = Convert.ToInt32(id);
            Contato colaborador = new Contato();
            colaborador = db.Contato.Find(id);
            ViewData["ContatoNome"] = colaborador.Nome;
            List<Empresa> empresas = db.Empresa.ToList();

            foreach (var item in colaborador.Empresas)
            {
                empresas.Remove(item);
            }

            if (empresas == null)
            {
                return HttpNotFound();
            }
            return View(empresas);
        }


        public ActionResult Vincular(String idArray)
        {
            String[] ids = idArray.Split(',');
            String retorno = "";

            if (TempData["ContatoID"] == null)
            {
                retorno = "Não existe um colaborador selecionado para vincular empresas.";
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }

            int contatoID = Convert.ToInt32(TempData["ContatoID"]);
            if (contatoID == 0)
            {
                retorno = "Não foi encontrado um contato com o ID " + contatoID;
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }
            Contato colaborador = db.Contato.Find(contatoID);

            foreach (var item in ids)
            {
                if (!item.Equals(""))
                {
                    Empresa empresa = db.Empresa.Find(Convert.ToInt32(item));
                    if (empresa == null)
                    {
                        retorno = "Não foi encontrada nenhuma empresa com o ID " + item;
                        return Json(retorno, JsonRequestBehavior.AllowGet);
                    }

                    empresa.Colaboradores.Add(colaborador);

                    if (ModelState.IsValid)
                    {
                        db.Entry(empresa).State = EntityState.Modified;
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            retorno = "Não foi possível vincular a empresa " + empresa.RazaoSocial + ": " + ex.Message;
                            return Json(retorno, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Desvincular(String idArray, int? contatoID)
        {

            String[] ids = idArray.Split(',');
            String retorno = "";

            if (contatoID == null || contatoID == 0)
            {
                retorno = "Não foi encontrado um contato com o ID " + contatoID;
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }
            Contato colaborador = db.Contato.Find(contatoID);

            foreach (var item in ids)
            {
                if (!item.Equals(""))
                {
                    Empresa empresa = db.Empresa.Find(Convert.ToInt32(item));
                    if (empresa == null)
                    {
                        retorno = "Não foi encontrada nenhuma empresa com o ID " + item;
                        return Json(retorno, JsonRequestBehavior.AllowGet);
                    }

                    empresa.Colaboradores.Remove(colaborador);

                    if (ModelState.IsValid)
                    {
                        db.Entry(empresa).State = EntityState.Modified;
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            retorno = "Não foi possível desvincular a empresa " + empresa.RazaoSocial + ": " + ex.Message;
                            return Json(retorno, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            return Json(retorno, JsonRequestBehavior.AllowGet);
        }
    }
}
