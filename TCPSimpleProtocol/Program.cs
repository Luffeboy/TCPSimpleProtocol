// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Net;
using System.Threading;
Console.WriteLine("write \"s\" for server, write \"c\" for client");

bool isServer = false;
bool isUsingJSON = false;
int port = 20020; // + 1 for JSON communication
while (true)
{
    string serverOrClient = Console.ReadLine();
    if (serverOrClient[0] == 's')
    {
        isServer = true;
        break;
    }
    if (serverOrClient[0] == 'c')
    {
        isServer = false;
        break;
    }
}
Console.WriteLine("Do you want to communicate via JSON? (write \"y\" for yes or \"n\" for no)");
while (true)
{
    string serverOrClient = Console.ReadLine();
    if (serverOrClient[0] == 'y')
    {
        isUsingJSON = true;
        port++;
        break;
    }
    if (serverOrClient[0] == 'n')
    {
        isUsingJSON = false;
        break;
    }
}
if (isServer)
    StartServer(port);
else
{
    if (isUsingJSON)
        StartClientJSON(port);
    else
        StartClient(port);
}





void StartServer(int port)
{
    using TcpListener listener = new TcpListener(IPAddress.Any, port);
    listener.Start();
    while (true)
    {
        TcpClient socket = listener.AcceptTcpClient();
        if (isUsingJSON)
            Task.Run(() => { ServerAcceptClientJSON(socket); });
        else
            Task.Run(() => { ServerAcceptClient(socket); });
    }
    //listener.Dispose(); // it will automatically dispose :)
}
void ServerAcceptClient(TcpClient socket)
{
    NetworkStream stream = socket.GetStream();
    StreamReader reader = new StreamReader(stream);
    StreamWriter writer = new StreamWriter(stream);
    while (true)
    {
        string cmd = "";
        string numbers = "";
        try
        {
            cmd = reader.ReadLine();
            writer.WriteLine("Input numbers");
            writer.Flush();
            numbers = reader.ReadLine();
        }
        catch // something messed up
        {
            break;
        }
        string[] numbersArray = numbers.Split(' ');
        if (numbersArray.Length != 2)
            break; // they messed up the message...
        int firstNum;
        int secondNum;
        if (!int.TryParse(numbersArray[0], out firstNum) || !int.TryParse(numbersArray[1], out secondNum))
            break; // couldn't parse the numbers...
        string response = ServerMethodResponse(cmd, firstNum, secondNum);
        writer.WriteLine(response);
        writer.Flush();
    }
    socket.Close();
}
void ServerAcceptClientJSON(TcpClient socket)
{
    NetworkStream stream = socket.GetStream();
    StreamReader reader = new StreamReader(stream);
    StreamWriter writer = new StreamWriter(stream);
    while (true)
    {
        string cmd = "";
        JSONObject jsonObject;
        try
        {
            cmd = reader.ReadLine();
            jsonObject = JSONObject.Deserialize(cmd);
        }
        catch // something messed up
        {
            break;
        }
        
        string response = ServerMethodResponse(jsonObject.Cmd, jsonObject.FirstNum, jsonObject.SecondNum);
        writer.WriteLine(response);
        writer.Flush();
    }
    socket.Close();
}

string ServerMethodResponse(string cmd, int firstNum, int secondNum)
{
    string response = "";
    switch (cmd)
    {
        case "Random":
            Random random = new Random();
            if (firstNum > secondNum)
            {
                int temp = firstNum;
                firstNum = secondNum;
                secondNum = temp;
            }
            response = "" + random.Next(firstNum, secondNum + 1);
            break;
        case "Add":
            response = "" + (firstNum + secondNum);
            break;
        case "Subtract":
            response = "" + (firstNum - secondNum);
            break;
    }
    return response;
}

void StartClient(int port)
{
    // stuff about json or no json
    Console.WriteLine("Write ipadress (leave blank for localhost)");
    string ipAddress = Console.ReadLine();
    if (string.IsNullOrEmpty(ipAddress))
        ipAddress = "127.0.0.1";
    // create connection
    using TcpClient socket = new TcpClient(ipAddress, port);
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
            writer.WriteLine(firstNum); // if the person wrote "num num"
        else
            writer.WriteLine(firstNum + " " + Console.ReadLine());  // if the person wrote "num\nnum"
        writer.Flush();
        Console.WriteLine(reader.ReadLine());


    }
    Console.ReadKey(); // stop terminal from closing
}
void StartClientJSON(int port)
{
    // stuff about json or no json
    Console.WriteLine("Write ipadress (leave blank for localhost)");
    string ipAddress = Console.ReadLine();
    if (string.IsNullOrEmpty(ipAddress))
        ipAddress = "127.0.0.1";
    // create connection
    using TcpClient socket = new TcpClient(ipAddress, port);
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
    Console.ReadKey(); // stop terminal from closing
}