using System.Net;
using System.Net.Sockets;
using System.Text;



TCPListenerSSRc Listener1 = new(80);

Listener1.Start();

TCPListenerSSRc Listener2 = new(5000);

Listener2.Start();


await Listener1.WorkerWithClients();

await Listener2.WorkerWithClients();


Listener1.Stop();

Listener2.Stop();


internal class TCPListenerSSRc
{

    private readonly TcpListener listener;

    private readonly int Port;


    // make tcplistener object
    public TCPListenerSSRc(int port)
    {

        Port = port;

        listener = new TcpListener(IPAddress.Parse("127.0.0.1"), Port);

    }


    // start listen connections to ssrc listener
    public void Start()
    {

        listener.Start();

        Console.WriteLine("SSRc is listening and awating connection...");

    }


    // stop listen connections to ssrc listener
    public void Stop()
    {

        listener.Stop();

        Console.WriteLine("SSRc finished listen...");

    }


    // make client and work with his request
    public async Task WorkerWithClients()
    {

        while (true)
        {

            TcpClient tcpClient = await listener.AcceptTcpClientAsync();

            await ResponseMessage(tcpClient, "Server listened you!");

        }

    }


    //test function of request-response string
    public async Task ResponseMessage(TcpClient tcpClient, string message)
    {

        var _ = tcpClient;

        var stream = tcpClient.GetStream();


        // read data
        byte[] sizeBuffer = new byte[4];

        await stream.ReadAsync(sizeBuffer, 0, sizeBuffer.Length);

        byte[] responseData = new byte[BitConverter.ToInt32(sizeBuffer, 0)];

        int bytes = await stream.ReadAsync(responseData, 0, responseData.Length);

        string response = Encoding.UTF8.GetString(responseData);

        Console.WriteLine(response);


        //make answer and send
        byte[] dataAnswer = Encoding.UTF8.GetBytes(message);

        await stream.WriteAsync(dataAnswer, 0, dataAnswer.Length);

        Console.WriteLine("Successful answer");
    }



}