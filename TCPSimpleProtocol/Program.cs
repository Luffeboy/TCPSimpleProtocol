// See https://aka.ms/new-console-template for more information
Console.WriteLine("write \"s\" for server, write \"c\" for client");

bool isServer = false;
int port = 20020;
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
if (isServer)
    StartServer(port);
else
    StartClient(port);





void StartServer(int port)
{

}

void StartClient(int port)
{

}