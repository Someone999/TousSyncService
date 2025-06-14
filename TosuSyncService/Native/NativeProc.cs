using System.Runtime.InteropServices;

namespace TosuSyncService.Native;

public class NativeProc(IntPtr proc)
{
    public bool IsValid => proc != IntPtr.Zero;
    public IntPtr Address => proc;
    public T? GetProcedureAs<T>() where T: Delegate
    {
        return proc == IntPtr.Zero 
            ? null
            : Marshal.GetDelegateForFunctionPointer<T>(proc);
    }

    public bool TryInvoke<TDelegate, TRet>(object?[]? args, out TRet? ret) where TDelegate: Delegate
    {
        var procedure = GetProcedureAs<TDelegate>();
        if (procedure == null)
        {
            ret = default;
            return false;
        }

        ret = (TRet?)procedure.DynamicInvoke(args);
        return true;
    }
    
    public TRet? Invoke<TDelegate, TRet>(object?[]? args) where TDelegate: Delegate
    {
        var procedure = GetProcedureAs<TDelegate>();
        if (procedure == null)
        {
            return default;
        }

        var ret = (TRet?)procedure.DynamicInvoke(args);
        return ret;
    }
}