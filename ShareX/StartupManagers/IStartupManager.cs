namespace ShareX.StartupManagers
{
    interface IStartupManager
    {
        StartupTaskState State
        {
            get;
            set;
        }
    }
}
