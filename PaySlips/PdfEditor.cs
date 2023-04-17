using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Path = System.IO.Path;

namespace Payslips
{
    internal class PdfEditor
    {
        public PdfEditor(string pathInput)
        {
            _pathInputInput = pathInput;
        }
        private string _pathInputInput { get; set; }
        static public string PathSave { get; set; } = string.Empty;

        private Regex _fullName = new Regex(@"\(+\d+\)");//Проверка страницы на наличие номера работника

        private void PersonPdfSave(PdfReader reader, string outputFilename, List<int> pageNumbers)
        {
            if (string.IsNullOrEmpty(PathSave))
            {
                PathSave = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "Расчётные листы")).ToString();
            }
            var document = new Document();
            using (PdfCopy pdfCopy = new PdfCopy(document, new FileStream(Path.Combine(PathSave, outputFilename + ".pdf"), FileMode.OpenOrCreate)))
            {
                document.Open();
                foreach (int pageNumber in pageNumbers)
                {
                    PdfImportedPage pdfImportedPage = pdfCopy.GetImportedPage(reader, pageNumber);
                    pdfCopy.AddPage(pdfImportedPage);
                }
                document.Dispose();
                pdfCopy.Close();
            }
                
        }
        public void DeleteDirectory(bool? delete)
        {
            if (delete.Value)
            {
                Directory.Delete(PathSave, true);
            }
        }
        public void Divide()
        {
            using (PdfReader reader = new PdfReader(_pathInputInput))
            {
                Regex fullName = new Regex(@"\n(.*?)(\s\()");
                List<int> Pages = new List<int>();//Для тех, у кого больше 1 страницы
                string currentText = string.Empty;
                string name = string.Empty;
                for (int i = 2; i <= reader.NumberOfPages; i++)
                {
                    currentText = PdfTextExtractor.GetTextFromPage(reader, i);
                    if (fullName.IsMatch(currentText))
                    {
                        if ((Pages.Count > 0 && name != string.Empty))
                        {
                            PersonPdfSave(reader, name, Pages);
                        }
                        Pages.Clear();
                        name = fullName.Match(currentText).Groups[1].Value;
                        Pages.Add(i);
                    }
                    else
                    {
                        Pages.Add(i);
                    }
                    if (i == reader.NumberOfPages)
                    {
                        PersonPdfSave(reader, name, Pages);
                    }
                }
            }
        }
    }
}
