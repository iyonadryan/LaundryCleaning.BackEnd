using LaundryCleaning.Service.Common.Exceptions;
using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.Data;
using LaundryCleaning.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaundryCleaning.Service.Services.Implementations
{
    public class InvoiceNumberService : IInvoiceNumberService
    {
        private readonly ApplicationDbContext _dbContext;

        public InvoiceNumberService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GenerateInvoiceNumberAsync(string code, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code) || code.Length > 20)
                throw new BusinessLogicException("Code cannot be empty and max 20 characters.");

            var tracker = await _dbContext.InvoiceNumberTrackers
                .FirstOrDefaultAsync(t => t.Code == code, cancellationToken);

            if (tracker == null)
            {
                tracker = new InvoiceNumberTracker
                {
                    Code = code,
                    LastNumber = 1
                };
                _dbContext.InvoiceNumberTrackers.Add(tracker);
            }
            else
            {
                tracker.LastNumber++;
                if (tracker.LastNumber > 99999)
                {
                    // Reset Number
                    tracker.LastNumber = 1;

                    _dbContext.InvoiceNumberTrackers.Update(tracker);
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            // Format : {CODE}-I-{Date : MMYY}{Number: RRRRR}
            var now = DateTime.Now;
            var monthYear = now.ToString("MMyy");
            var runningNumber = tracker.LastNumber.ToString("D5"); // Format 5 digit with leading zero
            return $"{code}-I-{monthYear}{runningNumber}";
        }
    }
}
