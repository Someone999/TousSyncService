using System.Collections;
using System.Reflection;
using System.Text;
using FleeExpressionEvaluator.Evaluator.Context;
using HsManCommand;
using HsManCommand.Attributes;
using HsManCommand.Contexts;
using HsManCommand.Results;
using TosuSyncService.Mmf;

namespace TosuSyncService.Commands;

[Command("getValue", "get")]
public class GetValueCommand : ICommand
{
    static readonly Task<ICommandExecutionResult> SuccessResult 
        = Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Success));
    public Task<ICommandExecutionResult> Execute(IExecutionData executeContext, string commandName, string[] args)
    {
        if (args.Length < 1)
        {
            return SuccessResult;
        }
        
        string path = args[0];
        var pathLevels = path.Split(".");
        if (pathLevels.Length == 0)
        {
            return SuccessResult;
        }

        EvaluateContext context;
        if (args.Length < 2)
        {
            context = Global.DefaultContext;
        }
        else
        {
            context = Global.EvaluateContextRegistry.GetContext(args[1]) ?? Global.DefaultContext;
        }

        var firstMember = context.SymbolTable.Symbols[pathLevels[0]];
        var val = GetPropertyValue(pathLevels[1..], firstMember.GetType(), firstMember, []);
        if (args is [_, _, "at", _])
        {
            var index = args[3];
            val = GetValueByIndex(val, index);
        }
        
        PrintObject(path, val);
        return SuccessResult;
    }

    private object? GetValueByIndex(object? obj, object? index)
    {
        if (obj == null)
        {
            return null;
        }

        var type = obj.GetType();
        if (type.IsArray)
        {
            return GetArrayValueByIndex(obj, index);
        }

        switch (obj)
        {
            case IList list:
                return list[Convert.ToInt32(index)];
            case IEnumerable enumerable:
                return enumerable.Cast<object>().ElementAt(Convert.ToInt32(index));
        }

        if (obj is IDictionary dictionary)
        {
            return dictionary[Convert.ToString(index) ?? throw new KeyNotFoundException()];
        }

        var property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(p => p.GetIndexParameters().Length > 0);

        if (property == null)
        {
            return null;
        }
        
        try
        {
            return property.GetValue(obj, new[] { index });
        }
        catch
        {
            // Handle exceptions
        }

        // Add special handling for other cases if needed

        return null;
    }

    private object? GetArrayValueByIndex(object obj, object? index)
    {
        if (index == null)
        {
            return null;
        }
        
        var i = Convert.ToInt32(index);
        Array? a = obj as Array;
        return a?.GetValue(i);
    }

    private void PrintObject(string name, object? obj)
    {
        if (obj == null)
        {
            Console.WriteLine("null");
            return;
        }

        PrintObjectByType(name, obj);
    }
    
    private void PrintObjectByType(string name, object obj)
    {
        var type = obj.GetType();
        if (IsPrimitiveOrString(type))
        {
            Console.WriteLine(GetFormattedString(name, obj.ToString(), name.Length));
            return;
        }
        
        PrintAllProperties(name, obj);
    }

    private void PrintAllProperties(string name, object? ins)
    {
        if (ins == null)
        {
            return;
        }
        
        var type = ins.GetType();
        var bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        var properties = type.GetProperties(bindingFlags);
        StringBuilder output = new StringBuilder("--------------\n");
        var maxLength = 0;
        List<(string, string?)> list = new List<(string, string?)>();

        foreach (var property in properties)
        {
            var fullName = name + "." + property.Name;
            var val = property.GetValue(ins);
            list.Add((fullName, val?.ToString()));
            maxLength = int.Max(fullName.Length, maxLength);
        }

        foreach (var item in list)
        {
            output.AppendLine(GetFormattedString(item.Item1, item.Item2, maxLength));
        }
        
        output.AppendLine("--------------");
        Console.WriteLine(output);
    }
    
    private string GetFormattedString(string name, string? val, int maxLen)
    {
        return $"{name.PadRight(maxLen + 16) + (val ?? "null")}";
    }

    private bool IsPrimitiveOrString(Type t) => t.IsPrimitive || t == typeof(string);
    
    private object? GetPropertyValue(string[] levels, Type t, object? ins, HashSet<Type> processedType)
    {
        var bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        var type = ins?.GetType() ?? t;
        foreach (var level in levels)
        {
            if (!processedType.Add(type))
            {
                break;
            }

            var propertyInfo = type.GetProperty(level, bindingFlags);
            if (propertyInfo == null)
            {
                return null;
            }

            type = propertyInfo.PropertyType;
            ins = propertyInfo.GetValue(ins);
            if (ins == null)
            {
                return null;
            }
        }

        return ins;
    }

    public string GetHelp()
    {
        return "/getValue <path> [symbolTableName]";
    }

    public string GetUsage()
    {
        return "/getValue <path> [symbolTableName]";
    }

    public Task<ICommandExecutionResult> ExceptionHandler(Exception exception)
    {
        return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Success));
    }

    public bool AutoHandleException => true;
}