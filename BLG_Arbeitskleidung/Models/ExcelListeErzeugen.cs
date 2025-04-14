using Arbeitsbekleidung.Models.Models;
using Arbeitsbekleidung.Database.Database;
using System.Windows.Forms;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.EntityFrameworkCore;


namespace BLG_Arbeitskleidung.Models {
    class ExcelListeErzeugen {
        public BLGBestandDbContext Database { get; set; } = new();
        private List<Arbeitskleidung> BanfListe { get; set; } = [];

        public ExcelListeErzeugen() {
            BanfListe = Database.Arbeitskleidung.Include(x => x.Bestände).ToList().Where(x => x.IsMeldegrenzeÜberschritten == true || x.GesamtBestand == 0).ToList();
        }

        public void ErstelleExcelListe() {
            try {
                SaveFileDialog saveFileDialog = new() {
                    Filter = "Excel Dokumente (.xlsx)|*.xlsx|Alle Dateien (*.*)|*.*",
                    Title = "Speichern unter..."
                };

                DialogResult result = saveFileDialog.ShowDialog();
                if(result == DialogResult.OK) {
                    FileInfo file = new(saveFileDialog.FileName);

                    if(file.Exists) {
                        file.Delete();
                        file = new FileInfo(saveFileDialog.FileName);
                    }

                    using(SpreadsheetDocument document = SpreadsheetDocument.Create(
                        saveFileDialog.FileName,
                        DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook)) {

                        WorkbookPart workbookPart = document.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();
                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet(new SheetData());

                        Sheets sheets = document.WorkbookPart!.Workbook.AppendChild(new Sheets());
                        Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Arbeitsbekleidung_Banf" };
                        sheets.Append(sheet);

                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>()!;

                        Columns columns = new Columns(
                            new Column { Min = 1, Max = 1, Width = 10, CustomWidth = true },
                            new Column { Min = 2, Max = 2, Width = 80, CustomWidth = true },
                            new Column { Min = 3, Max = 3, Width = 40, CustomWidth = true },
                            new Column { Min = 4, Max = 4, Width = 10, CustomWidth = true },
                            new Column { Min = 5, Max = 5, Width = 20, CustomWidth = true }
                            );
                        worksheetPart.Worksheet.InsertAt(columns, 0);

                        Row headerRow = new();
                        headerRow.Append(
                                new Cell() { CellValue = new CellValue("ID"), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue("Artikelname"), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue("Artikelnummer"), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue("Größe"), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue("Gesamtbestand"), DataType = CellValues.String }
                        );
                        sheetData.Append(headerRow);

                        foreach(var item in BanfListe) {
                            Row row = new Row();
                            row.Append(
                                new Cell() { CellValue = new CellValue(item.artikel_id.ToString()), DataType = CellValues.Number },
                                new Cell() { CellValue = new CellValue(item.artikel_name), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(item.artikel_nr), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(item.groesse), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(item.GesamtBestand.ToString()), DataType = CellValues.Number }
                            );
                            sheetData.Append(row);
                        }

                        workbookPart.Workbook.Save();

                        MessageBox.Show(
                            "Liste erfolgreich erstellt!",
                            "Information",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                            );
                    }
                }
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}