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
using System.IO;
using PagedList;
using System.Data.Entity.Infrastructure;
using Microsoft.Reporting.WebForms;
using System.ComponentModel;
using System.Data.SqlClient;
using LiveCore.Security;
using System.Net.Mail;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Configuration;
using System.Web.UI;
using System.Diagnostics;

namespace LiveCore.Controllers
{
    public class PropostaController : Controller
    {
        private LiveCoreContext db = new LiveCoreContext();

        // GET: /Proposta/
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Index(string ordem, string currentFilter, string currentFilterComercial, string currentFilterDtValidadeInicio, string currentFilterDtValidadeFinal, string currentFilterEstagio,
            string nomeSearch, string comercialSearch, string dtValidadeInicioSearch, string dtValidadeFinalSearch, string estagioSearch, int? page)
        {
            IEnumerable<String> status = db.Status.Select(p => p.Nome).ToList();
            ViewBag.Status = from action in status
                          select new SelectListItem
                          {
                              Text = action.ToString(),
                              Value = action.ToString()
                          };

            ViewBag.CurrentSort = ordem;
            ViewBag.NomeSortParm = String.IsNullOrEmpty(ordem) ? "Entidade_desc" : "";
            ViewBag.VersaoSortParm = ordem == "Versão" ? "Versão_desc" : "Versão";
            ViewBag.ComercialSortParm = ordem == "Comercial" ? "Comercial_desc" : "Comercial";
            ViewBag.StatusSortParm = ordem == "Estágio da Proposta" ? "Estágio da Proposta_desc" : "Estágio da Proposta";
            ViewBag.ValorTotalSortParm = ordem == "Valor Total" ? "Valor Total_desc" : "Valor Total";
            ViewBag.ValidadeSortParm = ordem == "Validade" ? "Validade_desc" : "Validade";

            if (!String.IsNullOrEmpty(nomeSearch))
            {
                page = 1;
            }
            else
            {
                nomeSearch = currentFilter;
            }

            if (!String.IsNullOrEmpty(comercialSearch))
            {
                page = 1;
            }
            else
            {
                comercialSearch = currentFilterComercial;
            }

            if (!String.IsNullOrEmpty(dtValidadeInicioSearch))
            {
                page = 1;
            }
            else
            {
                dtValidadeInicioSearch = currentFilterDtValidadeInicio;
            }

            if (!String.IsNullOrEmpty(dtValidadeFinalSearch))
            {
                page = 1;
            }
            else
            {
                dtValidadeFinalSearch = currentFilterDtValidadeFinal;
            }
            if (!String.IsNullOrEmpty(estagioSearch))
            {
                page = 1;
            }
            else
            {
                estagioSearch = currentFilterEstagio;
            }

            ViewBag.CurrentFilterNome = nomeSearch;
            ViewBag.CurrentFilterLogin = comercialSearch;
            ViewBag.CurrentFilterDtRegistroInicio = dtValidadeInicioSearch;
            ViewBag.CurrentFilterDtRegistroFinal = dtValidadeFinalSearch;
            ViewBag.CurrentFilterEstagio = estagioSearch;

            var proposta = from s in db.Propostas
                          select s;

            if (!String.IsNullOrEmpty(nomeSearch))
            {
                proposta = proposta.Where(s => s.Entidade.Nome.ToUpper().Contains(nomeSearch.ToUpper()));
            }
            if (!String.IsNullOrEmpty(comercialSearch))
            {
                proposta = proposta.Where(s => s.ContatoLive.Nome.ToUpper().Contains(comercialSearch.ToUpper()));
            }
            if (!String.IsNullOrEmpty(dtValidadeInicioSearch) && !String.IsNullOrEmpty(dtValidadeFinalSearch))
            {
                DateTime dataInicio = Convert.ToDateTime(dtValidadeInicioSearch);
                DateTime dataFinal = Convert.ToDateTime(dtValidadeFinalSearch);
                proposta = proposta.Where(s => s.Validade >= dataInicio).Where(s => s.Validade <= dataFinal);
            }
            if (!String.IsNullOrEmpty(estagioSearch))
            {
                proposta = proposta.Where(s => s.Status.Nome.ToUpper().Contains(estagioSearch.ToUpper()));
            }

            switch (ordem)
            {
                case "Entidade_desc":
                    proposta = proposta.OrderByDescending(s => s.Entidade.Nome);
                    break;
                case "Comercial_desc":
                    proposta = proposta.OrderByDescending(s => s.ContatoLive.Nome);
                    break;
                case "Comercial":
                    proposta = proposta.OrderBy(s => s.ContatoLive.Nome);
                    break;
                case "Versão_desc":
                    proposta = proposta.OrderByDescending(s => s.Versao);
                    break;
                case "Versão":
                    proposta = proposta.OrderBy(s => s.Versao);
                    break;
                case "Estágio da Proposta_desc":
                    proposta = proposta.OrderByDescending(s => s.Status.Nome);
                    break;
                case "Estágio da Proposta":
                    proposta = proposta.OrderBy(s => s.Status.Nome);
                    break;
                case "Valor Total_desc":
                    proposta = proposta.OrderByDescending(s => s.ValorTotal);
                    break;
                case "Valor Total":
                    proposta = proposta.OrderBy(s => s.ValorTotal);
                    break;
                case "Validade_desc":
                    proposta = proposta.OrderByDescending(s => s.Validade);
                    break;
                case "Validade":
                    proposta = proposta.OrderBy(s => s.Validade);
                    break;
                default:
                    proposta = proposta.OrderBy(s => s.Entidade.Nome);
                    break;
            }

            if (TempData["Erro"] != null && !TempData["Erro"].ToString().Equals(""))
            {
                ViewBag.Erro = TempData["Erro"];
            }

            if (TempData["Aviso"] != null && !TempData["Aviso"].ToString().Equals(""))
            {
                ViewBag.Aviso = TempData["Aviso"];
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(proposta.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Relatorio(int id)
        {
            var document = new Document(PageSize.A4, 50, 50, 35, 35);

            var output = new MemoryStream();
            var writer = PdfWriter.GetInstance(document, output);

            Image waterMark = Image.GetInstance("http://" + Request.Url.Authority + "/images/MarcaDAguaCapa.png");
            waterMark.SetAbsolutePosition(180, 280);
            waterMark.ScaleAbsolute(604, 600);

            PropostaImpressa propostaImpressa = db.PropostaImpressa.Where(p => p.PropostaID == id).FirstOrDefault();
            Proposta proposta = db.Propostas.Find(id);
            Empresa empresa = db.Empresa.FirstOrDefault();

            PdfPageCustom capa = new PdfPageCustom(empresa.Endereco + " / Tel.: " + empresa.Telefone);

            //iTextSharp.text.html.simpleparser.StyleSheet styles = new iTextSharp.text.html.simpleparser.StyleSheet();
            iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);

            writer.PageEvent = capa;
            // Open the Document for writing
            document.Open();
            document.Add(waterMark);
            
            String html = "<html><header></header><body>";
            html += "<div style=\"float:left;  width:100%; height: 400px;\">" +
            "<img src='http://" + Request.Url.Authority + "/images/LiveLogoHorizontal.png' height=\"50\" width=\"246\" />" +
            "</div>";

            html += "<div style=\"float:left; width:100%;\">";

                html += "<div style=\"width:100%;\"><p style=\"font: 14pt Arial; color: #333; text-align: center;\">Proposta Comercial " + proposta.IdentProposta +
                        "</p></div>";

                html += "<div style=\"float: right; font: 10pt Arial;text-align:justify;\">";

                    html += "<p style=\"text-align: right; color: #333;\"><b>Referência:</b> " + proposta.Referencia + "</p>";
                    html += "<p style=\"text-align: right; color: #333;\">" + proposta.ContatoLive.Nome + "</p>";
                    if (proposta.ContatoLive.PapelContatos.Count() > 0)
                    {
                        html += "<p style=\"text-align: right; color: #333;\">" + proposta.ContatoLive.PapelContatos.FirstOrDefault().Nome + "</p>";
                    }
                    html += "<p style=\"text-align: right; color: #333;\">" + proposta.ContatoLive.Email + "</p>";
            
                html += "</div>";

            html += "</div>";

            html += "</body></html>";
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, new StringReader(html));

            PdfPageCustom page = new PdfPageCustom("");
            page.cabecalho = proposta.DataCriacao.ToString("dd/MM/yyyy") + "                                               Proposta Comercial                                               " + "Referência: " + proposta.IdentProposta;
            writer.PageEvent = page;

            document.NewPage();

            String htmlIntroducao = "";
            String htmlAcordoConfiabilidade = "";
            String htmlEscopoProjeto = "";
            String htmlConfigSolucao = "";
            String htmlInvestimentos = "";
            String htmlMetodologia = "";
            String htmlObjetivos = "";
            String htmlTermoAceite = "";
            String htmlCondicoesGerais = "";

            html = "<html><header></header><body>";

            String htmlCliente = "<div style=\"float:left; padding-top: 30px;\">"+
            "<div style='float:left'>" +
            "<div style=\"font: 10pt Arial;line-height: 150%\"><b>Cliente:</b> " + proposta.Entidade.Nome +"</div>" +
            "<div style=\"font: 8pt Arial;line-height: 150%\"></div>" +
            "</div>" +
            "<div style='float:right'>" +
            "<img src='http://" + Request.Url.Authority + "/images/LiveLogo.jpg' height=\"150\" width=\"232\" />" +
            "</div>" +
            "</div>";

            html += htmlCliente;

            if (propostaImpressa.IntroducaoSN)
            {
                htmlIntroducao = "<div style=\"padding-top:20px;font: 10pt Arial;text-align:justify;line-height: 150%\"><p>" + propostaImpressa.Introducao +
                "</p></div>";
                html += htmlIntroducao;
            }
            if (propostaImpressa.AcordoConfiabSN)
            {
                htmlAcordoConfiabilidade = "<div style=\"padding-top:30px\">" +
                "<div style=\"background-color: #D5D9EA;width: 100%;color: #191970;height: 25px;line-height: 20px;padding-left: 4px;\"><b>ACORDO DE CONFIABILIDADE</b></div>" +
                "<p style=\"font: 10pt Arial;text-align:justify;line-height: 150%\">" + propostaImpressa.AcordoConfiab +
                "</p></div>";
                html += htmlAcordoConfiabilidade;
            }
            html += "</body></html>";
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, new StringReader(html));

            html = "<html><header></header><style>p {page-break-inside:avoid;}</style><body>";
            document.NewPage();

            if (propostaImpressa.ObjetivosSN)
            {
                htmlObjetivos = "<div style=\"padding-top:30px\">" +
                "<div style=\"background-color: #D5D9EA;width: 100%;color: #191970;height: 25px;line-height: 20px;padding-left: 4px;\"><b>OBJETIVOS</b></div>" +
                "<p style=\"font: 10pt Arial;text-align:justify;line-height: 150%\">" + propostaImpressa.Objetivos +
                "</p></div>";
                html += alteraLinkImagem(htmlObjetivos);
            }
            if (propostaImpressa.ConfigSolucaoSN)
            {
                htmlConfigSolucao = "<div style=\"padding-top:30px\">" +
                "<div style=\"background-color: #D5D9EA;width: 100%;color: #191970;height: 25px;line-height: 20px;padding-left: 4px;\"><b>CONFIGURAÇÃO DA SOLUÇÃO</b></div>" +
                "<p style=\"font: 10pt Arial;text-align:justify;line-height: 150%\">" + propostaImpressa.ConfigSolucao +
                "</p></div>";
                html += alteraLinkImagem(htmlConfigSolucao);
            }
            if (propostaImpressa.EscopoProjetoSN)
            {
                htmlEscopoProjeto = "<div style=\"padding-top:30px\">" +
                "<div style=\"background-color: #D5D9EA;width: 100%;color: #191970;height: 25px;line-height: 20px;padding-left: 4px;\"><b>ESCOPO DO PROJETO</b></div>" +
                "<p style=\"font: 10pt Arial;text-align:justify;line-height: 150%\">" + propostaImpressa.EscopoProjeto +
                "</p></div>";
                html += alteraLinkImagem(htmlEscopoProjeto);
            }
            if (propostaImpressa.MetodologiaSN)
            {
                htmlMetodologia = "<div style=\"padding-top:30px\">" +
                "<div style=\"background-color: #D5D9EA;width: 100%;color: #191970;height: 25px;line-height: 20px;padding-left: 4px;\"><b>METODOLOGIA</b></div>" +
                "<p style=\"font: 10pt Arial;text-align:justify;line-height: 150%\">" + propostaImpressa.Metodologia +
                "</p></div>";
                html += alteraLinkImagem(htmlMetodologia);
            }
            if (propostaImpressa.InvestimentosSN)
            {
                String tabelaValores = montaTabelaValores(id);
                htmlInvestimentos = "<div style=\"padding-top:30px\">" +
                "<div style=\"background-color: #D5D9EA;width: 100%;color: #191970;height: 25px;line-height: 20px;padding-left: 4px;\"><b>INVESTIMENTOS</b></div>" +
                "<p style=\"font: 10pt Arial;text-align:justify\">Neste tópico serão apresentados os investimentos necessários para a contratação do serviço descritos nesta proposta:</p>" + tabelaValores +
                "<p style=\"font: 10pt Arial;text-align:justify;line-height: 150%\">" + propostaImpressa.Investimentos +
                "</p></div>";
                html += alteraLinkImagem(htmlInvestimentos);
            }
            if (propostaImpressa.Cond_DespesasSN || propostaImpressa.Cond_FaturamentoSN || propostaImpressa.Cond_FreteSN || propostaImpressa.Cond_ImpostosSN ||
                propostaImpressa.Cond_PrazoEntregaSN || propostaImpressa.Cond_ValidadeSN || propostaImpressa.Cond_ValoresSN || propostaImpressa.Cond_GarantiaSN || propostaImpressa.Cond_CampoLivreSN)
            {
                htmlCondicoesGerais = "<div style=\"padding-top:30px\">" +
                "<div style=\"background-color: #D5D9EA;width: 100%;color: #191970;height: 25px;line-height: 20px;padding-left: 4px;\"><b>CONDIÇÕES GERAIS DE VENDA</b></div>" +
                "<p style=\"font: 10pt Arial;text-align:justify;line-height: 150%\">Neste item são descritas as condições comerciais desta proposta:" +
                "</p></div>";
                //html += htmlCondicoesGerais;

                htmlCondicoesGerais += "<p><table width=\"100%\" bordercolor=\"#D3D3D3\" border=\"1\" cellpadding=\"4\"  cellspacing=\"0\">";

                if (propostaImpressa.Cond_DespesasSN)
                {
                    htmlCondicoesGerais += "<tr style=\"height: 25px;font: 10pt Arial\"><td style=\"background-color: #D5D9EA;width: 16%;color: #191970;padding-left: 4px;\"><b>Despesas</b></td><td>" + propostaImpressa.Cond_Despesas + "</td></tr>";
                }
                if (propostaImpressa.Cond_FaturamentoSN)
                {
                    htmlCondicoesGerais += "<tr style=\"height: 25px;font: 10pt Arial\"><td style=\"background-color: #D5D9EA;width: 16%;color: #191970;padding-left: 4px;\"><b>Faturamento</b></td><td>" + propostaImpressa.Cond_Faturamento + "</td></tr>";
                }
                if (propostaImpressa.Cond_FreteSN)
                {
                    htmlCondicoesGerais += "<tr style=\"height: 25px;font: 10pt Arial\"><td style=\"background-color: #D5D9EA;width: 16%;color: #191970;padding-left: 4px;\"><b>Frete</b></td><td>" + propostaImpressa.Cond_Frete + "</td></tr>";
                }
                if (propostaImpressa.Cond_ImpostosSN)
                {
                    htmlCondicoesGerais += "<tr style=\"height: 25px;font: 10pt Arial\"><td style=\"background-color: #D5D9EA;width: 16%;color: #191970;padding-left: 4px;\"><b>Preço/Impostos</b></td><td>" + propostaImpressa.Cond_Impostos + "</td></tr>";
                }
                if (propostaImpressa.Cond_PrazoEntregaSN)
                {
                    htmlCondicoesGerais += "<tr style=\"height: 25px;font: 10pt Arial\"><td style=\"background-color: #D5D9EA;width: 16%;color: #191970;padding-left: 4px;\"><b>Prazo de Entrega</b></td><td>" + propostaImpressa.Cond_PrazoEntrega + "</td></tr>";
                }
                if (propostaImpressa.Cond_ValidadeSN)
                {
                    htmlCondicoesGerais += "<tr style=\"height: 25px;font: 10pt Arial\"><td style=\"background-color: #D5D9EA;width: 16%;color: #191970;padding-left: 4px;\"><b>Validade</b></td><td>" + propostaImpressa.Cond_Validade + "</td></tr>";
                }
                if (propostaImpressa.Cond_ValoresSN)
                {
                    htmlCondicoesGerais += "<tr style=\"height: 25px;font: 10pt Arial\"><td style=\"background-color: #D5D9EA;width: 16%;color: #191970;padding-left: 4px;\"><b>Valores</b></td><td>" + propostaImpressa.Cond_Valores + "</td></tr>";
                }
                if (propostaImpressa.Cond_GarantiaSN)
                {
                    htmlCondicoesGerais += "<tr style=\"height: 25px;font: 10pt Arial\"><td style=\"background-color: #D5D9EA;width: 16%;color: #191970;padding-left: 4px;\"><b>Garantia</b></td><td>" + propostaImpressa.Cond_Garantia + "</td></tr>";
                }

                htmlCondicoesGerais += "</table></p>";
                
                if (propostaImpressa.Cond_CampoLivreSN)
                {
                    htmlCondicoesGerais += "<div style=\"font: 10pt Arial;\">" + propostaImpressa.Cond_CampoLivre + "</div>";
                }

                html += htmlCondicoesGerais;
            }

            if (propostaImpressa.TermoAceiteSN)
            {
                htmlTermoAceite = "<div style=\"padding-top:30px\">" +
                "<div style=\"background-color: #D5D9EA;width: 100%;color: #191970;height: 25px;line-height: 20px;padding-left: 4px;\"><b>TERMOS DE ACEITE</b></div>" +
                "<p style=\"font: 10pt Arial;text-align:justify;line-height: 150%\">" + propostaImpressa.TermoAceite + "</p><br /><br />" +
                "<div style=\"font: 10pt Arial; float:left; padding-left: 20px\"><b>Data:</b><br /><br />____/____/____</div>" +
                "<div style=\"font: 10pt Arial; float:left; padding-left: 20px\"><b>CNPJ p/ Faturamento</b><br /><br />____.____.____/_____-____</div>" +
                "<div style=\"font: 10pt Arial; float:left; padding-left: 20px\"><b>Assinatura do Responsável</b><br /><br />_________________________________________</div></div>";
                html += alteraLinkImagem(htmlTermoAceite);
            }

            String htmlRevendas = "<div style=\"padding-top:40px\">" +
                "<p style=\"font: 10pt Arial;text-align:justify;line-height: 150%\"> Somos revendas certificados nos seguintes fabricantes de solução tecnológicas:" +
                "</p>" +
                "<p style=\"padding-left: 10px; padding-top: 20px;\" ><img src='http://" + Request.Url.Authority + "/images/RenvedasCertificadasLiveConsult.png' height=\"97\" width=\"695\" /></p></div>";
            
            html += htmlRevendas + "</body></html>";
            //hw.Parse(new StringReader(html));
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, new StringReader(html));
            // Close the Document - this saves the document contents to the output stream
            document.Close();

            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline;filename=" + proposta.IdentProposta + "-" + proposta.Entidade.Nome +".pdf");
            Response.BinaryWrite(output.ToArray());

