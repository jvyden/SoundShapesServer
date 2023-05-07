using SoundShapesServer.Types;
using SoundShapesServer.Types.Users;

namespace SoundShapesServer.Database;

public partial class GameDatabaseContext
{
    // TODO: Implement same ordering system as levels
    public IQueryable<Report> GetReports()
    {
        return _realm.All<Report>();
    }
    public Report? GetReportWithId(string id)
    {
        return _realm.All<Report>().FirstOrDefault(r => r.Id == id);
    }

    public void RemoveReport(Report report)
    {
        _realm.Write(() =>
        {
            _realm.Remove(report);
        });
    }

    public void RemoveAllReportsWithContentId(string id)
    {
        _realm.Write(() =>
        {
            _realm.RemoveRange(_realm.All<Report>().Where(r=>r.ContentId == id));
        });
    }
    public void SubmitReport(GameUser reporter, string contentId, string contentType, int reportReasonId)
    {
        Report report = new ()
        {
            Id = GenerateGuid(),            
            Issuer = reporter,
            ContentId = contentId,
            ContentType = contentType,
            ReportReasonId = Math.Clamp(reportReasonId, 0, 6),
            Issued = DateTimeOffset.UtcNow
        };
        
        _realm.Write(() =>
        {
            _realm.Add(report);
        });
    }
}