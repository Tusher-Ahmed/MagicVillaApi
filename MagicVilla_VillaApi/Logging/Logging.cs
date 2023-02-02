namespace MagicVilla_VillaApi.Logging
{
    public class Logging : ILogging
    {
        public void Log(string message, string type)
        {
           if(type=="error")
            {
                Console.Error.WriteLine("Error - "+message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }
    }
}
