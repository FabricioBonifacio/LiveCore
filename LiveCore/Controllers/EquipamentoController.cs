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
    public class EquipamentoController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /Equipamento/
        [PermissoesFiltro(Roles = "Equipamento")]
        public ActionResult Index(string ordem, string currentFilter, string nomeSearch, int? page)
        {
            ViewBag.CurrentSort = ordem;
            ViewBag.NomeSortParm = String.IsNullOrEmpty(ordem) ? "Equipamento_desc" : "";
            ViewBag.MarcaSortParm = ordem == "Fabricante" ? "Fabricante_desc" : "Fabricante";
            ViewBag.ReferenciaSortParm = ordem == "Referência" ? "Referência_desc" : "Referência";
            ViewBag.VendaSortParm = ordem == "Preço de Venda" ? "Preço de Venda_desc" : "Preço de Venda";
            ViewBag.AluguelSortParm = ordem == "Valor do Aluguel" ? "Valor do Aluguel_desc" : "Valor do Aluguel";

            if (nomeSearch != null)
            {
                page = 1;
            }
            else
            {
                nomeSearch = currentFilter;
            }

            ViewBag.CurrentFilter = nomeSearch;

            var equipamento = from s in db.Equipamento
                               select s;

            if (!String.IsNullOrEmpty(nomeSearch))
            {
                equipamento = equipamento.Where(s => s.Nome.ToUpper().Contains(nomeSearch.ToUpper()));
            }

            switch (ordem)
            {
                case "Fabricante_desc":
                    equipamento = equipamento.OrderByDescending(s => s.Marca);
                    break;
                case "Fabricante":
                    equipamento = equipamento.OrderBy(s => s.Marca);
                    break;
                case "Referência_desc":
                    equipamento = equipamento.OrderByDescending(s => s.Referencia);
                    break;
                case "Referência":
                    equipamento = equipamento.OrderBy(s => s.Referencia);
                    break;
                case "Preço de Venda_desc":
                    equipamento = equipamento.OrderByDescending(s => s.PrecoVenda);
                    break;
                case "Preço de Venda":
                    equipamento = equipamento.OrderBy(s => s.PrecoVenda);
                    break;
                case "Valor do Aluguel_desc":
                    equipamento = equipamento.OrderByDescending(s => s.ValorAluguel);
                    break;
                case "Valor de Aluguel":
                    equipamento = equipamento.OrderBy(s => s.ValorAluguel);
                    break;
                case "Equipamento_desc":
                    equipamento = equipamento.OrderByDescending(s => s.Nome);
                    break;
                default:
                    equipamento = equipamento.OrderBy(s => s.Nome);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(equipamento.ToPagedList(pageNumber, pageSize));
        }

        // GET: /Equipamento/Details/5
        [PermissoesFiltro(Roles = "Equipamento")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipamento equipamento = db.Equipamento.Find(id);
            if (equipamento == null)
            {
                return HttpNotFound();
            }
            if (TempData["Erro"] != null && !TempData["Erro"].ToString().Equals(""))
            {
                ViewBag.Erro = TempData["Erro"];
            }
            return View(equipamento);
        }

        public JsonResult VerificaPartNumber(String referencia, int equipamentoID)
        {
            
            Equipamento equipamento = db.Equipamento.Where(p => p.Referencia.ToUpper().Trim().Equals(referencia.ToUpper().Trim())).FirstOrDefault();

            var result = "";

            if(equipamento != null){
                if (equipamentoID == 0 || equipamentoID != equipamento.EquipamentoID)
                {
                    result = "Já existe um equipamento com esse Part Number.";
                }
            }
                        
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AutoCompleteEquipamento(String term)
        {
            var result = (from r in db.Equipamento
                          where r.Nome.ToLower().Contains(term.ToLower())
                          select new { r.Nome }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperaEquipamentoPorNome(String equipamento)
        {
            Equipamento ae = db.Equipamento.Where(p => p.Nome.ToUpper().Equals(equipamento.ToUpper())).FirstOrDefault();

            var result = 0;

            if (ae != null)
            {
                result = ae.EquipamentoID;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperaEquipamentoPorId(int equipamentoID)
        {
            Equipamento tc = db.Equipamento.Find(equipamentoID);

            var result = tc.Nome;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CriarEquipamento(String equipamentoNovo)
        {
            Equipamento equipamento = new Equipamento();

            equipamento.Nome = equipamentoNovo;

            var result = 0;

            try
            {
                db.Equipamento.Add(equipamento);
                db.SaveChanges();
                result = (db.Equipamento.Where(p => p.Nome.ToUpper().Equals(equipamentoNovo.ToUpper())).FirstOrDefault()).EquipamentoID;
            }
            catch (Exception ex)
            {
                //result = "Erro ao salvar Tipo de Contrato: " + ex.Message;   
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditEquipamentoModal(int equipamentoID, String nome, String descricao, String ncm, String unidade,
            String marca, String referencia, String precoVenda, String precoVendaDolar, String precoCusto, String precoCustoDolar,
            String valorAluguel, String valorAluguelDolar)
        {
            String retorno = "";
            Equipamento equipamento = new Equipamento();

            if (ncm.Trim().Equals(""))
            {
                ncm = "0";
            }

            if(unidade.Trim().Equals(""))
            {
                unidade = null;
            }

            equipamento.EquipamentoID = equipamentoID;
            equipamento.Nome = nome;
            equipamento.Descricao = descricao;
            equipamento.NCM = Convert.ToInt32(ncm);
            equipamento.UnidadeSigla = unidade;
            equipamento.Marca = marca;
            equipamento.Referencia = referencia;
            equipamento.ValorAluguel = Convert.ToDecimal(valorAluguel.Replace(".", ""));
            equipamento.ValorAluguelDolar = Convert.ToDecimal(valorAluguelDolar.Replace(".", ""));
            equipamento.PrecoCusto = Convert.ToDecimal(precoCusto.Replace(".", ""));
            equipamento.PrecoCustoDolar = Convert.ToDecimal(precoCustoDolar.Replace(".", ""));
            equipamento.PrecoVenda = Convert.ToDecimal(precoVenda.Replace(".", ""));
            equipamento.PrecoVendaDolar = Convert.ToDecimal(precoVendaDolar.Replace(".", ""));

            db.Entry(equipamento).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                retorno = "Equipamento editado com sucesso.";
            }
            catch (DbUpdateException ex)
            {
                retorno = ex.Message;
            }
            catch (Exception ex)
            {
                retorno = "Não foi possível salvar o equipamento " + equipamento.Nome + ": " + ex.Message;
            }

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        // GET: /Equipamento/Create
        [PermissoesFiltro(Roles = "Equipamento")]
        public ActionResult Create()
        {
            ViewBag.UnidadeSigla = new SelectList(db.Unidade, "UnidadeSigla", "Nome");
            return View();
        }

        // POST: /Equipamento/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EquipamentoID,Nome,Descricao,CodPatrimonio,UnidadeSigla,Marca,Referencia,NCM,PrecoCusto,PrecoVenda,ValorAluguel,DtVencGarantia,PrecoCustoDolar,PrecoVendaDolar,ValorAluguelDolar")] Equipamento equipamento)
        {
            if (ModelState.IsValid)
            {
                db.Equipamento.Add(equipamento);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Não foi possível salvar o Equipamento " + equipamento.Nome + "\n" + ex.Message;
                    return View(equipamento);
                }
                return RedirectToAction("Index");
            }

            ViewBag.UnidadeSigla = new SelectList(db.Unidade, "UnidadeSigla", "Nome", equipamento.UnidadeSigla);
            return View(equipamento);
        }

        // GET: /Equipamento/Edit/5
        [PermissoesFiltro(Roles = "Equipamento")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipamento equipamento = db.Equipamento.Find(id);
            if (equipamento == null)
            {
                return HttpNotFound();
            }
            ViewBag.UnidadeSigla = new SelectList(db.Unidade, "UnidadeSigla", "Nome", equipamento.UnidadeSigla);
            return View(equipamento);
        }

        // GET: /Servico/Edit/5
        [PermissoesFiltro(Roles = "Equipamento")]
        public ActionResult EditPopUp(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipamento equipamento = db.Equipamento.Find(id);
            if (equipamento == null)
            {
                return HttpNotFound();
            }
            ViewBag.UnidadeSigla = new SelectList(db.Unidade, "UnidadeSigla", "Nome", equipamento.UnidadeSigla);
            return View(equipamento);
        }

        // POST: /Equipamento/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EquipamentoID,Nome,Descricao,CodPatrimonio,UnidadeSigla,Marca,Referencia,NCM,PrecoCusto,PrecoVenda,ValorAluguel,DtVencGarantia,PrecoCustoDolar,PrecoVendaDolar,ValorAluguelDolar")] Equipamento equipamento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(equipamento).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Não foi possível salvar o Equipamento " + equipamento.Nome + "\n" + ex.Message;
                    return View(equipamento);
                }
                return RedirectToAction("Index");
            }
            ViewBag.UnidadeSigla = new SelectList(db.Unidade, "UnidadeSigla", "Nome", equipamento.UnidadeSigla);
            return View(equipamento);
        }

        // GET: /Equipamento/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipamento equipamento = db.Equipamento.Find(id);
            if (equipamento == null)
            {
                return HttpNotFound();
            }
            return View(equipamento);
        }

        // POST: /Equipamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Equipamento equipamento = db.Equipamento.Find(id);
            db.Equipamento.Remove(equipamento);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                TempData["Erro"] = "Não foi possível deletar o Equipamento " + equipamento.Nome + ". Alguns registros fazem referência a esse Equipamento.";
                return RedirectToAction("Details", "Equipamento", new { id = equipamento.EquipamentoID });
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Não foi possível deletar a Equipamento " + equipamento.Nome + ": " + ex.Message;
                return RedirectToAction("Details", "Equipamento", new { id = equipamento.EquipamentoID });
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
