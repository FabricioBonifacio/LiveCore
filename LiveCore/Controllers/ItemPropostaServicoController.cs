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
    public class ItemPropostaServicoController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /ItemPropostaServico/
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Index(int? id)
        {
            var itempropostaservico = db.ItemPropostaServico.Where(i => i.PropostaID == id).Include(i => i.AreaEscopo).Include(i => i.Servico).Include(i => i.TipoContrato);
            if (id != null && id > 0)
            {
                Proposta proposta = db.Propostas.Find(Convert.ToInt32(id));
                TempData["propostaID"] = proposta.PropostaID;
                ViewBag.Proposta = proposta.PropostaID;
                ViewBag.IdentProposta = proposta.IdentProposta;
            }
            return View(itempropostaservico.ToList());
        }
        
        // GET: /ItemPropostaServico/Details/5
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemPropostaServico itempropostaservico = db.ItemPropostaServico.Find(id);
            if (itempropostaservico == null)
            {
                return HttpNotFound();
            }
            if (TempData["Erro"] != null && !TempData["Erro"].ToString().Equals(""))
            {
                ViewBag.Erro = TempData["Erro"];
            }
            if (itempropostaservico.PropostaID != null && itempropostaservico.PropostaID > 0)
            {
                Proposta proposta = db.Propostas.Find(itempropostaservico.PropostaID);
                ViewBag.IdentProposta = proposta.IdentProposta;
            }
            return View(itempropostaservico);
        }

        // GET: /ItemPropostaServico/Create
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Create(int? propostaID)
        {
            ViewBag.AreaEscopoID = new SelectList(db.AreaEscopo, "AreaEscopoID", "Nome");
            ViewBag.ServicoID = new SelectList(db.Servico, "ServicoID", "Nome");
            ViewBag.TipoContratoID = new SelectList(db.TipoContrato.Where(p => p.SNServico), "TipoContratoID", "Nome");
            ItemPropostaServico item = new ItemPropostaServico();
            if (Convert.ToInt32(propostaID) != null && Convert.ToInt32(propostaID) > 0)
            {
                item.PropostaID = Convert.ToInt32(propostaID);
                Proposta proposta = db.Propostas.Find(item.PropostaID);
                ViewBag.IdentProposta = proposta.IdentProposta;
                ViewBag.Proposta = proposta.PropostaID;
            }
            return View(item);
        }

        // POST: /ItemPropostaServico/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ItemPropostaServID,TipoContratoID,AreaEscopoID,Quantidade,ValorUnitario,ServicoID,PropostaID,Moeda")] ItemPropostaServico itempropostaservico)
        {
            ViewBag.AreaEscopoID = new SelectList(db.AreaEscopo, "AreaEscopoID", "Nome", itempropostaservico.AreaEscopoID);
            ViewBag.ServicoID = new SelectList(db.Servico, "ServicoID", "Nome", itempropostaservico.ServicoID);
            ViewBag.TipoContratoID = new SelectList(db.TipoContrato.Where(p => p.SNServico), "TipoContratoID", "Nome", itempropostaservico.TipoContratoID);

            if (ModelState.IsValid)
            {
                db.ItemPropostaServico.Add(itempropostaservico);
                Proposta proposta = db.Propostas.Find(itempropostaservico.PropostaID);
                if (itempropostaservico.Moeda != null & itempropostaservico.Moeda.Equals("U$"))
                {
                    proposta.ValorTotalDolar += itempropostaservico.ValorUnitario * itempropostaservico.Quantidade;
                }
                else
                {
                    proposta.ValorTotal += itempropostaservico.ValorUnitario * itempropostaservico.Quantidade;
                }
                db.Entry(proposta).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Não foi possível salvar o item de serviço " + itempropostaservico.Servico.Nome + ": " + ex.Message;
                    return View(itempropostaservico);
                }
                return RedirectToAction("ItensProposta", "Proposta", new { id = itempropostaservico.PropostaID });
            }

            return View(itempropostaservico);
        }

        public ActionResult BuscaValores(int id, String moeda)
        {
            Servico servico = db.Servico.Where(p => p.ServicoID == id).First();

            Decimal valor = 0;

            if (moeda.Trim().Equals("U$"))
            {
                valor = servico.ValorDolar;
            }
            if (moeda.Trim().Equals("R$"))
            {
                valor = servico.Valor;
            }

            if(valor == 0){
                return Json(valor, JsonRequestBehavior.AllowGet);
            }

            return Json(valor.ToString().Replace(".", ",").Substring(0, valor.ToString().Length - 2), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CalculaValorTotal(String valorUnitario, String quantidade)
        {
            Decimal valorTotal = Convert.ToDecimal(valorUnitario) * Convert.ToInt32(quantidade);

            return Json(valorTotal.ToString().Replace(".", ","), JsonRequestBehavior.AllowGet);
        }

        // GET: /ItemPropostaServico/Edit/5
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemPropostaServico itempropostaservico = db.ItemPropostaServico.Find(id);
            if (itempropostaservico == null)
            {
                return HttpNotFound();
            }
            ViewBag.AreaEscopoID = new SelectList(db.AreaEscopo, "AreaEscopoID", "Nome", itempropostaservico.AreaEscopoID);
            ViewBag.ServicoID = new SelectList(db.Servico, "ServicoID", "Nome", itempropostaservico.ServicoID);
            ViewBag.TipoContratoID = new SelectList(db.TipoContrato, "TipoContratoID", "Nome", itempropostaservico.TipoContratoID);
            if (itempropostaservico.PropostaID != null && itempropostaservico.PropostaID > 0)
            {
                Proposta proposta = db.Propostas.Find(itempropostaservico.PropostaID);
                ViewBag.IdentProposta = proposta.IdentProposta;
            }
            return View(itempropostaservico);
        }

        // POST: /ItemPropostaServico/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemPropostaServID,TipoContratoID,AreaEscopoID,Quantidade,ValorUnitario,ServicoID,PropostaID,Moeda")] ItemPropostaServico itempropostaservico)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itempropostaservico).State = EntityState.Modified;
                List<ItemPropostaEquipamento> equips = db.ItemPropostaEquipamento.Where(p => p.PropostaID == itempropostaservico.PropostaID).ToList();
                List<ItemPropostaServico> serv = db.ItemPropostaServico.Where(p => p.PropostaID == itempropostaservico.PropostaID).ToList();
                Decimal valorTotal = 0;
                Decimal valorTotalDolar = 0;
                Proposta proposta = db.Propostas.Find(itempropostaservico.PropostaID);
                foreach (var item in equips)
                {
                    if (item.Moeda != null & item.Moeda.Equals("U$"))
                    {
                        valorTotalDolar += item.Quantidade * item.ValorUnitario;
                    }
                    else
                    {
                        valorTotal += item.Quantidade * item.ValorUnitario;
                    }
                }
                foreach (var item in serv)
                {
                    if (item.Moeda != null & item.Moeda.Equals("U$"))
                    {
                        valorTotalDolar += item.Quantidade * item.ValorUnitario;
                    }
                    else
                    {
                        valorTotal += item.Quantidade * item.ValorUnitario;
                    }
                }
                proposta.ValorTotal = valorTotal;
                proposta.ValorTotalDolar = valorTotalDolar;
                db.Entry(proposta).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Não foi possível salvar o item de serviço " + itempropostaservico.Servico.Nome + ": " + ex.Message;
                    return View(itempropostaservico);
                }
                return RedirectToAction("ItensProposta", "Proposta", new { id = itempropostaservico.PropostaID });
            }
            ViewBag.AreaEscopoID = new SelectList(db.AreaEscopo, "AreaEscopoID", "Nome", itempropostaservico.AreaEscopoID);
            ViewBag.ServicoID = new SelectList(db.Servico, "ServicoID", "Nome", itempropostaservico.ServicoID);
            ViewBag.TipoContratoID = new SelectList(db.TipoContrato, "TipoContratoID", "Nome", itempropostaservico.TipoContratoID);
            return View(itempropostaservico);
        }

        // GET: /ItemPropostaServico/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemPropostaServico itempropostaservico = db.ItemPropostaServico.Find(id);
            if (itempropostaservico == null)
            {
                return HttpNotFound();
            }
            return View(itempropostaservico);
        }

        // POST: /ItemPropostaServico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ItemPropostaServico itempropostaservico = db.ItemPropostaServico.Find(id);
            Proposta proposta = db.Propostas.Find(itempropostaservico.PropostaID);
            if (itempropostaservico.Moeda.Equals("U$"))
            {
                proposta.ValorTotalDolar -= itempropostaservico.ValorUnitario * itempropostaservico.Quantidade;
            }
            else
            {
                proposta.ValorTotal -= itempropostaservico.ValorUnitario * itempropostaservico.Quantidade;
            }
            if (proposta.ValorTotal < 0)
            {
                proposta.ValorTotal = 0;
            }
            if (proposta.ValorTotalDolar < 0)
            {
                proposta.ValorTotalDolar = 0;
            }
            db.Entry(proposta).State = EntityState.Modified;
            db.ItemPropostaServico.Remove(itempropostaservico);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Não foi possível deletar o item de serviço " + itempropostaservico.Servico.Nome + ": " + ex.Message;
                return RedirectToAction("Details", "ItemPropostaServico", new { id = itempropostaservico.ItemPropostaServID});
            }
            return RedirectToAction("ItensProposta", "Proposta", new { id = itempropostaservico.PropostaID });
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
