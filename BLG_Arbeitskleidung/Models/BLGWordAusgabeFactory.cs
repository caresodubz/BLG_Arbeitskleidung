using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using Arbeitsbekleidung.Models.Models;

namespace BLG_Arbeitskleidung.Models {
    public static class BLGWordAusgabeFactory {
        public static string CreateOutputFile(string name, string personalNr, Bestand[] bestände) {
            string tempPath = Path.GetTempFileName();
            string filePath = Path.ChangeExtension(tempPath, ".docx");
            File.Copy("./Lieferschein_Arbeitskleidung.docx", filePath, overwrite: true);

            using WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, true);
            Body body = wordDoc.MainDocumentPart!.Document.Body!;

            Table titelTable = body.Elements<Table>().ElementAt(0);
            TableRow titelRow = (TableRow)titelTable.ElementAt(2);

            TableCell nameCell = (TableCell)titelRow.ElementAt(3);
            TableCell personalNrCell = (TableCell)titelRow.ElementAt(6);

            SetTableCellText(nameCell, name);
            SetTableCellText(personalNrCell, personalNr);

            Table bestandTable = body.Elements<Table>().ElementAt(1);

            for(int i = 0; i < bestände.Length; i++) {
                TableRow tableRow = (TableRow)bestandTable.ChildElements[i + 4];

                TableCell kleidungCell = (TableCell)tableRow.ChildElements[1];
                TableCell mengeCell = (TableCell)tableRow.ChildElements[2];
                TableCell größeCell = (TableCell)tableRow.ChildElements[3];
                TableCell ausgabeAmCell = (TableCell)tableRow.ChildElements[4];

                SetTableCellText(kleidungCell, bestände[i].Arbeitskleidung!.DisplayArtikelName);
                SetTableCellText(mengeCell, bestände[i].menge.ToString());
                SetTableCellText(größeCell, bestände[i].Arbeitskleidung!.groesse);
                SetTableCellText(ausgabeAmCell, DateTime.Now.ToString("dd.MM.yyyy"));
            }

            wordDoc.MainDocumentPart.Document.Save();
            return filePath;
        }

        private static void SetTableCellText(TableCell cell, string textContent) {
            Paragraph? paragraph = cell.Elements<Paragraph>().FirstOrDefault();
            paragraph ??= cell.AppendChild(new Paragraph());

            Run? run = paragraph.Elements<Run>().FirstOrDefault();
            run ??= paragraph.AppendChild(new Run());
            Text? text = run.Elements<Text>().FirstOrDefault();

            if(text == null) {
                run.AppendChild(new Text(textContent));
            } else {
                text.Text = textContent;
            }
        }
    }
}