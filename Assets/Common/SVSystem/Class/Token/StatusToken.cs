public abstract class StatusToken
{
    public abstract void ExtractStatus(in StatusVector s);

    public void Execute(in StatusVector target, StatusVector modified)
    {
        modified.Validate();
        Calculate(target, modified);
    }
    public abstract void Calculate(in StatusVector target, StatusVector modified);
}