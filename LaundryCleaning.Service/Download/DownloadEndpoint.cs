using Microsoft.AspNetCore.Mvc;

namespace LaundryCleaning.Service.Download
{
    public static class DownloadEndpoint
    {
        public static void MapDownloadEndpoint(this WebApplication app)
        {
            app.MapGet("/download", async (HttpContext context, [FromQuery] string token, [FromServices] SecureDownloadHelper helper) =>
            {
                if (!helper.TryValidateToken(token, out var fileName))
                    return Results.BadRequest("Invalid or expired token.");

                var storagePath = System.IO.Path.Combine("Storages","temp");
                var filePath = System.IO.Path.Combine(storagePath, fileName);
                if (!File.Exists(filePath))
                    return Results.NotFound("File not found.");

                var contentType = GetContentType(fileName);
                var fileBytes = await File.ReadAllBytesAsync(filePath);
                return Results.File(fileBytes, contentType, System.IO.Path.GetFileName(fileName));
            });

            app.MapGet("/download-invoice", async (HttpContext context, [FromQuery] string token, [FromServices] SecureDownloadHelper helper) =>
            {
                if (!helper.TryValidateToken(token, out var fileName))
                    return Results.BadRequest("Invalid or expired token.");

                var storagePath = System.IO.Path.Combine("Storages", "Invoices");
                var filePath = System.IO.Path.Combine(storagePath, fileName);
                if (!File.Exists(filePath))
                    return Results.NotFound("File not found.");

                var contentType = GetContentType(fileName);
                var fileBytes = await File.ReadAllBytesAsync(filePath);
                return Results.File(fileBytes, contentType, System.IO.Path.GetFileName(fileName));
            });
        }

        private static string GetContentType(string fileName)
        {
            var ext = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xls" => "application/vnd.ms-excel",
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }
    }

}
