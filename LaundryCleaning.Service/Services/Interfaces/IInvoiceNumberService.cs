namespace LaundryCleaning.Service.Services.Interfaces
{
    public interface IInvoiceNumberService
    {
        Task<string> GenerateInvoiceNumberAsync(string code, CancellationToken cancellationToken);
    }
}
