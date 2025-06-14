// using osu.Game.Scoring;
// using osuToolsV2.Game.Mods;
// using TosuSyncService.PropertySynchronizer;
//
// namespace TosuSyncService.Utils;
//
// public static class DatabaseUtils
// {
//     static bool HasBeatmap(GosuData gosuData, GosuSyncDbContext dbContext)
//     {
//         var gosuBeatmap = gosuData.GosuMenu.Beatmap;
//         return dbContext.BeatmapHash.Any(h =>
//             h.Hash == gosuBeatmap.Md5 && gosuData.GamePlay.Ruleset == h.Ruleset);
//     }
//
//     private static GosuDataLazerScoreInfoSynchronizer _retryLazerScoreInfoSynchronizer = new();
//
//     private static GosuDataOsuToolsScoreInfoSynchronizer _retryOsuToolsScoreInfoSynchronizer = new();
//     private static readonly ScoreInfo _lazerScoreInfo = new ScoreInfo();
//     private static readonly osuToolsV2.Score.ScoreInfo _osuToolsScoreInfo = new osuToolsV2.Score.ScoreInfo();
//
//     public static (BeatmapHashDto?, BeatmapMetadataDto?)? AddBeatmapInfo(GosuData gosuData, GosuSyncDbContext dbContext)
//     {
//         _retryLazerScoreInfoSynchronizer.Synchronize(gosuData, _lazerScoreInfo);
//         _retryOsuToolsScoreInfoSynchronizer.Synchronize(gosuData, _osuToolsScoreInfo);
//         BeatmapMetadataDto beatmapMetadataDto;
//         BeatmapHashDto beatmapHashDto;
//
//         var beatmapHash = gosuData.GosuMenu.Beatmap.Md5;
//         var mods = _osuToolsScoreInfo.Mods;
//         if (mods != null && mods.Any(m => m is AutoPlayMod or CinemaMod))
//         {
//             return null;
//         }
//
//         if (HasBeatmap(gosuData, dbContext))
//         {
//             var storedMetadata = (from hash in dbContext.BeatmapHash
//                     join metadata in dbContext.BeatmapMetadata
//                         on hash.Id equals metadata.HashId where hash.Hash == beatmapHash
//                     select metadata).Include(b => b.BeatmapHashDto)
//                 .FirstOrDefault();
//
//             beatmapMetadataDto = storedMetadata ?? throw new InvalidOperationException();
//             beatmapHashDto = beatmapMetadataDto.BeatmapHashDto ?? throw new InvalidOperationException();
//         }
//         else
//         {
//             var tmp =
//                 BeatmapMetadataDto.FromGosuBeatmap(gosuData.GosuMenu.Beatmap);
//
//             if (tmp == null)
//             {
//                 return null;
//             }
//
//             beatmapMetadataDto = tmp;
//             beatmapHashDto = new BeatmapHashDto()
//             {
//                 Hash = gosuData.GosuMenu.Beatmap.Md5,
//                 Ruleset = gosuData.GamePlay.Ruleset
//             };
//
//             dbContext.BeatmapHash.Add(beatmapHashDto);
//             dbContext.SaveChanges();
//
//             beatmapMetadataDto.HashId = beatmapHashDto.Id;
//             dbContext.BeatmapMetadata.Add(beatmapMetadataDto);
//             dbContext.SaveChanges();
//         }
//
//         return new ValueTuple<BeatmapHashDto?, BeatmapMetadataDto?>(beatmapHashDto, beatmapMetadataDto);
//     }
//
//     public static void AddPlayRecord(GosuData data, GosuSyncDbContext dbContext, bool passed)
//     {
//         _retryLazerScoreInfoSynchronizer.Synchronize(data, _lazerScoreInfo);
//         _retryOsuToolsScoreInfoSynchronizer.Synchronize(data, _osuToolsScoreInfo);
//         
//         var mods = _osuToolsScoreInfo.Mods;
//         if (mods != null && mods.Any(m => m is AutoPlayMod or CinemaMod))
//         {
//             return;
//         }
//
//         
//         var currentBeatmapPath = BeatmapUtils.GetCurrentBeatmapPath(data);
//         if (string.IsNullOrEmpty(currentBeatmapPath))
//         {
//             return;
//         }
//
//         var ret = AddBeatmapInfo(data, dbContext);
//         if (ret == null)
//         {
//             return;
//         }
//
//         var (beatmapHashDto, beatmapMetadataDto) = ret.Value;
//         if (beatmapHashDto == null || beatmapMetadataDto == null)
//         {
//             return;
//         }
//
//         var ruleset = _osuToolsScoreInfo.Ruleset;
//         if (ruleset == null)
//         {
//             return;
//         }
//
//         var result = GamePlayResultDto.FromGosuData(data, passed);
//         result.BeatmapHashDto = beatmapHashDto;
//         dbContext.GamePlayResult.Add(result);
//         dbContext.SaveChanges();
//     }
//
//     public static void AddRetryRecord(GosuData data, GosuSyncDbContext dbContext)
//     {
//         _retryLazerScoreInfoSynchronizer.Synchronize(data, _lazerScoreInfo);
//         _retryOsuToolsScoreInfoSynchronizer.Synchronize(data, _osuToolsScoreInfo);
//         var mods = _osuToolsScoreInfo.Mods;
//         if (mods != null && mods.Any(m => m is AutoPlayMod or CinemaMod))
//         {
//             return;
//         }
//
//         var currentBeatmapPath = BeatmapUtils.GetCurrentBeatmapPath(data);
//         if (string.IsNullOrEmpty(currentBeatmapPath))
//         {
//             return;
//         }
//
//         var ret = AddBeatmapInfo(data, dbContext);
//         if (ret == null)
//         {
//             return;
//         }
//
//         var (beatmapHashDto, beatmapMetadataDto) = ret.Value;
//         if (beatmapHashDto == null || beatmapMetadataDto == null)
//         {
//             return;
//         }
//
//         var firstRetryCount = dbContext.RetryCount
//             .FirstOrDefault(b => b.HashId == beatmapHashDto.Id);
//         if (firstRetryCount == null)
//         {
//             dbContext.RetryCount.Add(new RetryCountDto()
//             {
//                 HashId = beatmapHashDto.Id,
//                 RetryCount = 1
//             });
//         }
//         else
//         {
//             firstRetryCount.RetryCount++;
//         }
//
//
//         var ruleset = _osuToolsScoreInfo.Ruleset;
//         if (ruleset == null)
//         {
//             return;
//         }
//
//         var result = GamePlayResultDto.FromGosuData(data, false);
//         result.BeatmapHashDto = beatmapHashDto;
//
//         var retryPointInfo = new RetryPointInfoDto()
//         {
//             GamePlayResultDto = result,
//             RetryTime = DateTime.Now,
//             Offset = data.GosuMenu.Beatmap.BeatmapTime.CurrentTime,
//         };
//
//
//         dbContext.GamePlayResult.Add(result);
//         dbContext.RetryPointInfo.Add(retryPointInfo);
//         dbContext.SaveChanges();
//     }
// }