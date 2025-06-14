using FleeExpressionEvaluator.Evaluator.Context;
using TosuSyncService.Websockets;

namespace TosuSyncService.DisplayVariables.Smoothed;

public class SmoothedValues(TosuWebSocketClient client, EvaluateContext context)
{
    public SmoothedGamePlay GamePlay { get; } = new SmoothedGamePlay(client, context);
    public SmoothedBeatmap Beatmap { get; } = new SmoothedBeatmap(context);
    public SmoothedExtra Extra { get; } = new SmoothedExtra(context);
}