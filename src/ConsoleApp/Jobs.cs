namespace ConsoleApp
{
    public static class Jobs
    {
        public static void PrintMessage()
        {
            Console.WriteLine($"Background job executed at {DateTime.Now:HH:mm:ss}");
        }
    }
}