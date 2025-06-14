namespace TosuSyncService.Model;

public class TosuEnum<TEnum> : TosuValue<TEnum> where TEnum : struct
{
    public override bool Equals(object? obj)
    {
        if (obj is TosuEnum<TEnum> tosuEnum)
        {
            return Equals(tosuEnum);
        }
        
        return false;
    }

    public override int GetHashCode()
    {
        HashCode hashCode = new HashCode();
        hashCode.Add(typeof(TEnum));
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        hashCode.Add(Value);
        return hashCode.ToHashCode();
    }

    protected bool Equals(TosuEnum<TEnum> other)
    {
        return other.Value.Equals(Value);
    }

    public static bool operator ==(TosuEnum<TEnum> left, TEnum right)
    {
        return left.Value.Equals(right);
    }

    public static bool operator !=(TosuEnum<TEnum> left, TEnum right)
    {
        return !(left == right);
    }
}