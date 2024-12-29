using MinimalAPI;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.MapGet("/", () => "Hello!");
app.MapDummy();

app.Run();

// Result<int> result = new Error();

// using MinimalAPI;
//
// var result = Parser.Parse();
//
// foreach (var kvp in result)
//     Console.WriteLine(kvp);
//
// var configuration = new ConfigurationManager()
//     .AddInMemoryCollection(result!)
//     .Build();
//
// var setup = configuration.GetSection("SafeServersSetup").Get<SafeServersSetup>();
// Console.ReadLine();
/*
using MinimalAPI;

var aGame = new Game
{
    ID = 0,
    Code = "New Game",
    Details = new List<GameDetail>()
    {
        new GameDetail
        {
            ID = 3,
            Translation = "Καινούργιο Παιχνίδι",
            Rules = "Χωρις κανόνες"
        }
    }
};

var proc = new DetailProcessor<Game, GameDetail, GameDetailViewModel>(
    locateExisting: (viewModel, game) =>
    {
        return game.Details.FirstOrDefault(d => d.ID == viewModel.ID);
    },
    locateMissing: (viewModelItems, game) =>
    {
        var viewModelIDs = viewModelItems.Select(m => m.ID).ToList();
        return game.Details.Where(d => !viewModelIDs.Contains(d.ID));
    },
    processNew: (viewModel, game) =>
    {
        var gameDetail = new GameDetail()
        {
            Rules = viewModel.Rules,
            Translation = viewModel.Translation
        };
        game.Details.Add(gameDetail);
        // Call the needed SP
    },
    processExisting: (viewModel, detail) =>
    {
        viewModel.Translation = detail.Translation;
        viewModel.Rules = detail.Rules;
        // Call the needed SP
    },
    processMissing: (detail, game) =>
    {
        game.Details.Remove(detail);
        // Call the needed SP
    }
);


proc.ApplyChanges(aGame,
    new List<GameDetailViewModel>()
    {
        new() { Rules = "No rules", Translation = "Game1" },
        new() { ID = 3, Rules = "No rules", Translation = "Game1" }
    });
*/