using System.Security.Cryptography;
using System.Text;

namespace LicenseGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            var options = ParseArguments(args);
            if (options == null)
            {
                ShowHelp();
                return;
            }

            try
            {
                var licenseKey = GenerateLicenseKey(options);
                Console.WriteLine($"Generated License Key: {licenseKey}");
                
                if (!string.IsNullOrEmpty(options.Email))
                {
                    Console.WriteLine($"Customer Email: {options.Email}");
                }
                
                Console.WriteLine($"License Type: {options.Type}");
                Console.WriteLine($"Duration: {options.Years} year(s)");
                Console.WriteLine($"Expiry Date: {DateTime.UtcNow.AddYears(options.Years):yyyy-MM-dd}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating license key: {ex.Message}");
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine("Optiviera License Key Generator");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  --email <email>     Customer email address");
            Console.WriteLine("  --type <type>       License type (Trial, Full)");
            Console.WriteLine("  --years <years>      Duration in years");
            Console.WriteLine("  --help               Show this help");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("  --email customer@example.com --type Full --years 1");
        }

        static LicenseOptions? ParseArguments(string[] args)
        {
            var options = new LicenseOptions();
            
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "--email":
                        if (i + 1 < args.Length)
                            options.Email = args[++i];
                        break;
                    case "--type":
                        if (i + 1 < args.Length)
                            options.Type = args[++i];
                        break;
                    case "--years":
                        if (i + 1 < args.Length && int.TryParse(args[++i], out int years))
                            options.Years = years;
                        break;
                    case "--help":
                        return null;
                }
            }

            // Set defaults
            if (string.IsNullOrEmpty(options.Type))
                options.Type = "Full";
            if (options.Years <= 0)
                options.Years = 1;

            return options;
        }

        static string GenerateLicenseKey(LicenseOptions options)
        {
            // Format: OPTV-XXXX-XXXX-XXXX-XXXX
            var productId = "OPTV";
            var data = $"{options.Type}-{options.Years}-{DateTime.UtcNow:yyyyMMdd}";
            var checksum = CalculateChecksum(data);
            
            // Encode data (simplified encoding)
            var encodedData = EncodeData(data);
            
            return $"{productId}-{encodedData}-{checksum}";
        }

        static string EncodeData(string data)
        {
            // Simple encoding - in production, use proper encryption
            var bytes = Encoding.UTF8.GetBytes(data);
            var encoded = Convert.ToBase64String(bytes)
                .Replace("+", "A")
                .Replace("/", "B")
                .Replace("=", "C");
            
            // Pad to 12 characters
            return encoded.PadRight(12, '0')[..12];
        }

        static string CalculateChecksum(string data)
        {
            var sum = data.Sum(c => c);
            return (sum % 10000).ToString("D4");
        }
    }

    class LicenseOptions
    {
        public string Email { get; set; } = string.Empty;
        public string Type { get; set; } = "Full";
        public int Years { get; set; } = 1;
    }
}
