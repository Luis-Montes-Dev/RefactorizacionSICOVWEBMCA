

using AspNetCoreGeneratedDocument;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SICOVWEB_MCA.Models;
using System.Globalization;
using static QuestPDF.Helpers.Colors;

namespace SICOVWEB_MCA.PDFDocs
{


    public class VentaDocumento : IDocument
    {
        private readonly Venta _venta;
        private readonly Contacto_cliente? _contacto;
        private readonly Promocion? _promocion;
        private readonly ApplicationDbContext _dbContext;

        public VentaDocumento(Venta venta, Contacto_cliente? contacto, Promocion? promocion) // Constructor
        {
            _venta = venta;
            _contacto = contacto;
            _promocion = promocion;
        }



        public DocumentMetadata GetMetadata() => DocumentMetadata.Default; // Metadatos del documento

        public void Compose(IDocumentContainer container) // Composición del documento
        {
            var total = Math.Round(_venta.DetalleVentas.Sum(d => d.Cantidad * d.PrecioUnitario), 2);
            var iva = Math.Round(total * 0.16M, 2);
            var totalConIVA = total + iva;

            // Diseño de la página
            container.Page(page =>
            {
                page.Size(PageSizes.A4); // Tamaño de la página A4
                page.Margin(20); // Margen de 20 unidades
                page.DefaultTextStyle(x => x.FontSize(10)); // Estilo de texto por defecto
                page.PageColor(Colors.White); // Color de fondo blanco

                // Encabezado
                page.Header().Column(header =>
                {
                    header.Spacing(5); // Espaciado entre elementos del encabezado

                    // Logo
                    header.Item().AlignCenter().Width(550).MaxHeight(80).Image(System.IO.File.ReadAllBytes("wwwroot/images/Logos3.jpg"));

                    // Información de la cotización
                    header.Item().Text($"NOTA DE VENTA: {_venta.Id_venta}")
                                 .FontSize(16).Bold().AlignCenter();

                    header.Item().Text($"Fecha: {_venta.Fecha_Venta:dd/MM/yyyy}");
                    header.Item().Text($"Empresa: {_venta.Cliente.Razon_Social}");
                    header.Item().Text($"Correo: {_venta.Cliente.Correo}");
                    header.Item().Text($"Teléfono: {_venta.Cliente.Telefono}");

                    // Mostrar nombre del contacto si está disponible
                    if (_contacto != null)
                        header.Item().Text($"Persona contacto: {_contacto.Nombre} {_contacto.Apellido_paterno} {_contacto.Apellido_materno}           Tel: {_contacto.Telefono}      Correo: {_contacto.Correo}");
                    else
                        header.Item().Text($"Persona contacto: Sin datos.");
                });

                page.Content().PaddingVertical(10).Column(content =>
                {
                    content.Spacing(10);

                    // Tabla
                    content.Item().Element(BuildTable);
                    
                    if (_promocion != null )
                    {
                        // Totales con descuento aplicado por promocion
                        content.Item().AlignRight().Column(col =>
                        {
                            col.Spacing(2);
                            col.Item().Text($"Precio Original: ${_venta.MontoTotal:N2}");
                            col.Item().Text($"Descuento: -{_promocion.Descuento:N2}% = ${_venta.MontoDescuento:N2}");
                            col.Item().Text($"Subtotal: ${_venta.Subtotal:N2}");
                            col.Item().Text($"IVA 16%: ${_venta.IVA:N2}");
                            col.Item().Text($"Total {_venta.TipoMoneda}: ${_venta.TotalConIva:N2}").Bold();
                        });
                    }
                    else
                    {

                        // Totales sin descuento
                        content.Item().AlignRight().Column(col =>
                            {
                                col.Spacing(2);
                                col.Item().Text($"Subtotal: ${_venta.Subtotal:N2}");
                                col.Item().Text($"IVA 16%: ${_venta.IVA:N2}");
                                col.Item().Text($"Total {_venta.TipoMoneda}: ${_venta.TotalConIva:N2}").Bold();
                            });
                        
                    }



                        // Datos finales
                        content.Item().PaddingTop(10).Column(col =>
                        {
                            col.Spacing(2);
                            col.Item().Text($"Fecha de entrega estimada: {_venta.Fecha_Entrega:dd/MM/yyyy}");
                            col.Item().Text($"Pago: {_venta.Cliente.Condicion_Pago}");
                            if (_promocion != null)
                            {
                                col.Item().Text($"Promoción aplicada: {_promocion.Nombre} - Descuento {_promocion.Descuento}%");
                            }
                            col.Item().Text($"Vendido por: {_venta.Empleado.Nombre} {_venta.Empleado.Apellido_Paterno} {_venta.Empleado.Apellido_Materno}");
                            col.Item().Text($"Tel: {_venta.Empleado.Telefono}");
                            col.Item().Text($"Correo: {_venta.Empleado.Correo}");
                            col.Item().Text($"Comentarios: {_venta.Observaciones}");

                        });
                });
            });
        }

