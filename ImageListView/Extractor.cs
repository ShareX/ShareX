using System.Reflection;

namespace ShareX.ImageListView;

/// <summary>
/// Extracts thumbnails from images.
/// </summary>
internal static class Extractor
{
#if USEWIC
                            private static bool useWIC = true;
#else
    private static bool useWIC = false;
#endif
    private static IExtractor? instance = null;

    public static IExtractor Instance
    {
        get
        {
            if (instance == null)
            {
                if (!useWIC)
                {
                    instance = new GDIExtractor();
                } else
                {
                    try
                    {
                        string? programFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        if (programFolder != null)
                        {
                            string pluginFileName = Path.Combine(programFolder, "WPFThumbnailExtractor.dll");
                            instance = LoadFrom(pluginFileName);
                        } else
                        {
                            instance = new GDIExtractor();
                        }
                    } catch
                    {
                        instance = new GDIExtractor();
                    }
                }
            }

            if (instance == null)
                instance = new GDIExtractor();

            return instance;
        }
    }

    private static IExtractor LoadFrom(string pluginFileName)
    {
        Assembly assembly = Assembly.LoadFrom(pluginFileName);
        foreach (Type type in assembly.GetTypes())
        {
            if (type.GetInterfaces().Contains(typeof(IExtractor)) && !type.IsInterface && type.IsClass && !type.IsAbstract)
            {
                return (IExtractor)(Activator.CreateInstance(type) ?? throw new InvalidOperationException("Failed to create an instance of the IExtractor type."));
            }
        }

        throw new InvalidOperationException("No suitable IExtractor implementation found in the assembly.");
    }

    public static bool UseWIC
    {
        get
        {
            return useWIC;
        }
        set
        {
#if USEWIC
                                    useWIC = value;
                                    instance = null;
#else
            useWIC = false;
            if (value)
                System.Diagnostics.Debug.WriteLine("Trying to set UseWIC option although the library was compiled without WPF/WIC support.");
#endif
        }
    }
}
