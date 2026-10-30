namespace Project.Core.Exceptions
{
    /// <summary>
    /// Something dated in a closed accounting period was about to be recorded, changed or deleted. The message says
    /// which period, for the user; the API returns it as a 400.
    /// </summary>
    public class PeriodClosedException : InvalidOperationException
    {
        public PeriodClosedException(string message) : base(message) { }
    }
}
