using System.Globalization;
using System.Text;
using FleeExpressionEvaluator.Evaluator;
using FleeExpressionEvaluator.Evaluator.Context;
using FleeExpressionEvaluator.Expressions;

namespace TosuSyncService.Expressions;

public class ExpressionReplacer
{
    private bool TryConvertToBool(string? str, out bool result)
    {
        if (string.IsNullOrEmpty(str))
        {
            result = false;
            return false;
        }

        if (bool.TryParse(str, out result))
        {
            return true;
        }

        if (!int.TryParse(str, out var i))
        {
            result = false;
            return false;
        }
        
        result = i != 0;
        return true;
    }
    
    private ExpressionReplacer()
    {
    }
    private FleeEvaluator _evaluator = new FleeEvaluator();
    public void Replace(IEnumerable<IExpression> expressions, StringBuilder originalText, EvaluateContext context)
    {
        foreach (var capture in expressions)
        {
            var realCapture = (TosuExpression) capture;
            var hasNoEval = realCapture.Metadata.TryGetValue("noEval", out var noEvalVal);
            if (hasNoEval && TryConvertToBool(noEvalVal, out var boolVal) && boolVal)
            {
                originalText.Replace(realCapture.FullText, capture.ExpressionText);
                continue;
            }
            
            var hasReplace = realCapture.Metadata.TryGetValue("replace", out var replaceVal);
            if (hasReplace)
            {
                originalText.Replace(realCapture.FullText, replaceVal);
                continue;
            }
            
            var val = _evaluator.Evaluate(capture, context);
            if (realCapture.OutputFormat is "")
            {
                originalText.Replace(capture.FullText, "");
                continue;
            }
            
            string result = val switch
            {
                IFormattable formattable => formattable.ToString(realCapture.OutputFormat, CultureInfo.InvariantCulture),
                null => "null",
                _ => val.ToString() ?? ""
            };
            
            originalText.Replace(capture.FullText, result);
        }
        
    }
    
    private static readonly Lazy<ExpressionReplacer> Lazy = new(() => new ExpressionReplacer());
    public static ExpressionReplacer Instance => Lazy.Value;
}