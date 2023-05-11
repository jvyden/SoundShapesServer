using SoundShapesServer.Types.Levels;

namespace SoundShapesServer.Database;

public partial class GameDatabaseContext
{
    public IQueryable<DailyLevel> GetDailyLevelObjects(DailyLevelOrderType order, bool descending, DailyLevelFilters filters)
    {
        IQueryable<DailyLevel> orderedDailyLevels = order switch
        {
            DailyLevelOrderType.Date => DailyLevelObjectsOrderedByDate(descending),
            _ => DailyLevelObjectsOrderedByDate(descending)
        };
        
        IQueryable<DailyLevel> filteredDailyLevels = FilterDailyLevels(orderedDailyLevels, filters);
        return filteredDailyLevels;
    }

    private static IQueryable<DailyLevel> FilterDailyLevels(IQueryable<DailyLevel> dailyLevels, DailyLevelFilters filters)
    {
        IQueryable<DailyLevel> response = dailyLevels;

        if (filters.LastDate == true)
        {
            response = response
                .AsEnumerable()
                .GroupBy(d => d.Date)
                .OrderBy(g => g.Key.Date)
                .Last()
                .AsQueryable();
        }
        
        if (filters.Date != null)
        {
            response = response
                .AsEnumerable()
                .Where(d => d.Date.Date == filters.Date?.Date)
                .AsQueryable();
        }

        return response;
    }
    
    private IQueryable<DailyLevel> DailyLevelObjectsOrderedByDate(bool descending)
    {
        if (descending) return _realm.All<DailyLevel>().OrderByDescending(d => d.Date);
        return _realm.All<DailyLevel>().OrderBy(d => d.Date);
    }

    public DailyLevel? GetDailyLevelWithId(string id)
    {
        return _realm.All<DailyLevel>().FirstOrDefault(d => d.Id == id);
    }
    public DailyLevel CreateDailyLevel(GameLevel level, DateTimeOffset date)
    {
        DailyLevel dailyLevel = new()
        {
            Id = GenerateGuid(), 
            Level = level, 
            Date = date.Date
        };

        _realm.Write(() =>
        {
            _realm.Add(dailyLevel);
        });

        return dailyLevel;
    }

    // TODO: EDIT DAILY LEVEL
    public void RemoveDailyLevel(DailyLevel dailyLevel)
    {
        _realm.Write(() =>
        {
            _realm.Remove(dailyLevel);
        });
    }
}