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
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using PagedList;
using System.Data.Entity.Infrastructure;
using LiveCore.Security;

namespace LiveCore.Controllers
{
    public class EntidadeController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /Entidade/
        [PermissoesFiltro(Roles = "Entidade")]
        public ActionResult Index(string ordem, string currentFilter, string currentFilterCNPJ, string currentFilterCPF, string currentFilterUF,
            string nomeSearch, string CNPJSearch, string CPFSearch, string UFSearch, int? page)
        {
            IEnumerable<UF> ufs = Enum.GetValues(typeof(UF)).Cast<UF>();
            ViewBag.UFs = from action in ufs
                          select new SelectListItem
                          {
                              Text = action.ToString(),
                              Value = action.ToString()
                          };
            ViewBag.CurrentSort = ordem;
            ViewBag.NomeSortParm = String.IsNullOrEmpty(ordem) ? "Entidade_desc" : "";
            ViewBag.CidadeSortParm = ordem == "Cidade" ? "Cidade_desc" : "Cidade";
            ViewBag.UFSortParm = ordem == "UF" ? "UF_desc" : "UF";

            if (!String.IsNullOrEmpty(nomeSearch))
            {
                page = 1;
            }
            else
            {
                nomeSearch = currentFilter;
            }

            if (!String.IsNullOrEmpty(CPFSearch))
            {
                page = 1;
            }
            else
            {
                CPFSearch = currentFilterCPF;
            }

            if (!String.IsNullOrEmpty(CNPJSearch))
            {
                page = 1;
            }
            else
            {
                CNPJSearch = currentFilterCNPJ;
            }

            if (!String.IsNullOrEmpty(UFSearch))
            {
                page = 1;
            }
            else
            {
                UFSearch = currentFilterUF;
            }

            ViewBag.CurrentFilterNome = nomeSearch;
            ViewBag.CurrentFilterCPF = CPFSearch;
            ViewBag.CurrentFilterCNPJ = CNPJSearch;
            ViewBag.CurrentFilterUF = UFSearch;

            var entidade = from s in db.Entidade
                          select s;

            if (!String.IsNullOrEmpty(nomeSearch))
            {
                entidade = entidade.Where(s => s.Nome.ToUpper().Contains(nomeSearch.ToUpper()));
            }
            if (!String.IsNullOrEmpty(CPFSearch))
            {
                entidade = entidade.Where(s => s.CPF.ToUpper().Contains(CPFSearch.ToUpper()));
            }
            if (!String.IsNullOrEmpty(CNPJSearch))
            {
                entidade = entidade.Where(s => s.CNPJ.ToUpper().Contains(CNPJSearch.ToUpper()));
            }
            if (!String.IsNullOrEmpty(UFSearch))
            {
                entidade = entidade.Where(s => s.UF.ToUpper().Contains(UFSearch.ToUpper()));
            }

            switch (ordem)
            {
                case "Entidade_desc":
                    entidade = entidade.OrderByDescending(s => s.Nome);
                    break;
                case "Cidade_desc":
                    entidade = entidade.OrderByDescending(s => s.CPF);
                    break;
                case "Cidade":
                    entidade = entidade.OrderBy(s => s.CPF);
                    break;
                case "UF_desc":
                    entidade = entidade.OrderByDescending(s => s.UF);
                    break;
                case "UF":
                    entidade = entidade.OrderBy(s => s.UF);
                    break;
                default:
                    entidade = entidade.OrderBy(s => s.Nome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(entidade.ToPagedList(pageNumber, pageSize));
        }

        // GET: /Contato/Details/5
        public ActionResult VincularPapel(String selectedPapeis, int id)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var selected = JsonConvert.DeserializeObject<List<string>>(selectedPapeis);

            Entidade entidade = db.Entidade.Where(p => p.EntidadeID == id).Include(p => p.Papeis).FirstOrDefault();
            List<Papel> papelList = new List<Papel>();
            foreach (var item in selected.ToList())
            {
                Papel papel = db.Papel.Find(Convert.ToInt32(item));
                papelList.Add(papel);
            }

            entidade.Papeis = papelList;

            var result = false;
            try
            {
                db.Entry(entidade).State = EntityState.Modified;
                db.SaveChanges();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscaPapeisSelecionados(int id)
        {
            Entidade entidade = db.Entidade.Where(p => p.EntidadeID == id).Include(p => p.Papeis).FirstOrDefault();

            var result = "";
            foreach (var item in entidade.Papeis)
            {
                result += item.PapelID + ";";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: /Entidade/Details/5
        [PermissoesFiltro(Roles = "Entidade")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entidade entidade = db.Entidade.Find(id);
            if (entidade == null)
            {
                return HttpNotFound();
            }

            String selecionados = "";

            foreach (var item in entidade.Papeis)
            {
                selecionados += item.PapelID + ";";
            }
            if (TempData["Erro"] != null && !TempData["Erro"].ToString().Equals(""))
            {
                ViewBag.Erro = TempData["Erro"];
            }

            ViewData["Selecionados"] = selecionados;

            return View(entidade);
        }

        // GET: /Entidade/Create
        [PermissoesFiltro(Roles = "Entidade")]
        public ActionResult Create()
        {
            ViewBag.PapelID = new SelectList(db.Papel, "PapelID", "Nome");
			Entidade model = new Entidade();
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

        // POST: /Entidade/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EntidadeID,NomeFantasia,Endereco,Bairro,CEP,Cidade,UF,Telefone,Email,CNPJ,InscEstadual,PapelID,Nome,CPF,TipoEntidade,SNProprietaria")] Entidade entidade)
        {
            if (ModelState.IsValid)
            {
                db.Entidade.Add(entidade);
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    ViewBag.Erro = "Já existe uma entidade com o nome " + entidade.Nome;
                    IEnumerable<UF> ufs = Enum.GetValues(typeof(UF))
                                                       .Cast<UF>();
                    entidade.UFs = from action in ufs
                                select new SelectListItem
                                {
                                    Text = action.ToString(),
                                    Value = action.ToString()
                                };
                    return View(entidade);
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Não foi possível salvar a entidade " + entidade.Nome +": " + ex.Message;
                    return View(entidade);
                }
                return RedirectToAction("Index");
            }

            //ViewBag.PapelID = new SelectList(db.Papel, "PapelID", "Nome", entidade.PapelID);
            return View(entidade);
        }

        // GET: /Entidade/Edit/5
        [PermissoesFiltro(Roles = "Entidade")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entidade entidade = db.Entidade.Find(id);
            if (entidade == null)
            {
                return HttpNotFound();
            }
            //ViewBag.PapelID = new SelectList(db.Papel, "PapelID", "Nome", entidade.PapelID);
			IEnumerable<UF> ufs = Enum.GetValues(typeof(UF))
                                                       .Cast<UF>();
            entidade.UFs = from action in ufs
                          select new SelectListItem
                          {
                              Text = action.ToString(),
                              Value = action.ToString()
                          };

            return View(entidade);
        }

        // GET: /Cliente/Edit/5
        [PermissoesFiltro(Roles = "Entidade")]
        public ActionResult Contatos(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entidade entidade = db.Entidade.Find(id);
            if (entidade == null)
            {
                return HttpNotFound();
            }
            return View(entidade);
        }

        // GET: /Contato/Details/5
        [PermissoesFiltro(Roles = "Entidade")]
        public ActionResult Selecao(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TempData["ContatoID"] = Convert.ToInt32(id);
            ViewData["Contato"] = Convert.ToInt32(id);
            Contato contato = new Contato();
            contato = db.Contato.Find(id);
            ViewData["ContatoNome"] = contato.Nome;
            List<Entidade> entidades = db.Entidade.ToList();

            foreach (var item in contato.Entidades)
            {
                entidades.Remove(item);
            }

            if (entidades == null)
            {
                return HttpNotFound();
            }
            return View(entidades);
        }


        public ActionResult Vincular(String idArray)
        {
            String[] ids = idArray.Split(',');
            String retorno = "";

            if (TempData["ContatoID"] == null)
            {
                retorno = "Não existe um contato selecionado para vincular entidades.";
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }

            int contatoID = Convert.ToInt32(TempData["ContatoID"]);
            if (contatoID == 0)
            {
                retorno = "Não foi encontrado um contato com o ID " + contatoID;
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }
            Contato contato = db.Contato.Find(contatoID);

            foreach (var item in ids)
            {
                if (!item.Equals(""))
                {
                    Entidade entidade = db.Entidade.Find(Convert.ToInt32(item));
                    if (entidade == null)
                    {
                        retorno = "Não foi encontrada nenhuma entidade com o ID " + item;
                        return Json(retorno, JsonRequestBehavior.AllowGet);
                    }

                    entidade.Contato.Add(contato);

                    if (ModelState.IsValid)
                    {
                        db.Entry(entidade).State = EntityState.Modified;
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            retorno = "Não foi possível vincular a entidade " + entidade.Nome + ": " + ex.Message;
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
            Contato contato = db.Contato.Find(contatoID);

            foreach (var item in ids)
            {
                if (!item.Equals(""))
                {
                    Entidade entidade = db.Entidade.Find(Convert.ToInt32(item));
                    if (entidade == null)
                    {
                        retorno = "Não foi encontrada nenhuma entidade com o ID " + item;
                        return Json(retorno, JsonRequestBehavior.AllowGet);
                    }

                    entidade.Contato.Remove(contato);

                    if (ModelState.IsValid)
                    {
                        db.Entry(entidade).State = EntityState.Modified;
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            retorno = "Não foi possível desvincular a entidade " + entidade.Nome + ": " + ex.Message;
                            return Json(retorno, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        // POST: /Entidade/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EntidadeID,NomeFantasia,Endereco,Bairro,CEP,Cidade,UF,Telefone,Email,CNPJ,InscEstadual,PapelID,Nome,CPF,TipoEntidade,SNProprietaria")] Entidade entidade)
        {
            if (ModelState.IsValid)
            {
                db.Entry(entidade).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    ViewBag.Erro = "Já existe uma entidade com o nome " + entidade.Nome;
                    return View(entidade);
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Não foi possível salvar a entidade " + entidade.Nome + ": " + ex.Message;
                    return View(entidade);
                }
                return RedirectToAction("Index");
            }
           // ViewBag.PapelID = new SelectList(db.Papel, "PapelID", "Nome", entidade.PapelID);
            return View(entidade);
        }

        // GET: /Entidade/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entidade entidade = db.Entidade.Find(id);
            if (entidade == null)
            {
                return HttpNotFound();
            }
            return View(entidade);
        }

        // POST: /Entidade/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Entidade entidade = db.Entidade.Find(id);
            db.Entry(entidade).Collection("Contato").Load();
            entidade.Contato.Clear();
            db.Entry(entidade).Collection("Papeis").Load();
            entidade.Papeis.Clear();
            db.Entidade.Remove(entidade);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                TempData["Erro"] = "Não foi possível deletar a entidade " + entidade.Nome + ". Alguns registros fazem referência a essa entidade.";
                return RedirectToAction("Details", "Entidade", new { id = entidade.EntidadeID });
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Não foi possível deletar a entidade " + entidade.Nome + ": " + ex.Message;
                return RedirectToAction("Details", "Entidade", new { id = entidade.EntidadeID });
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
