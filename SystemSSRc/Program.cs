using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using IniParser;
using IniParser.Model;



ConsoleSSRc consoleSSRc = new ConsoleSSRc();

consoleSSRc.UsingConsole();



// modified TCPListener abstract class
public abstract class Listener
{


    protected TcpListener listener;

    public int Port {  get; private set; }


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
    public virtual void MakeUser(string _) {  }


    public virtual void StopUser(string _) {  }


    public virtual void DeleteUser(string _) {  }


    public virtual void AddPath(string arg1, string arg2) {  }


    public virtual void RemovePath(string _) {  }


    public virtual void GetPath(string _) { }



    // methodes for UserListenerSSRc
    public void RunProcess(object _)
    {

        if (_.GetType() == typeof(string))
        {

            string @mainRunFile = (string)_;

            string @argsToRun = string.Empty;

            if (!mainRunFile.Contains(".exe"))
            {

                string extension = mainRunFile.Split('.').Last().ToLower();

                var iniParser = new FileIniDataParser();

                IniData allDataFromIniFile = iniParser.ReadFile(@"C:\.SERVER\.system\SystemSSRc\resurved.ini");

                string @extensionPath = allDataFromIniFile["Compilers"][extension];

                @mainRunFile = @mainRunFile.Replace("root", allDataFromIniFile["System"]["root"]);

                if (extensionPath == null)
                {

                    Console.WriteLine($"Can't start process with file extension: {extension}...");

                    return;

                }

                @argsToRun = @mainRunFile;

                @mainRunFile = extensionPath;

            }

            Console.WriteLine(@mainRunFile);

            Console.WriteLine(@argsToRun + "\n");


            Process processToRun = new Process();

            if (@argsToRun != string.Empty)
            {

                processToRun.StartInfo.Arguments = @argsToRun;

            }
            else
            {

                var iniParser = new FileIniDataParser();

                IniData allDataFromIniFile = iniParser.ReadFile(@"C:\.SERVER\.system\SystemSSRc\resurved.ini");

                @mainRunFile = @mainRunFile.Replace("root", allDataFromIniFile["System"]["root"]);

            }

            processToRun.StartInfo.FileName = @mainRunFile;



            processToRun.Start();

            Console.WriteLine($"Process by path: {(string)_} successfuly ran...");

        }

    }

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


    public override void MakeUser(string username)
    {
        


    }


    public override void StopUser(string username)
    {
        Console.WriteLine("ok");
    }


    public override void DeleteUser(string username)
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
    {"run"},
    */

    private SystemListenerSSRc systemL;

    private List<UserListenerSSRc>? userList;


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

                    case "run":

                        if( consoleInputArray.Length == 3)
                        {

                            if (Convert.ToInt32(consoleInputArray[1]) == 80)
                            {

                                //if (!Path.Exists(consoleInput[3].ToString()))
                                //{

                                //    Console.WriteLine($"You have not got this path {consoleInput[3]}...");

                                //    break;

                                //}

                                systemL.RunProcess(consoleInputArray[2]);

                            }
                            else
                            {

                                foreach (var item in userList)
                                {

                                    if (item.Port == Convert.ToInt32(consoleInputArray[1]))
                                    {

                                        if (!Path.Exists(consoleInput[3].ToString()))
                                        {

                                            Console.WriteLine($"You have not got this path {consoleInput[3]}...");

                                            break;

                                        }

                                        item.RunProcess(consoleInput[3]);

                                        break;

                                    }

                                }

                            }
                            

                        }
                        else
                        {

                            Console.WriteLine("It is not an internal or external command...1");

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

