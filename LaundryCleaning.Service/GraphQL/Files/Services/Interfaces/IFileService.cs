using LaundryCleaning.Service.Common.Inputs;
using LaundryCleaning.Service.Common.Response;

namespace LaundryCleaning.Service.GraphQL.Files.Services.Interfaces
{
    public interface IFileService
    {
        Task<GlobalUploadFileResponseCustomModel> UploadFile(GlobalUploadFileInput input, CancellationToken cancellationToken);
        Task<GlobalUploadFileResponseCustomModel> GenerateExcelFile(CancellationToken cancellationToken);
        Task<List<List<string>>> UploadExcelAndReadRows(GlobalUploadFileInput input, CancellationToken cancellationToken);
        Task<GlobalUploadFileResponseCustomModel> GenerateInvoice(CancellationToken cancellationToken);
    }
}
