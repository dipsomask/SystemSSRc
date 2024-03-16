using System.Net;
using System.Net.Sockets;



ConsoleSSRc consoleSSRc = new ConsoleSSRc();

consoleSSRc.UsingConsole();



// modified TCPListener abstract class
public abstract class Listener
{


    protected TcpListener listener;

    protected int Port;


    public Listener(int port_ = 80)
    {

        listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port_);

        Port = port_;

    }


    public void StartListen()
    {

        listener.Start();

    }


    public void StopListen()
    {

        listener.Stop();

    }


    public abstract void ListenProcess();



    // methodes for SystemListenerSSRc
    public virtual void MakeUser() {  }


    public virtual void StopUser() {  }


    public virtual void DeleteUser() {  }


    public virtual void AddPath(string arg1, string arg2) {  }


    public virtual void RemovePath(string _) {  }


    public virtual void GetPath(string _) { }



    // methodes for UserListenerSSRc
    public virtual void RunProcess(object? _) { }

    public virtual void DownloadObject(object? _) {  }

    public virtual byte? ImportObject(object? _) { return null; }



}



// system listener class
public class SystemListenerSSRc : Listener
{


    public override async void ListenProcess()
    {

        while (true)
        {

            TcpClient Client = await listener.AcceptTcpClientAsync();

            



        }

    }


    public override void MakeUser()
    {
        Console.WriteLine("ok");
    }


    public override void StopUser()
    {
        Console.WriteLine("ok");
    }


    public override void DeleteUser()
    {
        Console.WriteLine("ok");
    }



    // console methodes
    public override void AddPath(string arg1, string arg2)
    {
        Console.WriteLine("ok");
    }


    public override void RemovePath(string _)
    {
        Console.WriteLine("ok");
    }


    public override void GetPath(string _)
    {
        Console.WriteLine("ok");
    }


}



// users connections listener class
public class UserListenerSSRc(int port_) : Listener(port_)
{


    public override async void ListenProcess()
    {

        while (true)
        {

            TcpClient Client = await listener.AcceptTcpClientAsync();





        }

    }


    public override void RunProcess(object? _)
    {

        Console.WriteLine("ok");

    }

    public override void DownloadObject(object? _)
    {

        Console.WriteLine("ok");

    }

    public override byte? ImportObject(object? _) 
    {

        Console.WriteLine("ok");

        return null;
    
    }


}



internal class ConsoleSSRc
{


    /*console comands        
    {"add", "-pa" },
    {"remove", "-pa" },
    {"find", "-pa"},
    */

    private SystemListenerSSRc systemL;


    public ConsoleSSRc()
    {

        systemL = new SystemListenerSSRc();

        Console.WriteLine("console ready to work...");

    }


    public async void UsingConsole()
    {

        while (true)
        {

            string? consoleInput = await Console.In.ReadLineAsync();


            if (consoleInput != null)
            {

                string[] consoleInputArray= consoleInput.Split(' ');

                switch (consoleInputArray[0])
                {

                    case "add":

                        if(consoleInputArray.Length > 2 && consoleInputArray.Length < 5)
                        {

                            if (consoleInputArray[1] == "-pa")
                            {

                                systemL.AddPath(consoleInputArray[2], consoleInputArray[3]);

                            }
                            else
                            {

                                Console.WriteLine("It is not an internal or external command...");

                            }

                        }

                        break;



                    case "remove":

                        if (consoleInputArray.Length == 3)
                        {

                            if (consoleInputArray[1] == "-pa")
                            {

                                systemL.RemovePath(consoleInputArray[2]);

                            }
                            else
                            {

                                Console.WriteLine("It is not an internal or external command...");

                            }

                        }

                        break;

                    case "find":

                        if( consoleInputArray.Length == 3)
                        {

                            if (consoleInputArray[1] == "-pa")
                            {

                                systemL.GetPath(consoleInputArray[2]);

                            }
                            else
                            {

                                Console.WriteLine("It is not an internal or external command...");

                            }

                        }

                        break;


                    default:

                        Console.WriteLine("It is not an internal or external command...");

                        break;

                }

            }


        }



    }


}

