using LiveCore.DAL;
using LiveCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiveCore.Controllers
{
    public class HomeController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        public ActionResult Index()
        {
            String status = "";
            String quantidade = "";
            String valores = "";
            int cont = 0;
            Decimal? valor = 0;

            foreach (Status item in db.Status.ToList())
            {
                cont = db.Propostas.Where(p => p.StatusSigla.Equals(item.StatusSigla)).Count();
                if (cont > 0)
                {
                    valor = db.Propostas.Where(p => p.StatusSigla.Equals(item.StatusSigla)).Sum(p => p.ValorTotal);
                }
                quantidade += cont + ",";
                status += item.Nome + ",";
                valores += valor + " ";
                cont = 0;
                valor = 0;
            }

            ViewBag.EstagioProposta = status;
            ViewBag.Quantidade = quantidade;
            ViewBag.Valores = valores.Replace(",",".");

            if (TempData["Erro"] != null && !TempData["Erro"].ToString().Equals(""))
            {
                ViewBag.Erro = TempData["Erro"];
            }


            return View();
        }

        public ActionResult About()
        {
            //ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult AcessoNegado()
        {
            ViewBag.Message = "Acesso Negado";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}