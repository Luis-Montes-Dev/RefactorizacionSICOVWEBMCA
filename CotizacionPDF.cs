
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SICOVWEB_MCA.Models;
using System.Globalization;

namespace SICOVWEB_MCA
{
    public class CotizacionPDF : IDocument
    {
        public Cotizacion Cotizacion { get; set; }

        public CotizacionPDF(Cotizacion cotizacion)
        {
            Cotizacion = cotizacion;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            var total = Cotizacion.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
            var iva = total * 0.16M;
            var totalConIva = total + iva;

            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Column(col =>
                    {
                        col.Item().Text("MC Automatización").FontSize(18).Bold();
                        col.Item().Text($"Cotización No: {Cotizacion.IdCotizacion}").FontSize(14);
                        col.Item().Text($"Fecha: {Cotizacion.Fecha:dd/MM/yyyy}");
                    });

                page.Content()
                    .Column(col =>
                    {
                        col.Spacing(10);
                        col.Item().Text($"Empresa: {Cotizacion.Cliente.Razon_Social}");
                        col.Item().Text($"Correo: {Cotizacion.Cliente.Correo}");
                        col.Item().Text($"At'n: {Cotizacion.Cliente.Correo}");
                        col.Item().Text($"Elaboró: {Cotizacion.Empleado.Nombre} {Cotizacion.Empleado.Apellido_Paterno} {Cotizacion.Empleado.Apellido_Materno}");

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30); // #
                                columns.ConstantColumn(50); // Unidad
                                columns.RelativeColumn();   // Descripción
                                columns.ConstantColumn(50); // Cantidad
                                columns.ConstantColumn(80); // Precio Unitario
                                columns.ConstantColumn(80); // Importe Final
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("PART").Bold();
                                header.Cell().Element(CellStyle).Text("UNIDAD").Bold();
                                header.Cell().Element(CellStyle).Text("DESCRIPCIÓN").Bold();
                                header.Cell().Element(CellStyle).Text("CANT").Bold();
                                header.Cell().Element(CellStyle).Text("PRECIO UNIT").Bold();
                                header.Cell().Element(CellStyle).Text("IMPORTE FINAL").Bold();
                            });

                            int i = 1;
                            foreach (var item in Cotizacion.Detalles)
                            {
                                table.Cell().Element(CellStyle).Text(i.ToString());
                                table.Cell().Element(CellStyle).Text(item.Producto.Unidad);
                                table.Cell().Element(CellStyle).Text(item.Producto.Descripcion);
                                table.Cell().Element(CellStyle).Text(item.Cantidad.ToString());
                                table.Cell().Element(CellStyle).Text($"{item.PrecioUnitario:N2}");
                                table.Cell().Element(CellStyle).Text($"{item.PrecioTotal:N2}");
                                i++;
                            }

                            static IContainer CellStyle(IContainer container) =>
                                container.Padding(3).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                        });

                        col.Item().Text($"Subtotal: ${total:N2}").AlignRight();
                        col.Item().Text($"IVA 16%: ${iva:N2}").AlignRight();
                        col.Item().Text($"Total: ${totalConIva:N2}").Bold().FontSize(12).AlignRight();
                    });

                page.Footer()
                    .AlignCenter()
                    .Text("Gracias por su preferencia - MC Automatización").FontSize(9).Italic();
            });
        }
    }
}



