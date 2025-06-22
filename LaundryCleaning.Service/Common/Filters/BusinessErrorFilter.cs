using LaundryCleaning.Service.Common.Exceptions;
using Serilog;

namespace LaundryCleaning.Service.Common.Filters
{
    public class BusinessErrorFilter : IErrorFilter
    {
        private readonly ILogger<BusinessErrorFilter> _logger;

        public BusinessErrorFilter(ILogger<BusinessErrorFilter> logger)
        {
            _logger = logger;
        }
        public IError OnError(IError error)
        {
            if (error.Exception is BusinessLogicException ex)
            {
                _logger.LogWarning("Business logic error: {Message}", ex.Message);
                Log.Error("Business logic error: {Message}", ex.Message);

                return ErrorBuilder.New()
                    .SetMessage(ex.Message)
                    .SetCode("BUSINESS_ERROR")
                    .SetExtension("timestamp", DateTime.UtcNow)
                    .ClearLocations()
                    .Build();
            }

            // Error selain BusinessLogicException tetap diproses default
            return error;
        }
    }
}
