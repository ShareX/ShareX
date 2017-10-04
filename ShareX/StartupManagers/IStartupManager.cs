namespace ShareX.StartupManagers
{
    public interface IStartupManager
    {
        StartupTaskState State { get; set; }
    }
}
