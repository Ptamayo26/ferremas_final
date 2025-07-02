using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Ferremas.Api.Models;
using System;
using System.IO;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Elements;

namespace Ferremas.Api.Services
{
    public class FacturaPdfService
    {
        // Datos ficticios de la empresa Ferremas
        private const string EmpresaNombre = "FERREMAS S.A.";
        private const string EmpresaRut = "76.123.456-7";
        private const string EmpresaGiro = "Comercialización de materiales de construcción";
        private const string EmpresaDireccion = "Av. Principal 123, Santiago, Chile";
        private const string EmpresaTelefono = "+56 9 1234 5678";
        private const string EmpresaEmail = "contacto@ferremas.cl";

        private static TextStyle Titulo => TextStyle.Default.Size(18).Bold().FontColor(Colors.Grey.Darken3);
        private static TextStyle Subtitulo => TextStyle.Default.Size(12).SemiBold().FontColor(Colors.Grey.Darken2);
        private static TextStyle Normal => TextStyle.Default.Size(10).FontColor(Colors.Grey.Darken3);
        private static TextStyle TablaHeader => TextStyle.Default.Size(10).Bold().FontColor(Colors.White);
        private static TextStyle TablaCell => TextStyle.Default.Size(10).FontColor(Colors.Grey.Darken3);
        private static string ColorPrincipal = Colors.Grey.Lighten3;
        private static string ColorHeader = Colors.Blue.Medium;

