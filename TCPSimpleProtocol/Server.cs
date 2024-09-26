using System.Net.Sockets;
using System.Net;

public class Server
{
    public static void StartServer(int port, bool isUsingJSON)
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
        //listener.Dispose(); // it should automatically dispose, due to "using" keyword :)
    }
    public static void ServerAcceptClient(TcpClient socket)
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
    static void ServerAcceptClientJSON(TcpClient socket)
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

    static string ServerMethodResponse(string cmd, int firstNum, int secondNum)
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
}