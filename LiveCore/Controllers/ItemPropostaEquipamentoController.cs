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
using LiveCore.Security;

namespace LiveCore.Controllers
{
    public class ItemPropostaEquipamentoController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /ItemPropostaEquipamento/
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Index(int? id)
        {
            var itempropostaequipamento = db.ItemPropostaEquipamento.Where(i => i.PropostaID == id).Include(i => i.Equipamento).Include(i => i.TipoContrato);
            if(id != null && id > 0)
            {
                Proposta proposta = db.Propostas.Find(Convert.ToInt32(id));
                TempData["propostaID"] = proposta.PropostaID;
                ViewBag.Proposta = proposta.PropostaID;
                ViewBag.IdentProposta = proposta.IdentProposta;
            }
            return View(itempropostaequipamento.ToList());
        }

        // GET: /ItemPropostaEquipamento/Details/5
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemPropostaEquipamento itempropostaequipamento = db.ItemPropostaEquipamento.Find(id);
            if (itempropostaequipamento == null)
            {
                return HttpNotFound();
            }
            if (TempData["Erro"] != null && !TempData["Erro"].ToString().Equals(""))
            {
                ViewBag.Erro = TempData["Erro"];
            }
            if (itempropostaequipamento.PropostaID != null && itempropostaequipamento.PropostaID > 0)
            {
                Proposta proposta = db.Propostas.Find(itempropostaequipamento.PropostaID);
                ViewBag.IdentProposta = proposta.IdentProposta;
            }
            return View(itempropostaequipamento);
        }

        // GET: /ItemPropostaEquipamento/Create
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Create(int? propostaID)
        {
            ViewBag.EquipamentoID = new SelectList(db.Equipamento, "EquipamentoID", "Nome");
            ViewBag.TipoContratoID = new SelectList(db.TipoContrato.Where(p => p.SNEquipamento), "TipoContratoID", "Nome");
            ItemPropostaEquipamento item = new ItemPropostaEquipamento();
            if (Convert.ToInt32(propostaID) != null && Convert.ToInt32(propostaID) > 0)
            {
                item.PropostaID = Convert.ToInt32(propostaID);
                Proposta proposta = db.Propostas.Find(item.PropostaID);
                ViewBag.IdentProposta = proposta.IdentProposta;
                ViewBag.Proposta = proposta.PropostaID;
            }
            return View(item);
        }

        // POST: /ItemPropostaEquipamento/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemPropostaEquipID,TipoContratoID,Quantidade,ValorUnitario,EquipamentoID,PropostaID,AreaEscopoID,Moeda")] ItemPropostaEquipamento itempropostaequipamento)
        {
            if (ModelState.IsValid)
            {
                db.ItemPropostaEquipamento.Add(itempropostaequipamento);
                Proposta proposta = db.Propostas.Find(itempropostaequipamento.PropostaID);
                if(itempropostaequipamento.Moeda != null & itempropostaequipamento.Moeda.Equals("U$"))
                {
                    proposta.ValorTotalDolar += itempropostaequipamento.ValorUnitario * itempropostaequipamento.Quantidade;
                }
                else
                {
                    proposta.ValorTotal += itempropostaequipamento.ValorUnitario * itempropostaequipamento.Quantidade;
                }
                db.Entry(proposta).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Não foi possível salvar o item de equipamento " + itempropostaequipamento.Equipamento.Nome + ": " + ex.Message;
                    return View(itempropostaequipamento);
                }
                return RedirectToAction("ItensProposta", "Proposta", new { id = itempropostaequipamento.PropostaID });
            }

            ViewBag.EquipamentoID = new SelectList(db.Equipamento, "EquipamentoID", "Nome", itempropostaequipamento.EquipamentoID);
            ViewBag.TipoContratoID = new SelectList(db.TipoContrato.Where(p => p.SNEquipamento), "TipoContratoID", "Nome", itempropostaequipamento.TipoContratoID);
            return View(itempropostaequipamento);
        }

        public ActionResult BuscaValores(int id, int tpContratoID, String moeda)
        {
            Equipamento equip = db.Equipamento.Where(p => p.EquipamentoID == id).First();
            TipoContrato tipoContrato = db.TipoContrato.Where(p => p.TipoContratoID == tpContratoID).First();

            Decimal valor = 0;

            if (tipoContrato.Nome.Equals("Venda"))
            {
                if (moeda.Trim().Equals("U$"))
                {
                    valor = equip.PrecoVendaDolar;
                }
                if (moeda.Trim().Equals("R$"))
                {
                    valor = equip.PrecoVenda;
                }
            }
            else
            {
                if (moeda.Trim().Equals("U$"))
                {
                    valor = equip.ValorAluguelDolar;
                }
                if (moeda.Trim().Equals("R$"))
                {
                    valor = equip.ValorAluguel;
                }
            }

            if (valor == 0)
            {
                return Json(valor, JsonRequestBehavior.AllowGet);
            }

            return Json(valor.ToString().Replace(".",",").Substring(0, valor.ToString().Length -2), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CalculaValorTotal(String valorUnitario, String quantidade)
        {
            Decimal valorTotal = Convert.ToDecimal(valorUnitario) * Convert.ToInt32(quantidade);

            return Json(valorTotal.ToString().Replace(".", ","), JsonRequestBehavior.AllowGet);
        }

        // GET: /ItemPropostaEquipamento/Edit/5
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemPropostaEquipamento itempropostaequipamento = db.ItemPropostaEquipamento.Find(id);
            if (itempropostaequipamento == null)
            {
                return HttpNotFound();
            }
            ViewBag.EquipamentoID = new SelectList(db.Equipamento, "EquipamentoID", "Nome", itempropostaequipamento.EquipamentoID);
            ViewBag.TipoContratoID = new SelectList(db.TipoContrato.Where(p => p.SNEquipamento), "TipoContratoID", "Nome", itempropostaequipamento.TipoContratoID);
            if (itempropostaequipamento.PropostaID != null && itempropostaequipamento.PropostaID > 0)
            {
                Proposta proposta = db.Propostas.Find(itempropostaequipamento.PropostaID);
                ViewBag.IdentProposta = proposta.IdentProposta;
            }
            return View(itempropostaequipamento);
        }

        // POST: /ItemPropostaEquipamento/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemPropostaEquipID,TipoContratoID,Quantidade,ValorUnitario,EquipamentoID,PropostaID,AreaEscopoID,Moeda")] ItemPropostaEquipamento itempropostaequipamento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itempropostaequipamento).State = EntityState.Modified;
                List<ItemPropostaEquipamento> equips = db.ItemPropostaEquipamento.Where(p => p.PropostaID == itempropostaequipamento.PropostaID).ToList();
                List<ItemPropostaServico> serv = db.ItemPropostaServico.Where(p => p.PropostaID == itempropostaequipamento.PropostaID).ToList();
                Decimal valorTotal = 0;
                Decimal valorTotalDolar = 0;
                Proposta proposta = db.Propostas.Find(itempropostaequipamento.PropostaID);
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
                    ViewBag.Erro = "Não foi possível salvar o item de equipamento " + itempropostaequipamento.Equipamento.Nome + ": " + ex.Message;
                    return View(itempropostaequipamento);
                }
                return RedirectToAction("ItensProposta", "Proposta", new { id = itempropostaequipamento.PropostaID });
            }
            ViewBag.EquipamentoID = new SelectList(db.Equipamento, "EquipamentoID", "Nome", itempropostaequipamento.EquipamentoID);
            ViewBag.TipoContratoID = new SelectList(db.TipoContrato.Where(p => p.SNEquipamento), "TipoContratoID", "Nome", itempropostaequipamento.TipoContratoID);
            return View(itempropostaequipamento);
        }

        // GET: /ItemPropostaEquipamento/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemPropostaEquipamento itempropostaequipamento = db.ItemPropostaEquipamento.Find(id);
            if (itempropostaequipamento == null)
            {
                return HttpNotFound();
            }
            return View(itempropostaequipamento);
        }

        // POST: /ItemPropostaEquipamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ItemPropostaEquipamento itempropostaequipamento = db.ItemPropostaEquipamento.Find(id);
            Proposta proposta = db.Propostas.Find(itempropostaequipamento.PropostaID);
            if (itempropostaequipamento.Moeda.Equals("U$"))
            {
                proposta.ValorTotalDolar -= itempropostaequipamento.ValorUnitario * itempropostaequipamento.Quantidade;
            }
            else
            {
                proposta.ValorTotal -= itempropostaequipamento.ValorUnitario * itempropostaequipamento.Quantidade;
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
            db.ItemPropostaEquipamento.Remove(itempropostaequipamento);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Não foi possível deletar o item de equipamento " + itempropostaequipamento.Equipamento.Nome + ": " + ex.Message;
                return RedirectToAction("Details", "ItemPropostaEquipamento", new { id = itempropostaequipamento.ItemPropostaEquipID });
            }
            return RedirectToAction("ItensProposta", "Proposta", new { id = itempropostaequipamento.PropostaID });
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
