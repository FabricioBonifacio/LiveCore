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
    public class EmailConfigController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /EmailConfig/
        [PermissoesFiltro(Roles = "Configuração de E-mail")]
        public ActionResult Index()
        {
            if (db.EmailConfigs.ToList().Count() == 0)
            {
                return RedirectToAction("Create");
            }
            else
            {
                EmailConfig emailConfig = db.EmailConfigs.ToList().FirstOrDefault();
                return RedirectToAction("Edit", new { id = emailConfig.EmailConfigID });
            }
            // return View();
        }

        // GET: /EmailConfig/Details/5
        [PermissoesFiltro(Roles = "Configuração de E-mail")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailConfig emailconfig = db.EmailConfigs.Find(id);
            if (emailconfig == null)
            {
                return HttpNotFound();
            }
            return View(emailconfig);
        }

        // GET: /EmailConfig/Create
        [PermissoesFiltro(Roles = "Configuração de E-mail")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /EmailConfig/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="EmailConfigID,Host,Porta,Login,Senha,EmailFrom,EmailDefaultTo")] EmailConfig emailconfig)
        {
            if (ModelState.IsValid)
            {
                db.EmailConfigs.Add(emailconfig);
                try
                {
                    db.SaveChanges();
                    TempData["Msg"] = "A configuração de envio de e-mail foi salva com sucesso.";
                    int id = db.EmailConfigs.FirstOrDefault().EmailConfigID;
                    return RedirectToAction("Edit", new { id = id });
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Não foi possível editar a configuração de envio de e-mail.";
                }
            }

            return View(emailconfig);
        }

        // GET: /EmailConfig/Edit/5
        [PermissoesFiltro(Roles = "Configuração de E-mail")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailConfig emailconfig = db.EmailConfigs.Find(id);
            if (emailconfig == null)
            {
                return HttpNotFound();
            }
            if (TempData["Msg"] != null && !TempData["Msg"].ToString().Equals(""))
            {
                ViewBag.Msg = TempData["Msg"];
            }
            return View(emailconfig);
        }

        // POST: /EmailConfig/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="EmailConfigID,Host,Porta,Login,Senha,EmailFrom,EmailDefaultTo")] EmailConfig emailconfig)
        {
            if (ModelState.IsValid)
            {
                //if (emailconfig.Senha == null || emailconfig.Senha.Trim().Equals(""))
                //{
                //    EmailConfig emailconfigEdit = db.EmailConfigs.Find(emailconfig.EmailConfigID);
                //    emailconfig.Senha = emailconfigEdit.Senha;
                //}
                
                //if (emailconfig == null)
                //{
                //    return HttpNotFound();
                //}
                db.Entry(emailconfig).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    ViewBag.Msg = "A configuração de envio de e-mail foi salva com sucesso.";
                }
                catch(Exception ex){
                    ViewBag.Erro = "Não foi possível editar a configuração de envio de e-mail.";
                }
                return RedirectToAction("Index");
            }
            return View(emailconfig);
        }

        // GET: /EmailConfig/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailConfig emailconfig = db.EmailConfigs.Find(id);
            if (emailconfig == null)
            {
                return HttpNotFound();
            }
            return View(emailconfig);
        }

        // POST: /EmailConfig/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmailConfig emailconfig = db.EmailConfigs.Find(id);
            db.EmailConfigs.Remove(emailconfig);
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
