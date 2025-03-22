using Python.Runtime;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SemanticSimilarityAnalysis.Proj.Helpers
{
    public class CSharpPythonConnector
    {
        /// <summary>
        /// Generates a scatter plot from a CSV file using a Python script.
        /// </summary>
        /// <param name="relativeCsvFilePath">Relative path to the CSV file.</param>
        /// <param name="relativePlotFilePath">Relative path to save the generated plot.</param>
        public void PlotScatterFromCsv(string relativeCsvFilePath, string relativePlotFilePath)
        {
            var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"../../.."));
            string absoluteCsvFilePath = Path.Combine(projectRoot, relativeCsvFilePath);
            string absolutePlotFilePath = Path.Combine(projectRoot, relativePlotFilePath);
            string scriptPath = Path.Combine(projectRoot, "Helpers", "Python", "Scripts", "scatter_plot.py");
            if (!File.Exists(absoluteCsvFilePath))
            {
                Console.WriteLine($"Error: CSV file '{absoluteCsvFilePath}' not found.");
                return;
            }
            if (!File.Exists(scriptPath))
            {
                Console.WriteLine($"Error: Python script '{scriptPath}' not found.");
                return;
            }

            SetPythonDllPath(); // MUST do according to the documentation of Python.NET package
            PythonEngine.Initialize();
            Console.WriteLine("Python Engine initialized.");

            using (Py.GIL())
            {
                try
                {
                    RunPythonScript(scriptPath, absoluteCsvFilePath, absolutePlotFilePath);
                    PythonEngine.Shutdown();
                }
                catch (Exception)
                {
                    Console.WriteLine($"");
                }
            }
        }
        
        /// <summary>
        /// Executes a Python script with specified arguments.
        /// </summary>
        /// <param name="scriptPath">Path to the Python script.</param>
        /// <param name="csvFilePath">Path to the CSV file input.</param>
        /// <param name="plotFilePath">Path to save the generated plot.</param>
        public void RunPythonScript(string scriptPath, string csvFilePath, string plotFilePath)
        {
            string pythonExecutable = GetPythonExecutable()!;
            if (string.IsNullOrEmpty(pythonExecutable))
            {
                throw new Exception("Python executable not found. Ensure Python is installed and available in the system PATH.");
            }
            var start = new ProcessStartInfo()
            {
                FileName = pythonExecutable,
                Arguments = $"\"{scriptPath}\" \"{csvFilePath}\" \"{plotFilePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process? process = Process.Start(start))
            {
                if (process == null)
                {
                    throw new Exception("Failed to start Python process.");
                }

                using (StreamReader reader = process.StandardOutput)
                {
                    string output = reader.ReadToEnd();
                    Console.WriteLine(output);  // Print Python script print statements for debugging. 
                }

                process.WaitForExit(); // Ensure the process has completed
                if (process.ExitCode != 0)
                {
                    throw new Exception($"Python script failed with exit code {process.ExitCode}");
                }
            }
        }
        
        /// <summary>
        /// Sets the Python DLL path for Python.NET to function properly.
        /// </summary>
        private void SetPythonDllPath()
        {
            string pythonDllPath = DetectPythonLibrary()!;

            Console.WriteLine($"Detected Python library: {pythonDllPath}");

            if (string.IsNullOrEmpty(pythonDllPath) || !File.Exists(pythonDllPath))
            {
                throw new Exception("Python DLL not found. Ensure Python is installed.");
            }

            Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonDllPath);
            Console.WriteLine($"Python library set to: {pythonDllPath}");
        }

        /// <summary>
        /// Detects the appropriate Python library file based on the operating system.
        /// </summary>
        /// <returns>The path to the Python library file.</returns>
        private static string? DetectPythonLibrary()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string? pythonDllPath = Environment.GetEnvironmentVariable("PYTHON_DLL");
                return pythonDllPath;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "/Library/Developer/CommandLineTools/Library/Frameworks/Python3.framework/Versions/3.9/lib/libpython3.9.dylib";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "/usr/lib/libpython3.9.so";
            }

            return null;
        }
        
        /// <summary>
        /// Retrieves the Python executable based on the operating system.
        /// </summary>
        /// <returns>The name of the Python executable if found; otherwise, null.</returns>
        private string? GetPythonExecutable()
        {
            string pythonExecutable = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                pythonExecutable = "python";  // Windows should have python in PATH if installed via Windows Store or Anaconda
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                pythonExecutable = "python3"; // Typically, python3 is available in macOS
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                pythonExecutable = "python3"; // On many Linux distributions, `python3` is the default
            }

            // Optionally, check if Python executable is available in PATH
            if (string.IsNullOrEmpty(pythonExecutable) || !IsPythonExecutableInPath(pythonExecutable))
            {
                return null;
            }

            return pythonExecutable;
        }
        
        /// <summary>
        /// Checks if the given Python executable is available in the system's PATH.
        /// </summary>
        /// <param name="pythonExecutable">The name of the Python executable.</param>
        /// <returns>True if Python is found in PATH; otherwise, false.</returns>
        private bool IsPythonExecutableInPath(string pythonExecutable)
        {
            try
            {
                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = pythonExecutable,
                    Arguments = "--version", // Just check the version to see if it's available
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });

                if (process == null)
                {
                    return false;
                }

                using (var reader = process.StandardOutput)
                {
                    string output = reader.ReadToEnd();
                    return output.Contains("Python");
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
