using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

public class JsonData
{
    public string direction { get; set; }
}

public delegate void CallbackDirection(JsonData message);

public class UdpServer
{
    public UdpClient listener;
    private Thread listenerThread;
    private bool isRunning = false;
    private CallbackDirection callback;

    public UdpServer(string ipAddress, int port)
    {
        listener = new UdpClient(port);
        listenerThread = new Thread(new ThreadStart(ListenForIncomingRequest));
        listenerThread.IsBackground = true;
        callback = null;
    }

    public void StartServer()
    {
        isRunning = true;
        listenerThread.Start();
    }

    public void StopServer()
    {
        isRunning = false;
        listener.Close();
    }

    private void ListenForIncomingRequest()
    {
        Console.WriteLine("Server started on port " + ((IPEndPoint)listener.Client.LocalEndPoint).Port);
        try
        {
            while (isRunning)
            {
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] bytes = listener.Receive(ref groupEP);
                string jsonString = Encoding.UTF8.GetString(bytes);
                var jsonData = JsonConvert.DeserializeObject<JsonData>(jsonString);
                Console.WriteLine("Received direction: " + jsonData.direction);
                MessageHandler(jsonData);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException " + e.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.ToString()); // 다른 예외가 발생한 경우도 처리할 수 있습니다.
        }
    }

    public void SetMessageCallback(CallbackDirection callback)
    {
        this.callback = callback;
    }

    private void MessageHandler(JsonData jsonData)
    {
        callback?.Invoke(jsonData); // Callback to the main program
    }
}

public class Program
{
    static void Main()
    {
        UdpServer udpServer = new UdpServer("172.28.2.0", 50004); // 서버 포트를 50004로 설정
        udpServer.SetMessageCallback(OnDirection);
        udpServer.StartServer();

        Console.WriteLine("Server running. Press any key to stop...");
        Console.ReadKey();

        udpServer.StopServer();
    }

    static void OnDirection(JsonData message)
    {
        string direction = message.direction;
        Console.WriteLine("Received direction: " + direction); // 수신된 방향을 콘솔에 출력
    }
}
