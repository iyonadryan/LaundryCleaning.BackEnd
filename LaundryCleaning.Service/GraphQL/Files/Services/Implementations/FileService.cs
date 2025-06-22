using DinkToPdf;
using DinkToPdf.Contracts;
using LaundryCleaning.Service.Common.Inputs;
using LaundryCleaning.Service.Common.Models;
using LaundryCleaning.Service.Common.Response;
using LaundryCleaning.Service.Data;
using LaundryCleaning.Service.Download;
using LaundryCleaning.Service.GraphQL.Files.Services.Interfaces;
using LaundryCleaning.Service.GraphQL.Users.Services.Implementations;
using LaundryCleaning.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.HPSF;
using NPOI.XSSF.UserModel;
using RazorLight;
using System;
using System.Text;

namespace LaundryCleaning.Service.GraphQL.Files.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<FileService> _logger;
        private readonly SecureDownloadHelper _secureDownloadHelper;
        private readonly IConverter _converter;
        private readonly IInvoiceNumberService _invoiceNumberService;
        public FileService(
            ApplicationDbContext dbContext
            ,IHttpContextAccessor httpContextAccessor
            ,ILogger<FileService> logger
            ,SecureDownloadHelper secureDownloadHelper
            ,IConverter converter
            ,IInvoiceNumberService invoiceNumberService) 
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _secureDownloadHelper = secureDownloadHelper;
            _converter = converter;
            _invoiceNumberService = invoiceNumberService;
        }

        public async Task<GlobalUploadFileResponseCustomModel> UploadFile(GlobalUploadFileInput input, CancellationToken cancellationToken)
        {
            var file = input.File;

            var ext = System.IO.Path.GetExtension(file.Name);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var random = GenerateRandomString(10);
            var newFileName = $"{timestamp}_{random}{ext}";

            var filePath = System.IO.Path.Combine("wwwroot","Uploads", newFileName);

            // Buat folder jika belum ada
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath)!);

            await using var stream = File.Create(filePath);
            await file.CopyToAsync(stream, cancellationToken);

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var fileUrl = $"{baseUrl}/Uploads/{newFileName}";

            return new GlobalUploadFileResponseCustomModel() { 
                Success = true,
                Url = fileUrl
            };
        }

        public async Task<GlobalUploadFileResponseCustomModel> GenerateExcelFile(CancellationToken cancellationToken) 
        {
            // Create new workbook
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet("Sheet1");

            // Add header
            var headerRow = sheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("ID");
            headerRow.CreateCell(1).SetCellValue("Name");
            headerRow.CreateCell(2).SetCellValue("Email");

            // Add data
            var data = await (from u in _dbContext.Users
                              select u).ToListAsync(cancellationToken);

            int startIndex = 1;
            foreach (var user in data)
            {
                var row = sheet.CreateRow(startIndex);
                row.CreateCell(0).SetCellValue(startIndex);
                row.CreateCell(1).SetCellValue(user.Username);
                row.CreateCell(2).SetCellValue(user.Email);
                startIndex++;
            }

            // Generate file name
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var random = GenerateRandomString(10);
            var newFileName = $"{timestamp}_{random}.xlsx";

            var filePath = System.IO.Path.Combine("Storages", "temp", newFileName);

            // Buat folder jika belum ada
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath)!);

            using var ms = new MemoryStream();
            workbook.Write(ms);
            await File.WriteAllBytesAsync(filePath, ms.ToArray(),cancellationToken);

            var token = _secureDownloadHelper.GenerateToken(newFileName, TimeSpan.FromMinutes(5));

            var request = _httpContextAccessor.HttpContext!.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var url = $"{baseUrl}/download?token={Uri.EscapeDataString(token)}";

            return new GlobalUploadFileResponseCustomModel()
            {
                Success = true,
                Url = url
            };
        }

        public async Task<List<List<string>>> UploadExcelAndReadRows(GlobalUploadFileInput input, CancellationToken cancellationToken)
        {
            // Simpan ke memori (tidak perlu disimpan ke disk jika hanya membaca)
            using var stream = input.File.OpenReadStream();

            var workbook = new XSSFWorkbook(stream);
            var sheet = workbook.GetSheetAt(0); // Ambil sheet pertama
            var rows = new List<List<string>>();

            for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null) continue;

                var rowData = new List<string>();
                for (int j = 0; j < row.LastCellNum; j++)
                {
                    var cell = row.GetCell(j);
                    rowData.Add(cell?.ToString() ?? string.Empty);
                }

                rows.Add(rowData);
            }

            return rows;
        }

        public async Task<GlobalUploadFileResponseCustomModel> GenerateInvoice(CancellationToken cancellationToken)
        {
            string folderTemplatePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Storages", "Templates");

            var engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(folderTemplatePath)
                .UseMemoryCachingProvider()
                .Build();

            var data = await (from u in _dbContext.Users
                              select u).Take(5).ToListAsync(cancellationToken);
            
            decimal subTotal = 0;

            var items = new List<InvoiceItem>();
            foreach (var row in data)
            {
                decimal price = 100000;

                Random random = new Random();
                int number = random.Next(1, 4);

                var newItem = new InvoiceItem()
                { 
                    Description = row.Username,
                    Quantity = number,
                    UnitPrice = price,
                    TotalPrice = price * number
                };

                items.Add(newItem);

                subTotal = subTotal + (price * number);
            }

            decimal invoiceTax = (subTotal * 12) / 100;

            var invoiceNumber = await _invoiceNumberService.GenerateInvoiceNumberAsync("ABCDE", cancellationToken);

            var invoice = new InvoiceModel()
            { 
                Logo = "http://localhost:5292/Logo/IYON_LOGO_black.png",
                InvoiceNumber = invoiceNumber,
                InvoiceDate = DateTime.Now,
                InvoicePaymentDueDate = DateTime.Now.AddMonths(1),
                Items = items,
                SubTotal = subTotal,
                InvoiceTax = invoiceTax,
                Total = subTotal + invoiceTax,
                PaymentDetail = new InvoicePaymentDetail() 
                { 
                    BankName = "Bank Central Asia (BCA)",
                    BankAccountNumber = "1234567890",
                    BankAccountName = "PT IYON",
                    BankVirtualAccount= "8810 1234 5678 9012"
                }
            };

            var htmlContent = await engine.CompileRenderAsync("invoice-template.cshtml", invoice);

            // Convert to PDF
            var pdfDoc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4
            },
                        Objects = {
                new ObjectSettings() {
                    HtmlContent = htmlContent
                }
            }
            };

            var pdf = _converter.Convert(pdfDoc);

            string invoiceFolder = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Storages", "Invoices");

            string fileName = $"{invoiceNumber}.pdf";
            string filePath = System.IO.Path.Combine(invoiceFolder, fileName);

            // Buat folder jika belum ada
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath)!);

            // Simpan file PDF ke folder temp
            System.IO.File.WriteAllBytes(filePath, pdf);

            var token = _secureDownloadHelper.GenerateToken(filePath, TimeSpan.FromMinutes(5));

            var request = _httpContextAccessor.HttpContext!.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var url = $"{baseUrl}/download-invoice?token={Uri.EscapeDataString(token)}";

            return new GlobalUploadFileResponseCustomModel()
            {
                Success = true,
                Url = url
            };
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
