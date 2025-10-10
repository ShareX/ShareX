using System.Runtime.InteropServices;

namespace ShareX.HelpersLib
{
    public static class ArchitectureHelper
    {
        public static bool IsArm64()
        {
            try
            {
                return RuntimeInformation.ProcessArchitecture == Architecture.Arm64;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsX64()
        {
            try
            {
                return RuntimeInformation.ProcessArchitecture == Architecture.X64;
            }
            catch
            {
                return false;
            }
        }
    }
}
