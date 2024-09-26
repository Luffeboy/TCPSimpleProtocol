using System.Net.Sockets;

public class Client
{
    public static void StartClient(int port)
    {
        using TcpClient socket = ConnectToServer(port);
        NetworkStream stream = socket.GetStream();
        StreamReader reader = new StreamReader(stream);
        StreamWriter writer = new StreamWriter(stream);
        while (true)
        {
            // user input
            Console.WriteLine("What function do you want to use? (Random, Add or Subtract)");
            string method = Console.ReadLine();

            writer.WriteLine(method);
            writer.Flush();
            Console.WriteLine(reader.ReadLine());
            string firstNum = Console.ReadLine();
            if (firstNum.Contains(" "))
                writer.WriteLine(firstNum); // if the person wrote "num num" (hopefully)
            else
                writer.WriteLine(firstNum + " " + Console.ReadLine());  // if the person wrote "num\nnum"
            writer.Flush();
            Console.WriteLine(reader.ReadLine());


        }
    }
    public static void StartClientJSON(int port)
    {
        using TcpClient socket = ConnectToServer(port);
        NetworkStream stream = socket.GetStream();
        StreamReader reader = new StreamReader(stream);
        StreamWriter writer = new StreamWriter(stream);
        while (true)
        {
            // user input
            Console.WriteLine("What function do you want to use? (Random, Add or Subtract)");
            string method = Console.ReadLine();
            Console.WriteLine("input numbers");
            string firstNum = Console.ReadLine();
            int firstNumAsInt;
            int secondNumAsInt;
            if (firstNum.Contains(' '))
            {
                // they wrote the two numbers in one message
                string[] nums = firstNum.Split(' ');
                if (nums.Length != 2 || !int.TryParse(nums[0], out firstNumAsInt) || !int.TryParse(nums[1], out secondNumAsInt))
                    continue; // couldn't parse the numbers or there were too many numbers/words :(
            }
            else
            {
                // they only wrote one of the numbers
                if (!int.TryParse(firstNum, out firstNumAsInt) || !int.TryParse(Console.ReadLine(), out secondNumAsInt))
                    continue; // unable to parse the numbers
            }
            writer.WriteLine(new JSONObject(method, firstNumAsInt, secondNumAsInt).ToString());
            writer.Flush();
            Console.WriteLine(reader.ReadLine());


        }
    }

    private static TcpClient ConnectToServer(int port)
    {
        // get ip address
        Console.WriteLine("Write ipadress (leave blank for localhost)");
        string ipAddress = Console.ReadLine();
        if (string.IsNullOrEmpty(ipAddress))
            ipAddress = "127.0.0.1";
        // create connection
        return new TcpClient(ipAddress, port);
    }
}