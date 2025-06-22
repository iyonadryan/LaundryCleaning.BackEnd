using System.Reflection;
using System.Runtime.Loader;

namespace LaundryCleaning.Service.Extensions
{
    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDll(absolutePath);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllPath)
        {
            return LoadUnmanagedDllFromPath(unmanagedDllPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            // We don't need to load any managed assemblies manually
            return null;
        }
    }
}