        private void BuildTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {

                    columns.ConstantColumn(30);  // PART
                    columns.ConstantColumn(60);  // PRODUCTO
                    columns.ConstantColumn(40);  // MARCA
                    columns.RelativeColumn(2);   // DESCRIPCIÓN
                    columns.ConstantColumn(50);  // TIEMPO ENTREGA
                    columns.ConstantColumn(30);  // UNIDAD
                    columns.ConstantColumn(40);  // CANT                    
                    columns.ConstantColumn(60);  // P. UNITARIO
                    columns.ConstantColumn(60);  // SUBTOTAL
                });

                // Encabezado
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("PART").Bold();
                    header.Cell().Element(CellStyle).Text("PRODUCTO").Bold();
                    header.Cell().Element(CellStyle).Text("MARCA").Bold();
                    header.Cell().Element(CellStyle).Text("DESCRIPCIÓN").Bold();
                    header.Cell().Element(CellStyle).Text("TIEMPO ENTREGA").Bold();
                    header.Cell().Element(CellStyle).Text("UDS").Bold();
                    header.Cell().Element(CellStyle).Text("CANT").Bold();
                    header.Cell().Element(CellStyle).Text("P. UNITARIO").Bold();
                    header.Cell().Element(CellStyle).Text("SUBTOTAL").Bold();

                    static IContainer CellStyle(IContainer container) =>
                        container.Padding(1).Background(Colors.Grey.Lighten3).Border(1).AlignCenter();
                });

                // Filas dinámicas
                int index = 1;
                foreach (var item in _venta.DetalleVentas)
                {
                    if (item.Unidad != null && item.Unidad.Equals("Svc")) // Si es un servicio
                    {
                        //Datos editados del detalleVenta

                        table.Cell().Element(Cell).Text(index++.ToString());
                        table.Cell().Element(Cell).Text(item.Producto.Nombre ?? "");
                        table.Cell().Element(Cell).Text("N/A");
                        table.Cell().Element(Cell).Text(item.Descripcion ?? "");
                        table.Cell().Element(Cell).Text(item.TiempoEntrega ?? "");
                        table.Cell().Element(Cell).Text(item.Unidad ?? "");
                        table.Cell().Element(Cell).Text(item.Cantidad.ToString());
                        table.Cell().Element(Cell).Text($"${item.PrecioUnitario:N2}");
                        table.Cell().Element(Cell).Text($"${item.Subtotal:N2}");
                    }
                    //Datos tomados del producto
                    else
                    {
                        table.Cell().Element(Cell).Text(index++.ToString());
                        table.Cell().Element(Cell).Text(item.Producto?.Nombre ?? "");
                        table.Cell().Element(Cell).Text(item.Producto?.Marca ?? "");
                        table.Cell().Element(Cell).Text(item.Producto?.Descripcion ?? "");
                        table.Cell().Element(Cell).Text(item.TiempoEntrega ?? "");
                        table.Cell().Element(Cell).Text(item.Producto?.Unidad ?? "");
                        table.Cell().Element(Cell).Text(item.Cantidad.ToString());
                        table.Cell().Element(Cell).Text($"${item.Producto?.Precio_Venta:N2}");
                        table.Cell().Element(Cell).Text($"${item.Subtotal:N2}");
                    }

                }

                static IContainer Cell(IContainer container) =>
                    container.BorderBottom(1).Padding(5).AlignCenter();
            });
        }

    }

}