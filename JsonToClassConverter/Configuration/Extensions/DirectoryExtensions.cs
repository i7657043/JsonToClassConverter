public static class DirectoryExtensions
{
    public static void CreateIfNotExists(this string path)
    {
        try
        {
            string? dir = Path.GetDirectoryName(path);
            if (dir == null)
                throw new PathException(path);
            else if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
        catch (PathException)
        {
            throw;
        }
        
    }
}
