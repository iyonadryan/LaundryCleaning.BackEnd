using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace LaundryCleaning.Service.Data
{
    public class GenericCsvSeeder
    {
        private readonly ApplicationDbContext _context;

        public GenericCsvSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public void SeedFromCsv<T>(string filePath) where T : class
        {
            if (_context.Set<T>().Any()) return; // ignore duplicate

            // For nullable column
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,         // key
                HeaderValidated = null,           // not must include all field
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            var records = csv.GetRecords<T>().ToList();
            _context.Set<T>().AddRange(records);
            _context.SaveChanges();
        }
    }
}
