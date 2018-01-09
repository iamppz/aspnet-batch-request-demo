using System;

namespace BatchRequest.SDK.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();
            client.Request();
            Console.ReadKey();
        }
    }
}
