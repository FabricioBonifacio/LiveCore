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
using LiveCore.Repositories;
using LiveCore.Security;
using System.Security.Cryptography;
using System.Text;

namespace LiveCore.Controllers
{
    public class UsuarioController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /Usuario/
        [PermissoesFiltro(Roles="Gerente")]
        public ActionResult Index()
        {
            return View(db.Usuario.ToList());
        }

        // GET: /Usuario/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: /Usuario/Edit/5
        [PermissoesFiltro(Roles = "Contato")]
        public ActionResult UsuarioPermissao(String login)
        {
            if (login == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Where(p => p.Login.Trim().ToUpper().Equals(login.ToUpper())).FirstOrDefault();
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: /Usuario/Create
        [PermissoesFiltro(Roles = "Contato")]
        public ActionResult Create(int? id, String nome)
        {
            ViewBag.Nome = nome;
            ViewBag.Contato = id;
            return View();
        }

        // POST: /Usuario/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UsuarioID,Login,Senha,ConfirmSenha")] Usuario usuario, int? id)
        {
            int contatoID = 0;
            if (id != null && id > 0)
            {
                contatoID = Convert.ToInt32(id);
            }
            String erro = "";
            String msg = "";
            if (ModelState.IsValid)
            {
                Usuario usuarioExiste = db.Usuario.Where(p => p.Login == usuario.Login).FirstOrDefault();

                if(usuarioExiste != null){
                    erro = "Esse usuário já existe.";
                    return RedirectToAction("Details", "Contato", new { id = contatoID, erroMsg = erro });
                }

                MD5 md5Hash = MD5.Create();
                usuario.Senha = this.Encriptar(md5Hash, usuario.Senha);
                usuario.ConfirmSenha = usuario.Senha;

                db.Usuario.Add(usuario);
                try
                {
                    db.SaveChanges();

                    if (contatoID > 0)
                    {
                        Contato contato = db.Contato.Find(contatoID);
                        contato.Login = usuario.Login;
                        db.Entry(contato).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    erro = "Não foi possível salvar o usuário " + usuario.Login + ": " + ex.Message;
                    return RedirectToAction("Details", "Contato", new { id = contatoID, erroMsg = erro });
                }
                msg = "Usuário salvo com sucesso.";
                return RedirectToAction("Details", "Contato", new { id = contatoID, msg = msg });
            }
            erro = "Não foi possível salvar o usuário " + usuario.Login;
            return RedirectToAction("Details", "Contato", new { id = contatoID }); ;
        }

        public String Encriptar(MD5 md5Hash, string input)
        {   
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public bool VerificarMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = Encriptar(md5Hash, input);
            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // GET: /Usuario/Create
        public ActionResult Logar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            UsuarioRepositorio.Deslogar();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logar([Bind(Include = "UsuarioID,Login,Senha,ConfirmSenha")] Usuario usuario)
        {
            if(UsuarioRepositorio.AutenticarUsuario(usuario.Login, usuario.Senha) == false)
            {
                TempData["Erro"] = "Nome ou senha Inválida";
                ModelState.AddModelError("", "Invalid username or password.");
                return RedirectToAction("Index", "Home");
                
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: /Usuario/Edit/5
        [PermissoesFiltro(Roles = "Contato")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: /Usuario/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UsuarioID,Login,Senha,ConfirmSenha")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        // GET: /Usuario/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: /Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuario.Find(id);
            db.Usuario.Remove(usuario);
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
