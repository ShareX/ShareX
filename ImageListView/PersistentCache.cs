using System.Security.Cryptography;
using System.Text;

namespace ShareX.ImageListView;

/// <summary>
/// Represents a collection of items on disk that can be read 
/// and written by multiple threads.
/// </summary>
internal class PersistentCache
{
    #region Member Variables
    private string mDirectoryName;
    private long mSize;
    private long mCurrentSize;
    private readonly object lockObject;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the cache directory.
    /// </summary>
    public string DirectoryName
    {
        get
        {
            lock (lockObject)
            {
                return mDirectoryName;
            }
        }
        set
        {
            lock (lockObject)
            {
                mDirectoryName = value;
                CalculateSize();
            }
        }
    }
    /// <summary>
    /// Gets or sets the cache size in bytes.
    /// </summary>
    public long Size
    {
        get
        {
            lock (lockObject)
            {
                return mSize;
            }
        }
        set
        {
            lock (lockObject)
            {
                mSize = value;
            }
        }
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PersistentCache"/> class.
    /// </summary>
    /// <param name="directoryName">The path to the cache file.</param>
    /// <param name="size">Cache size in bytes.</param>
    public PersistentCache(string directoryName, long size)
    {
        lockObject = new object();
        mCurrentSize = 0;
        mSize = size;
        mDirectoryName = directoryName;
        if (!string.IsNullOrEmpty(directoryName))
        {
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            lock (lockObject)
            {
                CalculateSize();
            }
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="PersistentCache"/> class.
    /// </summary>
    public PersistentCache()
        : this(string.Empty, 0)
    {
        ;
    }
    #endregion

    #region Instance Methods
    /// <summary>
    /// Reads an item from the cache.
    /// </summary>
    /// <param name="id">Item identifier.</param>
    /// <returns>A stream holding item data.</returns>
    public Stream Read(string id)
    {
        lock (lockObject)
        {
            MemoryStream ms = new();
            if (string.IsNullOrEmpty(mDirectoryName)) return ms;

            id = MakeKey(id);

            string filename = Path.Combine(mDirectoryName, id);
            if (!File.Exists(filename)) return ms;

            using (FileStream fs = new(filename, FileMode.Open, FileAccess.Read))
            {
                int read = 0;
                byte[] buffer = new byte[4096];
                while ((read = fs.Read(buffer, 0, 4096)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
            }

            return ms;
        }
    }

    /// <summary>
    /// Writes an item to the cache.
    /// </summary>
    /// <param name="id">Item identifier. If an item with this identifier already 
    /// exists, it will be overwritten.</param>
    /// <param name="data">Item data.</param>
    public void Write(string id, Stream data)
    {
        lock (lockObject)
        {
            if (string.IsNullOrEmpty(mDirectoryName)) return;

            id = MakeKey(id);
            string filename = Path.Combine(mDirectoryName, id);
            long bytesWritten = 0;
            using (FileStream fs = new(filename, FileMode.Create, FileAccess.Write))
            {
                int read = 0;
                byte[] buffer = new byte[4096];
                data.Seek(0, SeekOrigin.Begin);
                while ((read = data.Read(buffer, 0, 4096)) > 0)
                {
                    fs.Write(buffer, 0, read);
                    bytesWritten += read;
                }
            }
            mCurrentSize += bytesWritten;

            if (mCurrentSize > mSize / 2) PurgeCache();
        }
    }

    /// <summary>
    /// Removes an item from the cache.
    /// </summary>
    /// <param name="id">Item identifier.</param>
    public void Remove(string id)
    {
        lock (lockObject)
        {
            if (string.IsNullOrEmpty(mDirectoryName)) return;

            id = MakeKey(id);
            string filename = Path.Combine(mDirectoryName, id);
            if (!File.Exists(filename)) return;
            FileInfo fi = new(filename);
            mCurrentSize -= fi.Length;
            if (mCurrentSize < 0) mCurrentSize = 0;
            File.Delete(filename);
        }
    }

    /// <summary>
    /// Removes all items from the cache.
    /// </summary>
    public void Clear()
    {
        lock (lockObject)
        {
            if (string.IsNullOrEmpty(mDirectoryName)) return;

            foreach (string file in Directory.GetFiles(mDirectoryName))
            {
                File.Delete(file);
            }
            mCurrentSize = 0;
        }
    }

    /// <summary>
    /// Converts the given string to an item key.
    /// </summary>
    /// <param name="key">Input string.</param>
    /// <returns>Item key.</returns>
    private string MakeKey(string key)
    {
        using MD5 md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(key));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    /// <summary>
    /// Calculates the size of the cache.
    /// </summary>
    private void CalculateSize()
    {
        lock (lockObject)
        {
            mCurrentSize = 0;

            if (string.IsNullOrEmpty(mDirectoryName)) return;

            foreach (FileInfo file in new DirectoryInfo(mDirectoryName).GetFiles())
            {
                mCurrentSize += file.Length;
            }
        }
    }

    /// <summary>
    /// Removes old items from the cache.
    /// </summary>
    private void PurgeCache()
    {
        lock (lockObject)
        {
            if (string.IsNullOrEmpty(mDirectoryName)) return;
            if (mSize == 0) return;

            FileInfo[] files = new DirectoryInfo(mDirectoryName).GetFiles();
            List<FileInfo> index = new();

            foreach (FileInfo file in new DirectoryInfo(mDirectoryName).GetFiles())
            {
                index.Add(file);
            }
            index.Sort((f1, f2) =>
            {
                DateTime d1 = f1.CreationTime;
                DateTime d2 = f2.CreationTime;
                return d1 < d2 ? -1 : d2 > d1 ? 1 : 0;
            });
            while (index.Count > 0 && mCurrentSize > mSize / 2)
            {
                int i = index.Count - 1;
                mCurrentSize -= index[i].Length;
                index.RemoveAt(i);
                File.Delete(index[i].FullName);
            }
            if (mCurrentSize < 0) mCurrentSize = 0;
        }
    }
    #endregion
}
