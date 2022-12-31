using OnRail;
using OnRail.Extensions.Map;
using OnRail.Extensions.OnSuccess;
using OnRail.Extensions.Try;
using TableToCode.Header;

namespace TableToCode.Program;

public class ProgramService : IProgram {
    private readonly IHeaderParser _headerParser;

    public ProgramService(IHeaderParser headerParser) {
        _headerParser = headerParser;
    }

    public Result Run() =>
        GetTableFromConsole("Input your header table: ")
            .OnSuccess(_headerParser.ParseHeader)
            .OnSuccessTee(result => Console.WriteLine( /* TODO: Remove*/))
            .Map();

    //TODO: Move to helper class
    private static Result<List<string>> GetTableFromConsole(string message) =>
        TryExtensions.Try(() => {
            Console.WriteLine(message);
            var tableRows = new List<string>();
            while (true) {
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    return tableRows;
                tableRows.Add(line);
            }
        });
}