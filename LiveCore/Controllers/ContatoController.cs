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
    public class ContatoController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();
        
        // GET: /Contato/
        [PermissoesFiltro(Roles = "Contato")]
        public ActionResult Index(string ordem, string currentFilter, string currentFilterLogin, string currentFilterDtRegistroInicio, string currentFilterDtRegistroFinal,
            string nomeSearch, string loginSearch, string dtRegistroInicioSearch, string dtRegistroFinalSearch, int? page)
        {
            ViewBag.CurrentSort = ordem;
            ViewBag.NomeSortParm = String.IsNullOrEmpty(ordem) ? "Contato_desc" : "";
            ViewBag.LoginSortParm = ordem == "Login" ? "Login_desc" : "Login";
            ViewBag.DtRegistroSortParm = ordem == "Data de Registro" ? "Data de Registro_desc" : "Data de Registro";

            if (!String.IsNullOrEmpty(nomeSearch))
            {
                page = 1;
            }
            else
            {
                nomeSearch = currentFilter;
            }

            if (!String.IsNullOrEmpty(loginSearch))
            {
                page = 1;
            }
            else
            {
                loginSearch = currentFilterLogin;
            }

            if (!String.IsNullOrEmpty(dtRegistroInicioSearch))
            {
                page = 1;
            }
            else
            {
                dtRegistroInicioSearch = currentFilterDtRegistroInicio;
            }

            if (!String.IsNullOrEmpty(dtRegistroFinalSearch))
            {
                page = 1;
            }
            else
            {
                dtRegistroFinalSearch = currentFilterDtRegistroFinal;
            }

            ViewBag.CurrentFilterNome = nomeSearch;
            ViewBag.CurrentFilterLogin = loginSearch;
            ViewBag.CurrentFilterDtRegistroInicio = dtRegistroInicioSearch;
            ViewBag.CurrentFilterDtRegistroFinal = dtRegistroFinalSearch;

            var contato = from s in db.Contato
                          select s;

            if (!String.IsNullOrEmpty(nomeSearch))
            {
                contato = contato.Where(s => s.Nome.ToUpper().Contains(nomeSearch.ToUpper()));
            }
            if (!String.IsNullOrEmpty(loginSearch))
            {
                contato = contato.Where(s => s.Login.ToUpper().Contains(loginSearch.ToUpper()));
            }
            if (!String.IsNullOrEmpty(dtRegistroInicioSearch) && !String.IsNullOrEmpty(dtRegistroFinalSearch))
            {
                DateTime dataInicio = Convert.ToDateTime(dtRegistroInicioSearch);
                DateTime dataFinal = Convert.ToDateTime(dtRegistroFinalSearch);
                contato = contato.Where(s => s.DtRegistro >= dataInicio).Where(s => s.DtRegistro <= dataFinal);
            }

            switch (ordem)
            {
                case "Contato_desc":
                    contato = contato.OrderByDescending(s => s.Nome);
                    break;
                case "Login_desc":
                    contato = contato.OrderByDescending(s => s.Login);
                    break;
                case "Login":
                    contato = contato.OrderBy(s => s.Login);
                    break;
                case "Data de Registro_desc":
                    contato = contato.OrderByDescending(s => s.DtRegistro);
                    break;
                case "Data de Registro":
                    contato = contato.OrderBy(s => s.DtRegistro);
                    break;
                default:
                    contato = contato.OrderBy(s => s.Nome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(contato.ToPagedList(pageNumber, pageSize));
        }

        // GET: /Contato/Details/5
        [PermissoesFiltro(Roles = "Contato")]
        public ActionResult Details(int? id, String erroMsg, String msg)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contato contato = db.Contato.Where(p => p.ContatoID == id).Include(p => p.PapelContatos).FirstOrDefault();
            Usuario usuario = db.Usuario.Where(p => p.Login.Equals(contato.Login)).FirstOrDefault();
            if (contato == null)
            {
                return HttpNotFound();
            }

            String selecionados = "";
            String selecionadosPermissoes = "";

            foreach (var item in contato.PapelContatos)
	        {
                selecionados += item.PapelContatoID + ";";
	        }

            if(usuario != null){
                foreach (var item in usuario.Permissoes)
                {
                    selecionadosPermissoes += item.PermissaoID + ";";
                }
            }

            if (TempData["Erro"] != null && !TempData["Erro"].ToString().Equals(""))
            {
                ViewBag.Erro = TempData["Erro"];
            }
            if(erroMsg != null && !erroMsg.Equals(""))
            {
                ViewBag.Erro = erroMsg;
            }
            if (msg != null && !msg.Equals(""))
            {
                ViewBag.Msg = msg;
            }
            ViewData["Selecionados"] = selecionados;
            ViewData["SelecionadosPermissoes"] = selecionadosPermissoes;
            
            return View(contato);
        }

        public JsonResult AutoCompleteContato(String term, int ident)
        {
            if (ident == -1)
            {
                Empresa empresa = db.Empresa.FirstOrDefault();

                var result = (from r in db.Contato
                              where r.Nome.ToLower().Contains(term.ToLower())
                              && r.Empresas.Any(item => item.EmpresaID == empresa.EmpresaID)
                              select new { r.Nome }).Distinct();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Entidade entidade = db.Entidade.Find(ident);

                var result = (from r in db.Contato
                              where r.Nome.ToLower().Contains(term.ToLower())
                              && r.Entidades.Any(item => item.EntidadeID == entidade.EntidadeID)
                              select new { r.Nome }).Distinct();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            
        }

        public JsonResult RecuperaContatoPorNome(String contato, int entidade)
        {
            Contato cont = db.Contato.Where(p => p.Nome.ToUpper().Equals(contato.ToUpper())).FirstOrDefault();

            var result = 0;

            if (cont != null)
            {
                result = cont.ContatoID;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperaContatoPorId(String contatoID)
        {
            var result = "";
            Contato cont = db.Contato.Find(contatoID);

            if (cont != null)
            {
                result = cont.Nome;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CriarContato(String contatoNovo)
        {
            Contato contato = new Contato();

            contato.Nome = contatoNovo;

            var result = 0;

            try
            {
                db.Contato.Add(contato);
                db.SaveChanges();
                result = (db.Contato.Where(p => p.Nome.ToUpper().Equals(contatoNovo.ToUpper())).FirstOrDefault()).ContatoID;
            }
            catch (Exception ex)
            {
                //result = "Erro ao salvar Tipo de Contrato: " + ex.Message;   
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditContatoModal(int contatoID, String nome, String telefone, String email)
        {
            String retorno = "";
            Contato contato = new Contato();

            contato.ContatoID = contatoID;
            contato.Nome = nome;
            contato.Telefone = telefone;
            contato.Email = email;

            db.Entry(contato).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                retorno = "Contato editado com sucesso.";
            }
            catch (DbUpdateException ex)
            {
                retorno = ex.Message;
            }
            catch (Exception ex)
            {
                retorno = "Não foi possível salvar o contato " + contato.Nome + ": " + ex.Message;
            }

            return Json(retorno, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Details", "Unidade", new { id = unidade.UnidadeSigla });
        }

        // GET: /Contato/Details/5
        public ActionResult VincularPapel(String selectedPapeis, int id)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var selected = JsonConvert.DeserializeObject<List<string>>(selectedPapeis);

            Contato contato = db.Contato.Where(p => p.ContatoID == id).Include(p => p.PapelContatos).FirstOrDefault();
            List<PapelContato> papelList = new List<PapelContato>();
            foreach (var item in selected.ToList())
            {
                PapelContato pc = db.PapelContato.Find(Convert.ToInt32(item));
                papelList.Add(pc);
            }

            contato.PapelContatos = papelList;

            var result = false;
            try
            {
                db.Entry(contato).State = EntityState.Modified;
                db.SaveChanges();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VincularPermissoes(String selectedPermissoes, int id)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var selected = JsonConvert.DeserializeObject<List<string>>(selectedPermissoes);
            var result = false;

            Contato contato = db.Contato.Where(p => p.ContatoID == id).FirstOrDefault();
            Usuario usuario = db.Usuario.Where(p => p.Login.Equals(contato.Login)).Include(p => p.Permissoes).FirstOrDefault();
            if(usuario != null){

                List<Permissao> permissaoList = new List<Permissao>();
                foreach (var item in selected.ToList())
                {
                    Permissao permissao = db.Permissao.Find(Convert.ToInt32(item));
                    permissaoList.Add(permissao);
                }

                usuario.Permissoes = permissaoList;

                try
                {
                    db.Entry(usuario).State = EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    result = false;
                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult BuscaPapeisSelecionados(int id)
        {
            Contato contato = db.Contato.Where(p => p.ContatoID == id).Include(p => p.PapelContatos).FirstOrDefault();

            var result = "";
            foreach (var item in contato.PapelContatos)
            {
                result += item.PapelContatoID + ";";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscaPermissoesSelecionadas(int id)
        {
            var result = "";
            Contato contato = db.Contato.Where(p => p.ContatoID == id).Include(p => p.PapelContatos).FirstOrDefault();
            Usuario usuario = db.Usuario.Where(p => p.Login.Equals(contato.Login)).Include(p => p.Permissoes).FirstOrDefault();
            if (usuario != null)
            {
                foreach (var item in usuario.Permissoes)
                {
                    result += item.PermissaoID + ";";
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        // GET: /Contato/Details/5
        [PermissoesFiltro(Roles = "Contato")]
        public ActionResult Selecao(int? id, String remetente, int? page, string ordem, string currentFilter, string nomeSearch, string selecionados)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.CurrentSort = ordem;
            ViewBag.NomeSortParm = String.IsNullOrEmpty(ordem) ? "Nome_desc" : "";

            TempData["entidadeID"] = Convert.ToInt32(id);
            ViewData["Entidade"] = Convert.ToInt32(id);
            ViewData["Remetente"] = remetente;

            if (TempData["Selecionados"] != null && !TempData["Selecionados"].ToString().Trim().Equals(""))
            {
                ViewData["Selecionados"] = TempData["Selecionados"];
            }
            else
            {
                ViewData["Selecionados"] = selecionados;
            }
            
            if (!String.IsNullOrEmpty(nomeSearch))
            {
                page = 1;
            }
            else
            {
                nomeSearch = currentFilter;
            }

            ViewBag.CurrentFilterNome = nomeSearch;

            var contatos = db.Contato.ToList();

            if (!String.IsNullOrEmpty(nomeSearch))
            {
                contatos = contatos.Where(s => s.Nome.ToUpper().Contains(nomeSearch.ToUpper())).ToList();
            }

            if (remetente.Equals("Entidade"))
            {
                Entidade entidade = new Entidade();
                entidade = db.Entidade.Find(id);
                ViewData["EntidadeNome"] = entidade.Nome;

                foreach (var item in entidade.Contato)
                {
                    contatos.Remove(item);
                }
            }
            else if (remetente.Equals("Empresa"))
            {
                Empresa entidade = new Empresa();
                entidade = db.Empresa.Find(id);
                ViewData["EntidadeNome"] = entidade.RazaoSocial;

                foreach (var item in entidade.Colaboradores)
                {
                    contatos.Remove(item);
                }
            }
             
            if (contatos == null)
            {
                return HttpNotFound();
            }

            switch (ordem)
            {
                case "Nome_desc":
                    contatos = contatos.OrderByDescending(p => p.Nome).ToList();
                    break;
                default:
                    contatos = contatos.OrderBy(s => s.Nome).ToList();
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(contatos.ToPagedList(pageNumber, pageSize));
            //return View(contatos);
        }

        public ActionResult SelecaoHelp(String selecao, String contatoMarcado, String contatoDesmarcado)
        {
            String retorno = null;

            //TempData["Selecionados"] = selecao;
            //TempData["Desmarcados"] = desmarcados;
            
            List<String> lista = new List<String>();;

            if (selecao != null)
            {
                if(!selecao.Trim().Equals(""))
                {
                    lista = selecao.Split(',').ToList();
                }

                if (contatoMarcado != null)
                {
                    if (!lista.Contains(contatoMarcado))
                    {
                        lista.Add(contatoMarcado);
                    }
                }
                if (contatoDesmarcado != null)
                {
                    if (lista.Contains(contatoDesmarcado))
                    {
                        lista.Remove(contatoDesmarcado);
                    }
                }
            }

            retorno = String.Join(",", lista.ToArray());
            TempData["Selecionados"] = retorno;

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Vincular(String idArray, String remetente)
        {
            String[] ids = idArray.Split(',');
            String retorno = "";

            if (TempData["entidadeID"] == null)
            {
                retorno = "Não existe uma entidade selecionada para vincular contatos.";
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }

            int entidadeID = Convert.ToInt32(TempData["entidadeID"]);
            if (entidadeID == null || entidadeID == 0)
            {
                retorno = "Não foi encontrada uma entidade com o ID " + entidadeID;
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }

            if(remetente.Equals("Entidade"))
            {
                Entidade entidade = db.Entidade.Find(entidadeID);

                foreach (var item in ids)
                {
                    if (!item.Equals(""))
                    {
                        
                        Contato contato = db.Contato.Find(Convert.ToInt32(item));
                        if (contato == null)
                        {
                            retorno = "Não foi encontrado nenhum contato com o ID " + item;
                            return Json(retorno, JsonRequestBehavior.AllowGet);
                        }

                        contato.Entidades.Add(entidade);

                        if (ModelState.IsValid)
                        {
                            db.Entry(contato).State = EntityState.Modified;
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                retorno = "Não foi possível vincular o Contato " + contato.Nome + ": " + ex.Message;
                                return Json(retorno, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }
            else if (remetente.Equals("Empresa"))
            {
                Empresa entidade = db.Empresa.Find(entidadeID);

                foreach (var item in ids)
                {
                    if (!item.Equals(""))
                    {
                        Contato contato = db.Contato.Find(Convert.ToInt32(item));
                        if (contato == null)
                        {
                            retorno = "Não foi encontrado nenhum contato com o ID " + item;
                            return Json(retorno, JsonRequestBehavior.AllowGet);
                        }

                        contato.Empresas.Add(entidade);

                        if (ModelState.IsValid)
                        {
                            db.Entry(contato).State = EntityState.Modified;
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                retorno = "Não foi possível vincular o Contato " + contato.Nome + ": " + ex.Message;
                                return Json(retorno, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }
            
            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Desvincular(String idArray, int? entidadeID, String remetente)
        {

            String[] ids = idArray.Split(',');
            String retorno = "";

            if (entidadeID == null || entidadeID == 0)
            {
                retorno = "Não foi encontrada uma entidade com o ID " + entidadeID;
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }

            if(remetente.Equals("Entidade"))
            {
                Entidade entidade = db.Entidade.Find(entidadeID);

                foreach (var item in ids)
                {
                    if (!item.Equals(""))
                    {
                        Contato contato = db.Contato.Find(Convert.ToInt32(item));
                        if (contato == null)
                        {
                            retorno = "Não foi encontrado nenhum contato com o ID " + item;
                            return Json(retorno, JsonRequestBehavior.AllowGet);
                        }

                        contato.Entidades.Remove(entidade);

                        if (ModelState.IsValid)
                        {
                            db.Entry(contato).State = EntityState.Modified;
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                retorno = "Não foi possível desvincular o Contato " + contato.Nome + ": " + ex.Message;
                                return Json(retorno, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }
            else if (remetente.Equals("Empresa"))
            {
                Empresa entidade = db.Empresa.Find(entidadeID);

                foreach (var item in ids)
                {
                    if (!item.Equals(""))
                    {
                        Contato contato = db.Contato.Find(Convert.ToInt32(item));
                        if (contato == null)
                        {
                            retorno = "Não foi encontrado nenhum contato com o ID " + item;
                            return Json(retorno, JsonRequestBehavior.AllowGet);
                        }

                        contato.Empresas.Remove(entidade);

                        if (ModelState.IsValid)
                        {
                            db.Entry(contato).State = EntityState.Modified;
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                retorno = "Não foi possível desvincular o Contato " + contato.Nome + ": " + ex.Message;
                                return Json(retorno, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }
            
            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        // GET: /Cliente/Edit/5
        [PermissoesFiltro(Roles = "Contato")]
        public ActionResult Entidades(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contato contato = db.Contato.Find(id);
            if (contato == null)
            {
                return HttpNotFound();
            }
            return View(contato);
        }

        // GET: /Contato/Create
        [PermissoesFiltro(Roles = "Contato")]
        public ActionResult Create(int? id)
        {
            ViewBag.EntidadeID = 0;
            ViewBag.DtRegistro = DateTime.Now.ToString("dd/MM/yyyy");

            if (id != null && id > 0)
            {
                ViewBag.EntidadeID = id;
            }
            return View();
        }

        // POST: /Contato/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ContatoID,Nome,Login,Telefone,Email,DtRegistro")] Contato contato, int? id)
        {
            if (ModelState.IsValid)
            {
                Entidade entidade = db.Entidade.Find(id);
                if (entidade != null)
                {
                    contato.Entidades = new List<Entidade>();
                    contato.Entidades.Add(entidade);
                }

                db.Contato.Add(contato);
                Contato cont = db.Contato.Where(p => p.Nome.ToUpper().Equals(contato.Nome.Trim().ToUpper())).FirstOrDefault();
                if (cont != null && cont.ContatoID != contato.ContatoID)
                {
                    ViewBag.Erro = "O serviço " + contato.Nome + " já existe.";
                    return View(contato);
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Não foi possível salvar o Contato " + contato.Nome + ": " + ex.Message;
                    return View(contato);
                }
                if (id != null && id > 0)
                {
                    return RedirectToAction("Contatos", "Entidade", new { id = id });
                }
                return RedirectToAction("Index");
            }

            //ViewBag.EntidadeID = new SelectList(db.Entidade, "EntidadeID", "RazaoSocial", contato.EntidadeID);
            return View(contato);
        }

        // GET: /Contato/Edit/5
        [PermissoesFiltro(Roles = "Contato")]
        public ActionResult Edit(int? id, int? orig)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contato contato = db.Contato.Find(id);
            if (contato == null)
            {
                return HttpNotFound();
            }
            //ViewBag.EntidadeID = new SelectList(db.Entidade, "EntidadeID", "RazaoSocial", contato.EntidadeID);

            if(orig != null && orig > 0){
                ViewData["EntidadeRetorno"] = orig;
                ViewBag.origem = orig;
            }

            return View(contato);
        }

        // POST: /Contato/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ContatoID,Nome,Login,Telefone,Email,DtRegistro,EntidadeID")] Contato contato, int? orig)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contato).State = EntityState.Modified;
                Contato cont = db.Contato.Where(p => p.Nome.ToUpper().Equals(contato.Nome.Trim().ToUpper())).FirstOrDefault();
                if (cont != null && cont.ContatoID != contato.ContatoID)
                {
                    ViewBag.Erro = "O serviço " + contato.Nome + " já existe.";
                    return View(contato);
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Não foi possível salvar o Contato " + contato.Nome + ": " + ex.Message;
                    return View(contato);
                }
                if (orig != null && orig > 0)
                {
                    return RedirectToAction("Selecao", "Contato", new { id = orig });
                }
                return RedirectToAction("Index");
            }
            //ViewBag.EntidadeID = new SelectList(db.Entidade, "EntidadeID", "RazaoSocial", contato.EntidadeID);
            return View(contato);
        }

        // GET: /Contato/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contato contato = db.Contato.Find(id);
            if (contato == null)
            {
                return HttpNotFound();
            }
            return View(contato);
        }

        // POST: /Contato/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contato contato = db.Contato.Find(id);
            db.Entry(contato).Collection("PapelContatos").Load();
            contato.PapelContatos.Clear();
            db.Contato.Remove(contato);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                TempData["Erro"] = "Não foi possível deletar o Contato " + contato.Nome + ". Alguns registros fazem referência a esse Contato.";
                return RedirectToAction("Details", "Contato", new { id = contato.ContatoID });
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Não foi possível deletar o Contato " + contato.Nome + ": " + ex.Message;
                return RedirectToAction("Details", "Contato", new { id = contato.ContatoID });
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
