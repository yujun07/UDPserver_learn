using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

[System.Serializable]
public class JsonData
{
    public string direction;
}

public class UDPClient : MonoBehaviour
{
    private UdpClient udpClient;
    public string serverIP = "172.28.2.0";
    public int serverPort = 50004; // 서버 포트를 50004로 설정
    public int localPort = 50003;

    void Start()
    {
        udpClient = new UdpClient(localPort);
    }

    public void SendData(JsonData data)
    {
        try
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
            string jsonString = JsonConvert.SerializeObject(data);
            byte[] sendBytes = Encoding.UTF8.GetBytes(jsonString);
            udpClient.Send(sendBytes, sendBytes.Length, endPoint);
            Debug.Log("Message sent: " + jsonString);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
}
