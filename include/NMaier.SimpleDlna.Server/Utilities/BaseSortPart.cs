namespace NMaier.SimpleDlna.Server.Utilities;

internal abstract class BaseSortPart : IComparable<BaseSortPart>
{
    private readonly Type _type;

    protected BaseSortPart()
    {
        _type = GetType();
    }

    public int CompareTo(BaseSortPart? other)
    {
        if (other == null)
        {
            return 1;
        }
        if (_type != other._type)
        {
            if (_type == typeof(StringSortPart))
            {
                return 1;
            }
            return -1;
        }
        if (other is StringSortPart sp)
        {
            return ((StringSortPart)this).CompareTo(sp);
        }
        return ((NumericSortPart)this).CompareTo(
          (NumericSortPart)other);
    }
}