        public byte[] GenerarBoleta(Pedido pedido, Cliente cliente)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Background(ColorPrincipal).Padding(10).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(EmpresaNombre).Style(Titulo);
                            col.Item().Text($"RUT: {EmpresaRut}").Style(Normal);
                            col.Item().Text(EmpresaGiro).Style(Normal);
                            col.Item().Text(EmpresaDireccion).Style(Normal);
                            col.Item().Text($"Tel: {EmpresaTelefono}").Style(Normal);
                            col.Item().Text($"Email: {EmpresaEmail}").Style(Normal);
                        });
                        // row.ConstantItem(100).Image("wwwroot/images/logo.png", ImageScaling.FitArea); // Si tienes logo
                    });

                    page.Content().Column(col =>
                    {
                        col.Item().PaddingVertical(10).Text($"BOLETA ELECTRÓNICA N° {pedido.Id:D8}").Style(Titulo).AlignCenter();
                        col.Item().Text($"Fecha: {pedido.FechaCreacion:dd/MM/yyyy HH:mm}").Style(Subtitulo);
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Cliente:").Style(Subtitulo);
                                c.Item().Text($"{cliente.Nombre} {cliente.Apellido}").Style(Normal);
                                c.Item().Text($"RUT: {cliente.Rut ?? "-"}").Style(Normal);
                                c.Item().Text($"Dirección: {pedido.DireccionEntrega ?? "-"}").Style(Normal);
                                c.Item().Text($"Correo: {cliente.CorreoElectronico ?? "-"}").Style(Normal);
                                c.Item().Text($"Teléfono: {cliente.Telefono ?? "-"}").Style(Normal);
                            });
                        });

                        col.Item().PaddingVertical(10).Element(BuildTablaProductos(pedido));

                        decimal subtotal = pedido.Detalles.Sum(x => x.Subtotal ?? 0);
                        decimal iva = Math.Round(subtotal * 0.19m);
                        decimal total = pedido.Total ?? subtotal + iva;

                        col.Item().PaddingTop(10).AlignRight().Column(tot =>
                        {
                            tot.Item().Text($"Subtotal: ${subtotal:N0}").Style(Normal);
                            tot.Item().Text($"IVA (19%): ${iva:N0}").Style(Normal);
                            tot.Item().Background(Colors.Grey.Lighten2).Padding(5).Text($"Total: ${total:N0}").Style(Titulo);
                        });
                    });

                    page.Footer().AlignCenter().PaddingTop(10).Text("Documento válido solo electrónicamente - www.ferremas.cl").Style(Normal);
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerarFactura(Pedido pedido, Cliente cliente, DatosFacturaEmpresa datosEmpresa)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Background(ColorPrincipal).Padding(10).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(EmpresaNombre).Style(Titulo);
                            col.Item().Text($"RUT: {EmpresaRut}").Style(Normal);
                            col.Item().Text(EmpresaGiro).Style(Normal);
                            col.Item().Text(EmpresaDireccion).Style(Normal);
                            col.Item().Text($"Tel: {EmpresaTelefono}").Style(Normal);
                            col.Item().Text($"Email: {EmpresaEmail}").Style(Normal);
                        });
                        // row.ConstantItem(100).Image("wwwroot/images/logo.png", ImageScaling.FitArea); // Si tienes logo
                    });

                    page.Content().Column(col =>
                    {
                        col.Item().PaddingVertical(10).Text($"FACTURA ELECTRÓNICA N° {pedido.Id:D8}").Style(Titulo).AlignCenter();
                        col.Item().Text($"Fecha: {pedido.FechaCreacion:dd/MM/yyyy HH:mm}").Style(Subtitulo);
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Razón Social: {datosEmpresa.RazonSocial}").Style(Normal);
                                c.Item().Text($"RUT Empresa: {datosEmpresa.Rut}").Style(Normal);
                                c.Item().Text($"Giro: {datosEmpresa.Giro}").Style(Normal);
                                c.Item().Text($"Dirección Empresa: {datosEmpresa.Direccion}").Style(Normal);
                                c.Item().Text($"Correo Cliente: {cliente.CorreoElectronico ?? "-"}").Style(Normal);
                                c.Item().Text($"Teléfono Cliente: {cliente.Telefono ?? "-"}").Style(Normal);
                            });
                        });

                        col.Item().PaddingVertical(10).Element(BuildTablaProductos(pedido));

                        decimal subtotal = pedido.Detalles.Sum(x => x.Subtotal ?? 0);
                        decimal iva = Math.Round(subtotal * 0.19m);
                        decimal total = pedido.Total ?? subtotal + iva;

                        col.Item().PaddingTop(10).AlignRight().Column(tot =>
                        {
                            tot.Item().Text($"Subtotal: ${subtotal:N0}").Style(Normal);
                            tot.Item().Text($"IVA (19%): ${iva:N0}").Style(Normal);
                            tot.Item().Background(Colors.Grey.Lighten2).Padding(5).Text($"Total: ${total:N0}").Style(Titulo);
                        });
                    });

                    page.Footer().AlignCenter().PaddingTop(10).Text("Documento válido solo electrónicamente - www.ferremas.cl").Style(Normal);
                });
            });

            return document.GeneratePdf();
        }

        private static Action<IContainer> BuildTablaProductos(Pedido pedido)
        {
            return tableContainer =>
            {
                tableContainer.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(ColorHeader).Padding(4).Text("Producto").Style(TablaHeader);
                        header.Cell().Background(ColorHeader).Padding(4).AlignCenter().Text("Cant.").Style(TablaHeader);
                        header.Cell().Background(ColorHeader).Padding(4).AlignRight().Text("P. Unitario").Style(TablaHeader);
                        header.Cell().Background(ColorHeader).Padding(4).AlignRight().Text("Subtotal").Style(TablaHeader);
                    });

                    foreach (var d in pedido.Detalles)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(d.Producto?.Nombre ?? "Producto").Style(TablaCell);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignCenter().Text(d.Cantidad?.ToString() ?? "").Style(TablaCell);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text($"${d.PrecioUnitario?.ToString("N0") ?? "0"}").Style(TablaCell);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text($"${d.Subtotal?.ToString("N0") ?? "0"}").Style(TablaCell);
                    }
                });
            };
        }
    }
} 