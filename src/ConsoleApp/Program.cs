using ConsoleApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        //var enqueeExample = new SimpleEnqueeExample();
        //enqueeExample.RunExample();

        var enqueeExampleWithDashboard = new SimpleEnqueeWithDashboardExample();
        await enqueeExampleWithDashboard.RunExample(args);
    }
}