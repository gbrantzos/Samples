namespace MinimalAPI;

public abstract class Entity
{
    public long ID { get; set; }
}

public class Game : Entity
{
    public string Code { get; set; } = String.Empty;
    public List<GameDetail> Details { get; set; } = new List<GameDetail>();
}

public class GameDetail : Entity
{
    public string Translation { get; set; } = String.Empty;
    public string Rules { get; set; } = String.Empty;
}

public abstract class ViewModel
{
    public long ID { get; set; }
}

public class GameDetailViewModel : ViewModel
{
    public string Translation { get; set; } = String.Empty;
    public string Rules { get; set; } = String.Empty;
}

/// <summary>
/// Reposistory
/// ---
/// 
/// Talking about SP supported repo for a complex entity
/// How should a repository work?
/// - Save the main entity (insert or update based on ID)
/// - Process details: Loop through details and
/// 	- Add new ones (ID == 0)
/// 	- Apply changes (ID != 0 and found on existing details collection)
/// 	- Delete (entity found on existing but MISSING from request - view model)
/// - For this to work we need a way to "locate" the existing row
/// 
/// DetailProcessor<TDetailEntity, TRequestModel>
/// 	Locator: Func<IEnumerable<TDetailsEntity>, IEnumerable<TRequestModel>, TDetailEntity>
/// 	NewEntityFactory: Func<TRequestModel, TEntityModel>
/// 	ApplyChanges: Action<TExisting, TRequestModel>
/// 	ApplyDeletes: Action ????
/// </summary>
/// <typeparam name="TMaster"></typeparam>
/// <typeparam name="TDetail"></typeparam>
/// <typeparam name="TDetailViewModel"></typeparam>
public class DetailProcessor<TMaster, TDetail, TDetailViewModel>
    where TMaster : Entity
    where TDetail : Entity
    where TDetailViewModel : ViewModel
{
    private readonly Func<TDetailViewModel, TMaster, TDetail?> _locateExisting;
    private readonly Func<IEnumerable<TDetailViewModel>, TMaster, IEnumerable<TDetail>> _locateMissing;
    private readonly Action<TDetailViewModel, TMaster> _processNew;
    private readonly Action<TDetailViewModel, TDetail> _processExisting;
    private readonly Action<TDetail, TMaster> _processMissing;

    public DetailProcessor(
        Func<TDetailViewModel, TMaster, TDetail?> locateExisting,
        Func<IEnumerable<TDetailViewModel>, TMaster, IEnumerable<TDetail>> locateMissing,
        Action<TDetailViewModel, TMaster> processNew, 
        Action<TDetailViewModel, TDetail> processExisting,
        Action<TDetail, TMaster> processMissing)
    {
        _locateExisting = locateExisting;
        _locateMissing = locateMissing;
        _processNew = processNew;
        _processExisting = processExisting;
        _processMissing = processMissing;
    }

    public void ApplyChanges(TMaster masterEntity, IReadOnlyCollection<TDetailViewModel> detailChanges)
    {
        foreach (var viewModel in detailChanges)
        {
            var existing = _locateExisting(viewModel, masterEntity);
            if (existing is null)
            {
                _processNew(viewModel, masterEntity);
            }
            else
            {
                _processExisting(viewModel, existing);
            }
        }

        foreach (var missing in _locateMissing(detailChanges, masterEntity))
        {
            _processMissing(missing, masterEntity);
        }
    }
}