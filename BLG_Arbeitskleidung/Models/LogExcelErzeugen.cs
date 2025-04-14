using Arbeitsbekleidung.Database.Database;
using Arbeitsbekleidung.Models.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Windows.Forms;

namespace BLG_Arbeitskleidung.Models {
    class LogExcelErzeugen {
        public BLGBestandDbContext Database { get; set; } = new();
        private List<Log> LogListe { get; set; } = [];

        public LogExcelErzeugen() {
            LogListe = Database.Log.ToList();
        }

        public void LogExcel() {
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
                        Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Log-Liste" };
                        sheets.Append(sheet);

                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>()!;

                        Columns columns = new Columns(
                            new Column { Min = 1, Max = 1, Width = 10, CustomWidth = true },
                            new Column { Min = 2, Max = 2, Width = 15, CustomWidth = true },
                            new Column { Min = 3, Max = 3, Width = 15, CustomWidth = true },
                            new Column { Min = 4, Max = 4, Width = 15, CustomWidth = true },
                            new Column { Min = 5, Max = 5, Width = 15, CustomWidth = true },
                            new Column { Min = 6, Max = 6, Width = 15, CustomWidth = true },
                            new Column { Min = 7, Max = 7, Width = 15, CustomWidth = true },
                            new Column { Min = 8, Max = 8, Width = 30, CustomWidth = true },
                            new Column { Min = 9, Max = 9, Width = 30, CustomWidth = true }
                            );
                        worksheetPart.Worksheet.InsertAt(columns, 0);

                        Row headerRow = new();
                        headerRow.Append(
                                new Cell() { CellValue = new CellValue("ID"), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue("Funktion"), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue("Bearbeiter"), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue("Log-Datum"), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue("Artikel"), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue("Stückzahl"), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue("Größe"), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue("Personalnummer (Empfänger)"), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue("Bemerkung"), DataType = CellValues.String }
                        );
                        sheetData.Append(headerRow);

                        foreach(var item in LogListe) {
                            Row row = new Row();
                            row.Append(
                                new Cell() { CellValue = new CellValue(item.log_id.ToString()), DataType = CellValues.Number },
                                new Cell() { CellValue = new CellValue(item.log_funktion!), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(item.bearbeiter!), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(item.log_date.ToString()), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(item.artikel!), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(item.log_stueckzahl.ToString()), DataType = CellValues.Number },
                                new Cell() { CellValue = new CellValue(item.log_groesse!), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(item.person_nr!.ToString()), DataType = CellValues.String },
                                new Cell() { CellValue = new CellValue(item.log_bemerkung!), DataType = CellValues.String }
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
