using LiveCore.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Web.Security;
using LiveCore.Models;
using LiveCore.Controllers;
using System.Security.Cryptography;

namespace LiveCore.Repositories
{
    public class UsuarioRepositorio
    {
        
        public static bool AutenticarUsuario (String login, String senha)
        {
            LiveCoreContext db = new LiveCoreContext();

            Usuario usuario = db.Usuario.Where(p => p.Login == login).SingleOrDefault();

            if(usuario == null)
            {
                return false;
            }

            UsuarioController usuCon = new UsuarioController();
            MD5 md5Hash = MD5.Create();
            
            if(!usuCon.VerificarMd5Hash(md5Hash, senha, usuario.Senha)){
                return false;
            }

            //Seta um cookie encriptado com o login do usuario autenticado
            FormsAuthentication.SetAuthCookie(usuario.Login, false);

            return true;
        }

        public static Usuario GetUsuarioLogado()
        {
            //Pega o login do usuário logado
            String login = HttpContext.Current.User.Identity.Name;

            //se nao existe usuario logado
            if(login == "")
            {
                return null;
            }
            else
            {
                LiveCoreContext db = new LiveCoreContext();

                Usuario usuario = db.Usuario.Where(p => p.Login == login).SingleOrDefault();

                return usuario;
            }
        }

        public static void Deslogar()
        {
            FormsAuthentication.SignOut();
        }

        public static bool ExistePermissao(string roleName)
        {
            Usuario usuario = GetUsuarioLogado();

            foreach (var item in usuario.Permissoes)
            {
                if (item.Nome.Equals(roleName))
                {
                    return true;
                }
            }

            return false;
        }


    }
}