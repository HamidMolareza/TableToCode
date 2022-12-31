using Microsoft.Extensions.Logging;
using OnRail;
using OnRail.Extensions.Map;
using OnRail.Extensions.OnSuccess;
using OnRail.Extensions.Try;
using TableToCode.DefinitionTable;

namespace TableToCode.Program;

public class ProgramService : IProgram {
    private readonly IDefinitionTable _definitionTable;
    private readonly ILogger<ProgramService> _logger;

    public ProgramService(ILogger<ProgramService> logger, IDefinitionTable definitionTable) {
        _definitionTable = definitionTable;
        _logger = logger;
    }

    public Result Run() =>
        GetTableFromConsole("Input your header table: ")
            .OnSuccess(_definitionTable.ParseHeader)
            .OnSuccessTee(result => _logger.LogInformation("This is a test log")) //TODO: Remove
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