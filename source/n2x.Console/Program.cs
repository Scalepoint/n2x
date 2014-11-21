using n2x.Converter;
using NConsoler;

namespace n2x.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Consolery.Run(typeof(Program), args);
        }

        [Action]
        public static void Convert([Required]string path)
        {
            var convertersProvider = new DocumentConverterProvider();
            var converter = new N2XConverter(convertersProvider);

            var result = converter.ConvertSolution(path, (total, processed) => System.Console.Write("\r{0}%   ({1} of {2})", processed * 100/total, processed, total));

            System.Console.WriteLine(result ? "Success" : "Failure");
        }
    }
}
