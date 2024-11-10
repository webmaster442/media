namespace Media.Database;

internal interface IDirtyFlag
{
    bool IsDirty { get; set; }
}
