using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace LiveCore.Models
{
    public class PdfPageCustom : iTextSharp.text.pdf.PdfPageEventHelper
    {
        String capaFooter = "";
        
        public String cabecalho { get; set; }

        public PdfPageCustom(String capaRodape)
        {
            capaFooter = capaRodape;
            cabecalho = "";
        }

        //I create a font object to use within my footer
        protected Font footer
        {
            get
            {
                // create a basecolor to use for the footer font, if needed.
                BaseColor grey = new BaseColor(128, 128, 128);
                Font font = FontFactory.GetFont("Arial", 9, Font.NORMAL, grey);
                return font;
            }
        }

        protected Font header
        {
            get
            {
                // create a basecolor to use for the footer font, if needed.
                BaseColor grey = new BaseColor(128, 128, 128);
                Font font = FontFactory.GetFont("Arial", 10, Font.NORMAL, grey);
                return font;
            }
        }
        //override the OnStartPage event handler to add our header
        public override void OnStartPage(PdfWriter writer, Document doc)
        {
            //I use a PdfPtable with 1 column to position my header where I want it
            PdfPTable headerTbl = new PdfPTable(1);

            //set the width of the table to be the same as the document
            headerTbl.TotalWidth = doc.PageSize.Width;

            //I use an image logo in the header so I need to get an instance of the image to be able to insert it. I believe this is something you couldn't do with older versions of iTextSharp
            //Image logo = Image.GetInstance(HttpContext.Current.Server.MapPath("/Content/images/ui-bg_flat_55_fbec88_40x100.png"));

            //I used a large version of the logo to maintain the quality when the size was reduced. I guess you could reduce the size manually and use a smaller version, but I used iTextSharp to reduce the scale. As you can see, I reduced it down to 7% of original size.
            //logo.ScalePercent(7);

            Paragraph para = new Paragraph(cabecalho,header);

            //create instance of a table cell to contain the logo
            PdfPCell cell = new PdfPCell(para);

            //align the logo to the right of the cell
            cell.HorizontalAlignment = Element.ALIGN_CENTER;

            //add a bit of padding to bring it away from the right edge
            //cell.PaddingRight = 20;

            //remove the border
            cell.Border = 0;

            //Add the cell to the table
            headerTbl.AddCell(cell);

            //write the rows out to the PDF output stream. I use the height of the document to position the table. Positioning seems quite strange in iTextSharp and caused me the biggest headache.. It almost seems like it starts from the bottom of the page and works up to the top, so you may ned to play around with this.
            headerTbl.WriteSelectedRows(0, -1, 0, (doc.PageSize.Height - 10), writer.DirectContent);

        }

        //override the OnPageEnd event handler to add our footer
        public override void OnEndPage(PdfWriter writer, Document doc)
        {
            if(!capaFooter.Equals(""))
            {
                PdfPTable footerTbl = new PdfPTable(1);

                //set the width of the table to be the same as the document
                footerTbl.TotalWidth = doc.PageSize.Width;

                //Center the table on the page
                footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;

                //create new instance of Paragraph for 2nd cell text
                Paragraph para = new Paragraph(capaFooter, footer);

                //create new instance of cell to hold the text
                PdfPCell cell = new PdfPCell(para);

                //align the text to the right of the cell
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //set border to 0
                cell.Border = 0;

                // add some padding to take away from the edge of the page
                cell.PaddingRight = 10;

                //add the cell to the table
                footerTbl.AddCell(cell);

                //write the rows out to the PDF output stream.
                footerTbl.WriteSelectedRows(0, -1, 0, (doc.BottomMargin + 5), writer.DirectContent);

                capaFooter = "";

            }
            else
            {
                //I use a PdfPtable with 2 columns to position my footer where I want it
                PdfPTable footerTbl = new PdfPTable(1);

                //set the width of the table to be the same as the document
                footerTbl.TotalWidth = doc.PageSize.Width;

                //Center the table on the page
                footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;

                //Create a paragraph that contains the footer text
                //Paragraph para = new Paragraph(doc.PageNumber.ToString(), footer);

                //add a carriage return
                //para.Add(Environment.NewLine);
                //para.Add("Teste");

                //create a cell instance to hold the text
                //PdfPCell cell = new PdfPCell(para);

                //set cell border to 0
                //cell.Border = 0;

                //add some padding to bring away from the edge
                //cell.PaddingLeft = 10;

                //add cell to table
                //footerTbl.AddCell(cell);

                //create new instance of Paragraph for 2nd cell text
                Paragraph para = new Paragraph(doc.PageNumber.ToString(), footer);

                //create new instance of cell to hold the text
                PdfPCell cell = new PdfPCell(para);

                //align the text to the right of the cell
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                //set border to 0
                cell.Border = 0;

                // add some padding to take away from the edge of the page
                cell.PaddingRight = 10;

                //add the cell to the table
                footerTbl.AddCell(cell);

                //write the rows out to the PDF output stream.
                footerTbl.WriteSelectedRows(0, -1, 0, (doc.BottomMargin + 5), writer.DirectContent);
            }
        }
    }
}