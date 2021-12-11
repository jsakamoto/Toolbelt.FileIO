namespace Toolbelt;

public static class FileIO
{
    public static string FindContainerDirToAncestor(string wildCard)
    {
        var containerDir = AppDomain.CurrentDomain.BaseDirectory;
        while (containerDir != null && !ExistsAnyFilesInDir(containerDir, wildCard)) containerDir = Path.GetDirectoryName(containerDir);
        if (containerDir == null) throw new Exception("The container dir could not found.");
        return containerDir;
    }

    public static bool ExistsAnyFilesInDir(string dir, string wildCard)
    {
        return Directory.GetFiles(dir, wildCard, SearchOption.TopDirectoryOnly).Any();
    }

    public static void DeleteDir(string dir, int maxRetry = 3)
    {
        for (var retryAtemp = 0; retryAtemp <= maxRetry; retryAtemp++)
        {
            try
            {
                if (Directory.Exists(dir)) Directory.Delete(dir, recursive: true);
                break;
            }
            catch
            {
                if (retryAtemp == maxRetry) throw;
                Thread.Sleep(200 * (int)Math.Pow(2, retryAtemp));
            }
        }
    }

    public static void XcopyDir(string srcDir, string dstDir, Func<(string Dir, string Name), bool>? predicate = null)
    {
        if (predicate == null) predicate = _ => true;

        if (!predicate(ToArgument(dstDir))) return;
        Directory.CreateDirectory(dstDir);

        var srcFileNames = Directory.GetFiles(srcDir);
        foreach (var srcFileName in srcFileNames)
        {
            var dstFileName = Path.Combine(dstDir, Path.GetFileName(srcFileName));
            if (!predicate(ToArgument(dstFileName))) continue;
            File.Copy(srcFileName, dstFileName);
        }

        var srcSubDirs = Directory.GetDirectories(srcDir);
        foreach (var srcSubDir in srcSubDirs)
        {
            var dstSubDir = Path.Combine(dstDir, Path.GetFileName(srcSubDir));
            XcopyDir(srcSubDir, dstSubDir, predicate);
        }
    }

    private static (string Dir, string Name) ToArgument(string path)
    {
        return (Dir: Path.GetDirectoryName(path), Name: Path.GetFileName(path));
    }
}