            return null;
        }

        private string montaTabelaValores(int id)
        {
            PropostaImpressa propostaImpressa = db.PropostaImpressa.Find(id);
            Proposta proposta = db.Propostas.Find(id);
            String htmlInvestimentos = "<p style=\"font: 10pt Arial\"><table style=\"font: 10pt Arial;\" width=\"100%\" bordercolor=\"#D3D3D3\" border=\"1\" cellpadding=\"4\"  cellspacing=\"0\">";

            DataSet ds = new DataSet("Investimento");
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand("SP_PropostaRelatorio", conn);
                sqlComm.Parameters.AddWithValue("@id", id);

                sqlComm.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;

                da.Fill(ds);
            }

            List<String> AreaEscopo = new List<string>();

            foreach (DataRow item in ds.Tables[0].Rows)
            {
                if (!AreaEscopo.Contains(item[8].ToString()))
                {
                    AreaEscopo.Add(item[8].ToString());
                }
            }

            htmlInvestimentos += "<tr style=\"text-align: center; color: white; background-color: #5B668F; font-weight: bold\" ><td>ITEM DA PROPOSTA</td><td>UND</td><td>QTD</td><td>MOEDA</td><td>VLR UNITÁRIO</td><td>VALOR</td></tr>";
            foreach (var item in AreaEscopo)
            {
                htmlInvestimentos += "<tr><th style=\"text-align: center; color: white; background-color: #99A7BC\" colspan=\"6\"><b>" + item + "</b></th></tr>";
                Decimal somaReal = 0;
                Decimal somaDolar = 0;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (item.Equals(row[8].ToString()))
                    {
                        htmlInvestimentos += "<tr>";
                        htmlInvestimentos += "<td>" + row[9].ToString() + row[10].ToString() + "</td>";
                        htmlInvestimentos += "<td>" + row[11].ToString() + "</td>";
                        htmlInvestimentos += "<td style=\"text-align: right\">" + row[3].ToString() + "</td>";
                        htmlInvestimentos += "<td>" + row[7].ToString() + "</td>";
                        htmlInvestimentos += "<td style=\"text-align: right\">" + string.Format("{0:n2}", Convert.ToDecimal(row[4].ToString())) + "</td>";
                        htmlInvestimentos += "<td style=\"text-align: right\">";
                        if (row[7].ToString().Equals("U$"))
                        {
                            htmlInvestimentos += string.Format("{0:n2}", Convert.ToDecimal(row[6].ToString()));
                            somaDolar += Convert.ToDecimal(row[6].ToString());
                        }
                        else
                        {
                            htmlInvestimentos += string.Format("{0:n2}", Convert.ToDecimal(row[5].ToString()));
                            somaReal += Convert.ToDecimal(row[5].ToString());
                        }
                        htmlInvestimentos += "</td>";
                        htmlInvestimentos += "</tr>";
                    }
                }
                htmlInvestimentos += "<tr><td style=\"text-align: right\" colspan=\"6\"><b>Total por Área: ";
                if(somaReal > 0)
                {
                    htmlInvestimentos += "R$ " + string.Format("{0:n2}", somaReal);
                }
                if(somaDolar > 0)
                {
                    if (somaReal > 0)
                    {
                        htmlInvestimentos += " / ";
                    }
                    htmlInvestimentos += "U$ " + string.Format("{0:n2}", somaDolar);
                }
                htmlInvestimentos += "</b></td></tr>";
            }
            htmlInvestimentos += "</table></p>";
            return htmlInvestimentos;
        }

        public String alteraLinkImagem(String html)
        {
            return html.Replace("../..", "http://" + Request.Url.Authority);
        }

        // GET: /Proposta/Details/5
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proposta proposta = db.Propostas.Find(id);
            if (proposta == null)
            {
                return HttpNotFound();
            }
            ProximoPassoProposta proximoPasso = db.ProximoPassoProposta.Where(p => p.PropostaID == id).FirstOrDefault();
            if (proximoPasso != null)
            {
                ViewBag.ProximoPassoProposta = proximoPasso.ProximoPassoPropostaID;
                ViewBag.StatusProximoPasso = proximoPasso.Status;
                if (proximoPasso.TipoAlerta != null)
                {
                    ViewBag.AlertaProximoPasso = proximoPasso.TipoAlerta;
                }
            }

            ViewBag.ValorServico = 0;
            ViewBag.ValorServicoDolar = 0;
            ViewBag.ValorEquipamento = 0;
            ViewBag.ValorEquipamentoDolar = 0;

            if (db.ItemPropostaServico.Where(p => p.PropostaID == id).Count() > 0)
            {
                foreach (var item in db.ItemPropostaServico.Where(p => p.PropostaID == id).ToList())
                {
                    if(item.Moeda != null && item.Moeda.Equals("U$"))
                    {
                        ViewBag.ValorServicoDolar += item.Quantidade * item.ValorUnitario;
                    }
                    else
                    {
                        ViewBag.ValorServico += item.Quantidade * item.ValorUnitario;
                    }
                    
                }
            }

            if (db.ItemPropostaEquipamento.Where(p => p.PropostaID == id).Count() > 0)
            {
                foreach (var item in db.ItemPropostaEquipamento.Where(p => p.PropostaID == id).ToList())
                {
                    if (item.Moeda != null && item.Moeda.Equals("U$"))
                    {
                        ViewBag.ValorEquipamentoDolar += item.Quantidade * item.ValorUnitario;
                    }
                    else
                    {
                        ViewBag.ValorEquipamento += item.Quantidade * item.ValorUnitario;
                    }
                }
            }

            if (ViewBag.ValorEquipamento + ViewBag.ValorServico != proposta.ValorTotal)
            {
                TempData["Aviso"] = "A soma dos valores dos itens da proposta está diferente do valor total.";
            }
            
            if (TempData["Erro"] != null && !TempData["Erro"].ToString().Equals(""))
            {
                ViewBag.Erro = TempData["Erro"];
            }

            if (TempData["Aviso"] != null && !TempData["Aviso"].ToString().Equals(""))
            {
                ViewBag.Aviso = TempData["Aviso"];
            }

            if (TempData["Msg"] != null && !TempData["Msg"].ToString().Equals(""))
            {
                ViewBag.Msg = TempData["Msg"];
            }

            ViewData["HistoricoProposta"] = id;
            
            return View(proposta);
        }

        // GET: /Proposta/Details/5
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult ItensProposta(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proposta proposta = db.Propostas.Find(id);
            if (proposta == null)
            {
                return HttpNotFound();
            }

            List<AreaEscopo> areasEscopoProposta = new List<AreaEscopo>();

            foreach (var item in proposta.ItemPropostaServicos)
            {
                if (!areasEscopoProposta.Contains(item.AreaEscopo))
                {
                    areasEscopoProposta.Add(item.AreaEscopo);
                }
            }

            foreach (var item in proposta.ItemPropostaEquipamentos)
            {
                if (!areasEscopoProposta.Contains(item.AreaEscopo))
                {
                    areasEscopoProposta.Add(item.AreaEscopo);
                }
            }

            ViewBag.AreasEscopo = areasEscopoProposta;

            foreach (var item in areasEscopoProposta)
            {
                String[] valores = SomaValoresAreaEscopo(item.AreaEscopoID, proposta).Split(';');
                ViewData[item.Nome] = Convert.ToDecimal(valores[0]);
                ViewData[item.Nome + "Dolar"] = Convert.ToDecimal(valores[1]);
            }
            
            if (TempData["Erro"] != null && !TempData["Erro"].ToString().Equals(""))
            {
                ViewBag.Erro = TempData["Erro"];
            }

            if (TempData["Aviso"] != null && !TempData["Aviso"].ToString().Equals(""))
            {
                ViewBag.Aviso = TempData["Aviso"];
            }

            return View(proposta);
        }

        public ActionResult ExibirPropostasPorStatus(String estagio)
        {
            return RedirectToAction("Index", "Proposta", new { estagioSearch = estagio });
        }

        private String SomaValoresAreaEscopo(int areaEscopoID, Proposta proposta)
        {
            Decimal soma = 0;
            Decimal somaDolar = 0;
            
            foreach (var item in proposta.ItemPropostaEquipamentos)
            {
                if(item.AreaEscopoID == areaEscopoID)
                {
                    if (item.Moeda.Equals("U$"))
                    {
                        somaDolar += item.Quantidade * item.ValorUnitario;
                    }
                    else
                    {
                        soma += item.Quantidade * item.ValorUnitario;
                    }
                }
            }

            foreach (var item in proposta.ItemPropostaServicos)
            {
                if (item.AreaEscopoID == areaEscopoID)
                {
                    if (item.Moeda.Equals("U$"))
                    {
                        somaDolar += item.Quantidade * item.ValorUnitario;
                    }
                    else
                    {
                        soma += item.Quantidade * item.ValorUnitario;
                    }
                }
            }

            return soma + ";" + somaDolar ;
        }

        public ActionResult RecalcularValorProposta(int id)
        {
            Proposta proposta = db.Propostas.Find(id);
            Decimal valor = 0;
            Decimal valorDolar = 0;

            foreach (var item in proposta.ItemPropostaServicos)
            {
                if (item.Moeda != null & item.Moeda.Equals("U$"))
                {
                    valorDolar += item.Quantidade * item.ValorUnitario;
                }
                else
                {
                    valor += item.Quantidade * item.ValorUnitario;
                }
            }

            foreach (var item in proposta.ItemPropostaEquipamentos)
            {
                if (item.Moeda != null & item.Moeda.Equals("U$"))
                {
                    valorDolar += item.Quantidade * item.ValorUnitario;
                }
                else
                {
                    valor += item.Quantidade * item.ValorUnitario;
                }
            }

            proposta.ValorTotal = valor;
            proposta.ValorTotalDolar = valorDolar;

            db.Propostas.Add(proposta);
            db.Entry(proposta).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                TempData["Msg"] = "O valor da proposta foi recalculado com sucesso.";
            }
            catch (Exception ex)
            {
                ViewBag.Erro = "Não foi possível salvar a proposta " + proposta.IdentProposta + ": " + ex.Message;
            }

            return RedirectToAction("Details", "Proposta", new { id = proposta.PropostaID });
        }

        public ActionResult BuscaContatos(int id)
        {
            //int id = 0;
            Entidade entidade = db.Entidade.Find(id);
            var result = (from s in entidade.Contato
                          select new { id = s.ContatoID, name = s.Nome })
                          .ToList();

            return Json(result, JsonRequestBehavior.AllowGet); 
        }

        // GET: /Proposta/Create
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Create()
        {
            Empresa empresa = db.Empresa.FirstOrDefault();
            if (empresa != null)
            {
                List<Entidade> naoProp  = db.Entidade.Include(p => p.Contato).ToList();
                List<Contato> contatoNaoProp = new List<Contato>();

                foreach (var item in naoProp)
                {
                    foreach (var cont in item.Contato)
                    {
                        if(!empresa.Colaboradores.Contains(cont)){
                            contatoNaoProp.Add(cont);
                        }
                    }
                }

                ViewBag.ContatoID = new SelectList(contatoNaoProp, "ContatoID", "Nome");
                ViewBag.RespLiveID = new SelectList(empresa.Colaboradores, "ContatoID", "Nome");
                ViewBag.EntidadeID = new SelectList(db.Entidade.ToList(), "EntidadeID", "Nome");
            }
            else
            {
                TempData["Aviso"] = "Favor cadastrar uma empresa como proprietária ou configurar uma existente.";
                return RedirectToAction("Index");
            }
            ViewBag.StatusSigla = new SelectList(db.Status, "StatusSigla", "Nome");
            return View();
        }

        // POST: /Proposta/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PropostaID,EntidadeID,ValorTotal,Validade,RespLiveID,ContatoID,StatusSigla,Anexo,ContentType,NomeArquivo,IdentProposta,Versao,DataCriacao,Cambio,ValorTotalDolar,Referencia")] Proposta proposta, HttpPostedFileBase uploadFile)
        {
            if (ModelState.IsValid)
            {
                String data = DateTime.Now.ToString("yyyyMMdd");

                var lista = db.Propostas.Where(p => p.IdentProposta.Contains(data)).ToList();

                if(lista.Count > 0)
                {
                    foreach (var item in lista)
                    {
                        String texto = ((Proposta)item).IdentProposta.Substring(8,3);
                        int numero = Convert.ToInt32(texto);
                        numero++;
                        if(numero.ToString().Length == 1)
                        {
                            proposta.IdentProposta = data + "00" + numero;
                        }
                        else if(numero.ToString().Length == 2)
                        {
                            proposta.IdentProposta = data + "0" + numero;
                        }
                        else
                        {
                            proposta.IdentProposta = data + numero;
                        }
                    }
                }
                else{
                    proposta.IdentProposta = data + "001";
                }

                if (uploadFile != null)
                {
                    byte[] buffer = new byte[uploadFile.ContentLength];
                    uploadFile.InputStream.Read(buffer, 0, uploadFile.ContentLength);
                    proposta.ContentType = uploadFile.ContentType;
                    proposta.NomeArquivo = uploadFile.FileName;
                    proposta.Anexo = buffer;
                }
                proposta.DataCriacao = DateTime.Now.Date;

                db.Propostas.Add(proposta);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Não foi possível salvar a proposta " + proposta.IdentProposta + ": " + ex.Message;
                    return View(proposta);
                }
                return RedirectToAction("Index");
            }

            Empresa empresa = db.Empresa.FirstOrDefault();
            if (empresa != null)
            {
                List<Entidade> naoProp = db.Entidade.Include(p => p.Contato).ToList();
                List<Contato> contatoNaoProp = new List<Contato>();

                foreach (var item in naoProp)
                {
                    foreach (var cont in item.Contato)
                    {
                        if (!empresa.Colaboradores.Contains(cont))
                        {
                            contatoNaoProp.Add(cont);
                        }
                    }
                }

                ViewBag.ContatoID = new SelectList(contatoNaoProp, "ContatoID", "Nome", proposta.ContatoID);
                ViewBag.RespLiveID = new SelectList(empresa.Colaboradores, "ContatoID", "Nome", proposta.RespLiveID);
                ViewBag.EntidadeID = new SelectList(db.Entidade.ToList(), "EntidadeID", "Nome", proposta.EntidadeID);
            }
            ViewBag.StatusSigla = new SelectList(db.Status, "StatusSigla", "Nome", proposta.StatusSigla);
            return View(proposta);
        }

        // GET: /Proposta/Edit/5
        [PermissoesFiltro(Roles = "Proposta")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proposta proposta = db.Propostas.Find(id);
            if (proposta == null)
            {
                return HttpNotFound();
            }
            Empresa empresa = db.Empresa.FirstOrDefault();
            if (empresa != null)
            {
                List<Entidade> naoProp = db.Entidade.Where(p => p.EntidadeID == proposta.EntidadeID).Include(p => p.Contato).ToList();
                List<Contato> contatoNaoProp = new List<Contato>();

                foreach (var item in naoProp)
                {
                    foreach (var cont in item.Contato)
                    {
                        if (!empresa.Colaboradores.Contains(cont))
                        {
                            contatoNaoProp.Add(cont);
                        }
                    }
                }

                ViewBag.ContatoID = new SelectList(contatoNaoProp, "ContatoID", "Nome", proposta.ContatoID);
                ViewBag.RespLiveID = new SelectList(empresa.Colaboradores, "ContatoID", "Nome", proposta.RespLiveID);
                ViewBag.EntidadeID = new SelectList(db.Entidade.ToList(), "EntidadeID", "Nome", proposta.EntidadeID);
                ViewData["NomeArquivo"] = proposta.NomeArquivo;
                ViewData["ContentType"] = proposta.ContentType;
            }
            else 
            {
                TempData["Aviso"] = "Favor cadastrar uma empresa como proprietária ou configurar uma existente.";
                return RedirectToAction("Details", new { id = id });
            }
            ViewBag.StatusSigla = new SelectList(db.Status, "StatusSigla", "Nome", proposta.StatusSigla);

            return View(proposta);
        }

        // POST: /Proposta/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PropostaID,EntidadeID,ValorTotal,Validade,RespLiveID,ContatoID,StatusSigla,Anexo,ContentType,NomeArquivo,IdentProposta,Versao,DataCriacao,Cambio,ValorTotalDolar,Referencia")] Proposta proposta, HttpPostedFileBase uploadFile)
        {
            
            if (ModelState.IsValid)
            {
                if (uploadFile != null) { 
                    byte[] buffer = new byte[uploadFile.ContentLength];
                    uploadFile.InputStream.Read(buffer, 0, uploadFile.ContentLength);
                    proposta.ContentType = uploadFile.ContentType;
                    proposta.NomeArquivo = uploadFile.FileName;
                    proposta.Anexo = buffer;
                }
                db.Propostas.Add(proposta);
                db.Entry(proposta).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    if (proposta.StatusSigla.ToUpper().Equals("AP"))
                    {
                        this.EnviaEmailAprovacao(proposta.PropostaID);
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Não foi possível salvar a proposta " + proposta.IdentProposta + ": " + ex.Message;
                }
            }
            Empresa empresa = db.Empresa.FirstOrDefault();
            if (empresa != null)
            {
                List<Entidade> naoProp = db.Entidade.Include(p => p.Contato).ToList();
                List<Contato> contatoNaoProp = new List<Contato>();

                foreach (var item in naoProp)
                {
                    foreach (var cont in item.Contato)
                    {
                        if (!empresa.Colaboradores.Contains(cont))
                        {
                            contatoNaoProp.Add(cont);
                        }
                    }
                }

                ViewBag.ContatoID = new SelectList(contatoNaoProp, "ContatoID", "Nome", proposta.ContatoID);
                ViewBag.RespLiveID = new SelectList(empresa.Colaboradores, "ContatoID", "Nome", proposta.RespLiveID);
                ViewBag.EntidadeID = new SelectList(db.Entidade.ToList(), "EntidadeID", "Nome", proposta.EntidadeID);
            }
            ViewBag.StatusSigla = new SelectList(db.Status, "StatusSigla", "Nome", proposta.StatusSigla);
            return View(proposta);
        }

        public ActionResult CopiarProposta(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proposta proposta = db.Propostas.Find(id);
            if (proposta == null)
            {
                return HttpNotFound();
            }

            Empresa empresa = db.Empresa.FirstOrDefault();
            if (empresa != null)
            {
                List<Entidade> naoProp = db.Entidade.Include(p => p.Contato).ToList();
                List<Contato> contatoNaoProp = new List<Contato>();

                foreach (var item in naoProp)
                {
                    foreach (var cont in item.Contato)
                    {
                        if (!empresa.Colaboradores.Contains(cont))
                        {
                            contatoNaoProp.Add(cont);
                        }
                    }
                }

                ViewBag.ContatoID = new SelectList(contatoNaoProp, "ContatoID", "Nome", proposta.ContatoID);
                ViewBag.RespLiveID = new SelectList(empresa.Colaboradores, "ContatoID", "Nome", proposta.RespLiveID);
                ViewBag.EntidadeID = new SelectList(db.Entidade.ToList(), "EntidadeID", "Nome", proposta.EntidadeID);
            }
            ViewBag.StatusSigla = new SelectList(db.Status, "StatusSigla", "Nome", proposta.StatusSigla);
            
            return View(proposta); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CopiarProposta([Bind(Include = "PropostaID,EntidadeID,ValorTotal,Validade,RespLiveID,ContatoID,StatusSigla,Anexo,ContentType,NomeArquivo,IdentProposta,Versao,DataCriacao,Cambio,ValorTotalDolar,Referencia")] Proposta proposta, HttpPostedFileBase uploadFile)
        {
            if (ModelState.IsValid)
            {
                String data = DateTime.Now.ToString("yyyyMMdd");

                var lista = db.Propostas.Where(p => p.IdentProposta.Contains(data)).ToList();

                if (lista.Count > 0)
                {
                    foreach (var item in lista)
                    {
                        String texto = ((Proposta)item).IdentProposta.Substring(8, 3);
                        int numero = Convert.ToInt32(texto);
                        numero++;
                        if (numero.ToString().Length == 1)
                        {
                            proposta.IdentProposta = data + "00" + numero;
                        }
                        else if (numero.ToString().Length == 2)
                        {
                            proposta.IdentProposta = data + "0" + numero;
                        }
                        else
                        {
                            proposta.IdentProposta = data + numero;
                        }
                    }
                }
                else
                {
                    proposta.IdentProposta = data + "001";
                }

                if (uploadFile != null)
                {
                    byte[] buffer = new byte[uploadFile.ContentLength];
                    uploadFile.InputStream.Read(buffer, 0, uploadFile.ContentLength);
                    proposta.ContentType = uploadFile.ContentType;
                    proposta.NomeArquivo = uploadFile.FileName;
                    proposta.Anexo = buffer;
                }
                proposta.DataCriacao = DateTime.Now.Date;
                int propostaIDAnterior = proposta.PropostaID;
                proposta.PropostaID = 0;
                db.Propostas.Add(proposta);
                db.SaveChanges();
                proposta = db.Propostas.Where(p => p.IdentProposta.Equals(proposta.IdentProposta)).FirstOrDefault();
                CopiarItensProposta(proposta.PropostaID, propostaIDAnterior);
                CopiarPropostaImpressa(proposta.PropostaID, propostaIDAnterior);
                return RedirectToAction("Index");
            }
            Empresa empresa = db.Empresa.FirstOrDefault();
            if (empresa != null)
            {
                List<Entidade> naoProp = db.Entidade.Include(p => p.Contato).ToList();
                List<Contato> contatoNaoProp = new List<Contato>();

                foreach (var item in naoProp)
                {
                    foreach (var cont in item.Contato)
                    {
                        if (!empresa.Colaboradores.Contains(cont))
                        {
                            contatoNaoProp.Add(cont);
                        }
                    }
                }

                ViewBag.ContatoID = new SelectList(contatoNaoProp, "ContatoID", "Nome", proposta.ContatoID);
                ViewBag.RespLiveID = new SelectList(empresa.Colaboradores, "ContatoID", "Nome", proposta.RespLiveID);
                ViewBag.EntidadeID = new SelectList(db.Entidade.ToList(), "EntidadeID", "Nome", proposta.EntidadeID);
            }
            ViewBag.StatusSigla = new SelectList(db.Status, "StatusSigla", "Nome", proposta.StatusSigla);
            return View(proposta);
        }

        public void CopiarItensProposta(int id, int propostaIDAnterior)
        {
            List<ItemPropostaEquipamento> itemEquipamento = db.ItemPropostaEquipamento.Where(p => p.PropostaID == propostaIDAnterior).ToList();
            List<ItemPropostaServico> itemServico = db.ItemPropostaServico.Where(p => p.PropostaID == propostaIDAnterior).ToList();

            foreach (var item in itemEquipamento)
            {
                item.PropostaID = id;
                ItemPropostaEquipamento ipe = item;
                db.ItemPropostaEquipamento.Add(ipe);
            }
            foreach (var item in itemServico)
            {
                item.PropostaID = id;
                ItemPropostaServico ips = item;
                db.ItemPropostaServico.Add(ips);
            }
            db.SaveChanges();
        }

        public void CopiarPropostaImpressa(int id, int propostaIDAnterior)
        {
            PropostaImpressa propostaImpressa = db.PropostaImpressa.Where(p => p.PropostaID == propostaIDAnterior).FirstOrDefault();
            if(propostaImpressa != null)
            {
                propostaImpressa.PropostaID = id;
                PropostaImpressa novaProposta = propostaImpressa;
                db.PropostaImpressa.Add(novaProposta);

                db.SaveChanges();
            }
        }

        public void EnviaEmailAprovacao(int id)
        {
            String smtp = "";
            String porta = "";
            String loginHost = "";
            String senhaHost = "";
            String emailPadraoTo = "";
            String emailFrom = "";
            //if (!EventLog.SourceExists("LiveCore"))
            //    EventLog.CreateEventSource("LiveCore", "Application");

            using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                cnn.Open();
                SqlDataReader reader = null;
                SqlCommand cmd = new SqlCommand("select * from EMAILCONFIG");
                cmd.Connection = cnn;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    smtp = reader["HOST"].ToString();
                    porta = reader["PORTA"].ToString();
                    emailFrom = reader["EMAILFROM"].ToString();
                    emailPadraoTo = reader["EMAILDEFAULTTO"].ToString();
                }
                cnn.Close();
            }

            Proposta proposta = db.Propostas.Where(p => p.PropostaID == id).Include(p => p.Entidade).FirstOrDefault();
            Empresa empresa = db.Empresa.FirstOrDefault();
            if (empresa != null && (empresa.EmailFinanceiro != null && !empresa.EmailFinanceiro.Equals("")) 
                && (empresa.EmailComercial != null && !empresa.EmailComercial.Equals("")))
            {
                if (!smtp.Trim().Equals("") && !porta.Trim().Equals(""))
                {
                    try
                    {
                        SmtpClient SmtpServer = new SmtpClient();
                        //SmtpServer.Credentials = new System.Net.NetworkCredential(loginHost, senhaHost);
                        SmtpServer.Credentials = CredentialCache.DefaultNetworkCredentials;

                        SmtpServer.Port = Convert.ToInt32(porta);
                        SmtpServer.Host = smtp;
                        SmtpServer.EnableSsl = false;
                        MailMessage mail = new MailMessage();
                        mail.IsBodyHtml = true;
                        mail.From = new MailAddress(emailFrom);
                        mail.To.Add(empresa.EmailFinanceiro);
                        mail.To.Add(empresa.EmailComercial);
                        mail.Subject = "Proposta " + proposta.IdentProposta + " aprovada - " + proposta.Entidade.Nome;
                        mail.Body = "A proposta " + proposta.IdentProposta + "foi aprovada.<br>Para visualizar a proposta, clique <a href='http://" + Request.Url.Authority + "/Proposta/Relatorio/" +
                        proposta.PropostaID.ToString() + "'><b>AQUI</b></a>";

                        mail.IsBodyHtml = true;
                        mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                        SmtpServer.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        StreamWriter wFile;
                        wFile = new StreamWriter("c:\\inetpub\\LogEmailAprovacao.txt", true);
                        wFile.WriteLine(ex.Message.ToString());
                        if (ex.InnerException != null)
                        {
                            wFile.WriteLine(ex.InnerException.Message);
                        }
                        wFile.Close();
                        ViewBag.Erro = "N: " + ex.Message;
                    
                    }
                }
            }
        } 

        // GET: /Proposta/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proposta proposta = db.Propostas.Find(id);
            if (proposta == null)
            {
                return HttpNotFound();
            }
            return View(proposta);
        }

        // POST: /Proposta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Proposta proposta = db.Propostas.Find(id);
            foreach (var item in db.HistoricoPropostas.Where(p => p.PropostaID == proposta.PropostaID))
	        {
                db.HistoricoPropostas.Remove(item);
	        }
            foreach (var item in db.ItemPropostaEquipamento.Where(p => p.PropostaID == proposta.PropostaID))
            {
                db.ItemPropostaEquipamento.Remove(item);
            } 
            foreach (var item in db.ItemPropostaServico.Where(p => p.PropostaID == proposta.PropostaID))
            {
                db.ItemPropostaServico.Remove(item);
            }
            foreach (var item in db.PropostaImpressa.Where(p => p.PropostaID == proposta.PropostaID))
            {
                db.PropostaImpressa.Remove(item);
            }
            db.Propostas.Remove(proposta);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                TempData["Erro"] = "Não foi possível deletar a proposta " + proposta.IdentProposta + ". Alguns registros fazem referência a essa proposta.";
                return RedirectToAction("Details", "Proposta", new { id = proposta.PropostaID });
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Não foi possível deletar a proposta " + proposta.IdentProposta + ": " + ex.Message;
                return RedirectToAction("Details", "Proposta", new { id = proposta.PropostaID });
            }
            return RedirectToAction("Index");
        }

        public ActionResult BuscaAnexo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proposta proposta = db.Propostas.Find(id);
            if (proposta == null)
            {
                return HttpNotFound();
            }

            Response.AddHeader("Content-type", proposta.ContentType);
            Response.AddHeader("Content-Disposition", "attachment; filename=" + proposta.NomeArquivo);
            Response.BinaryWrite(proposta.Anexo);
            Response.Flush();
            Response.End();

            return View(proposta);
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
