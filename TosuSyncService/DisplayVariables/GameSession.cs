using FleeExpressionEvaluator.Evaluator.Context;
using HsManCommonLibrary.ValueHolders;
using TosuSyncService.Model;
using TosuSyncService.Model.Settings;
using TosuSyncService.SmoothDampers;

namespace TosuSyncService.DisplayVariables;

public class GameSession(EvaluateContext context)
{
    private bool _hasFailed;
    private SmoothDamper<double> _scoreDifferSmoothDamper = new SmoothDamper<double>();

    public bool HasFailed
    {
        get
        {
            var gosuDataHolder = (IValueHolder<TosuData>) context.SymbolTable.Symbols["data"];
            if (!gosuDataHolder.TryGetValueAs<TosuData>(out var gosuData) || gosuData == null)
            {
                return false;
            }
            
            bool notPlaying = gosuData.State != TosuGameState.Play ||
                              gosuData.Beatmap.Time.CurrentTime < 10;
            if (notPlaying)
            {
                _hasFailed = false;
                return _hasFailed;
            }

            bool shouldRespawn = _hasFailed && gosuData.Play.HealthBar.Normal >= 199;
            if (shouldRespawn)
            {
                _hasFailed = false;
                return _hasFailed;
            }

            
            if (gosuData.Play.HealthBar.Normal > 0)
            {
                return _hasFailed;
            }

            _hasFailed = true;
            return _hasFailed;
        }
    }
}