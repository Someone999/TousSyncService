using System.Diagnostics;
using FleeExpressionEvaluator.Expressions;
using TosuSyncService.Expressions.Lexer;
using TosuSyncService.Mmf.FileWatchers;

namespace TosuSyncService.Expressions;

public class ExpressionManager
{
    private ConcurrencyDetector _concurrencyDetector;
    private System.Timers.Timer _fileWriteWatcherTimer = new();
    private ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim();
    private FileWriteWatcher _fileWriteWatcher;
    private LexerExpressionMatcher _lexerExpressionMatcher = LexerExpressionMatcher.Instance;
    private IEnumerable<IExpression> _captures = [];
    private string _patternFile;

    public ExpressionManager(string patternFile)
    {
        _fileWriteWatcherTimer.AutoReset = true;
        _fileWriteWatcherTimer.Interval = 500;
        _fileWriteWatcher = FileWriteWatcherManager.GetOrCreate(patternFile);
        _fileWriteWatcher.FileWrote += ConfigFileWrote;
        _fileWriteWatcherTimer.Elapsed += (_, _) =>
        {
            _fileWriteWatcher.Check();
        };
        
        _patternFile = patternFile;
        _concurrencyDetector = new ConcurrencyDetector();

        void OnConcurrencyDetectorOnRateLimited(object? sender, EventArgs args)
        {
            StackTrace stackTrace = new();
            throw new InvalidOperationException(stackTrace.ToString());
        }

        _concurrencyDetector.RateLimited += OnConcurrencyDetectorOnRateLimited;
    }

    private void ConfigFileWrote(object? sender, string file, DateTime modifiedTime)
    {
       UpdateInternal(file);
    }

    public string OutputPattern { get; private set; } = "";

    public IEnumerable<IExpression> Captures
    {
        get
        {
            _readerWriterLockSlim.EnterReadLock();
            var ret = _captures;
            _readerWriterLockSlim.ExitReadLock();
            return ret;
        }
    }

    private void UpdateInternal(string patternFile)
    {
        _readerWriterLockSlim.EnterWriteLock();
        try
        {
            var content = File.ReadAllText(patternFile);
            _captures = _lexerExpressionMatcher.Matches(content);
            OutputPattern = content;
            ConfigUpdated?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception)
        {
            _captures = [];
        }
        finally
        {
            _readerWriterLockSlim.ExitWriteLock();
        }
        
    }
    
    public void Update()
    {
        UpdateInternal(_patternFile);
        ConfigUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void StopWatchFileWrite()
    {
        _fileWriteWatcherTimer.Stop();
    }

    public void StartWatchFileWrite()
    {
        _fileWriteWatcherTimer.Start();
    }

    public event EventHandler? ConfigUpdated;
}