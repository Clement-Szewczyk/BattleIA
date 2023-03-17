namespace HelloWorld
{
    class Program
    {   
        static void Main(string[] args)
        {
            Console.WriteLine("toto");
            Console.WriteLine("toto2"); 
            Console.WriteLine("rentre une valeur : ")
            var value = Console.ReadLine();
            if (value == 1)
            {
                Console.WriteLine("UN");
            }
            else
            {
                Console.WriteLine("RIEN");      
            }
        }
    }
}
/*
Console.WriteLine("What is your name?");
            var name = Console.ReadLine();
            var currentDate = DateTime.Now;
            Console.WriteLine($"{Environment.NewLine}Hello, {name}, on {currentDate:d} at {currentDate:t}!");
            Console.Write($"{Environment.NewLine}Press any key to exit...");
            Console.ReadKey(true);
            */