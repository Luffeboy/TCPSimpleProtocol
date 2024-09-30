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
    Server.StartServer(port, isUsingJSON);
else
{
    if (isUsingJSON)
        Client.StartClientJSON(port);
    else
        Client.StartClient(port);
}
Console.ReadKey(); // stop terminal from closing