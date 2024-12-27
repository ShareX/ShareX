using System.ComponentModel;

namespace ShareX.ImageListView;

#region Event Delegates
/// <summary>
/// Represents the method that will handle the RunWorkerCompleted event.
/// </summary>
/// <param name="sender">The object that is the source of the event.</param>
/// <param name="e">A <see cref="QueuedWorkerCompletedEventArgs"/> that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void RunQueuedWorkerCompletedEventHandler(object sender, QueuedWorkerCompletedEventArgs e);
/// <summary>
/// Represents the method that will handle the DoWork event.
/// </summary>
/// <param name="sender">The object that is the source of the event.</param>
/// <param name="e">An <see cref="QueuedWorkerDoWorkEventArgs"/> that contains event data.</param>
[EditorBrowsable(EditorBrowsableState.Never)]
public delegate void QueuedWorkerDoWorkEventHandler(object sender, QueuedWorkerDoWorkEventArgs e);
#endregion

#region Event Arguments
/// <summary>
/// Represents the event arguments of the RunWorkerCompleted event.
/// </summary>
public class QueuedWorkerCompletedEventArgs : AsyncCompletedEventArgs
{
    /// <summary>
    /// Gets a value that represents the result of an asynchronous operation.
    /// </summary>
    public object Result { get; private set; }
    /// <summary>
    /// Gets the priority of this item.
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// Initializes a new instance of the QueuedWorkerCompletedEventArgs class.
    /// </summary>
    /// <param name="argument">The argument of an asynchronous operation.</param>
    /// <param name="result">The result of an asynchronous operation.</param>
    /// <param name="priority">A value between 0 and 5 indicating the priority of this item.</param>
    /// <param name="error">The error that occurred while loading the image.</param>
    /// <param name="cancelled">A value indicating whether the asynchronous operation was canceled.</param>
    public QueuedWorkerCompletedEventArgs(object argument, object result, int priority, Exception error, bool cancelled)
        : base(error, cancelled, argument)
    {
        Result = result;
        Priority = priority;
    }
}
/// <summary>
/// Represents the event arguments of the DoWork event.
/// </summary>
public class QueuedWorkerDoWorkEventArgs : DoWorkEventArgs
{
    /// <summary>
    /// Gets the priority of this item.
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// Initializes a new instance of the QueuedWorkerDoWorkEventArgs class.
    /// </summary>
    /// <param name="argument">The argument of an asynchronous operation.</param>
    /// <param name="priority">A value between 0 and 5 indicating the priority of this item.</param>
    public QueuedWorkerDoWorkEventArgs(object argument, int priority)
        : base(argument)
    {
        Priority = priority;
    }
}
#endregion
