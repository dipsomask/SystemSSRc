using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text;
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


    public abstract void ListenProcess(List<UserListenerSSRc>? userList_);



    // methodes for SystemListenerSSRc
    public virtual void AddUser(string arg1, int arg2) {  }


    public virtual void StopUser(string arg1) {  }


    public virtual void RefreshUser(string arg1) { }


    public virtual void DeleteUser(string arg1) {  }


    public virtual void AddPath(string arg1, string arg2, string arg3) {  }


    public virtual void RemovePath(string arg1, string arg2) {  }


    public virtual void GetPath(string arg1, string arg2) { }



    // methodes for UserListenerSSRc
    public static string RunProcess(string _ )
    {

        string @mainRunFile = (string)_;

        string @argsToRun = string.Empty;

        if (!mainRunFile.Contains(".exe"))
        {

            string extension = mainRunFile.Split('.').Last();


            var iniParser = new FileIniDataParser();

            IniData allDataFromIniFile = iniParser.ReadFile(@"C:\.SERVER\.system\SystemSSRc\resurved.ini");


            string @extensionPath = allDataFromIniFile["Compilers"][extension];

            if (extensionPath == null)
            {

                return $"Can't start process with file extension: {extension}...";

            }


            @mainRunFile = @mainRunFile.Replace("root", allDataFromIniFile["System"]["root"]);

            @argsToRun = @mainRunFile;

            @mainRunFile = extensionPath;

        }


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

        return $"Process {_.Split('\\').Last()} successfuly ran...";

    }

    public virtual void DownloadObject(object? _) {  }

    public virtual byte? ImportObject(object? _) { return null; }



}



// system listener class
public class SystemListenerSSRc : Listener
{

    public override async void ListenProcess(List<UserListenerSSRc>? userList_)
    {
        

        while (true)
        {

            TcpClient client = await listener.AcceptTcpClientAsync();

            var clientStream = client.GetStream();


            // Чтение ответа
            byte[] buffer = new byte[1024];

            int bytesRead;

            StringBuilder responseData = new StringBuilder();


            do
            {

                bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length);

                responseData.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

            } while (clientStream.DataAvailable);


            string response = responseData.ToString();

            string[] _ = response.Split('\n');

            string responseMessage = _.Last();

            Console.WriteLine($"Response message: {responseMessage}");



            if (responseMessage != null)
            {

                string[] responseMessageArray = responseMessage.Split(' ');

                //Console.WriteLine(responseMessageArray[4]);

                switch (responseMessageArray[0])
                {

                    case "run":

                        if (responseMessageArray.Length == 3)
                        {

                            if (Convert.ToInt32(responseMessageArray[1]) == 80)
                            {

                                byte[] requestMessage = Encoding.UTF8.GetBytes(RunProcess(responseMessageArray[2]));

                                await clientStream.WriteAsync(requestMessage);
                                
                            }

                        }
                        else
                        {

                            Console.WriteLine("It is not an internal or external command...");

                        }

                        break;

                    case "add":

                        if(responseMessageArray.Length == 4 && userList_ != null)
                        {

                            if (responseMessageArray[1] == "-u")
                            {

                                int port_ = Convert.ToInt32(responseMessageArray[2]);

                                UserListenerSSRc newUser = new UserListenerSSRc(port_);

                                userList_.Add(newUser);

                                AddUser(responseMessageArray[3], port_); 

                            }

                        }

                        break;


                    default:

                        Console.WriteLine("It is not an internal or external command...");

                        break;

                }

            }

            client.Close();

        }

    }


    // console methodes
    public override void AddPath(string section, string key, string value)
    {
        var iniFilePath = @"C:\.SERVER\.system\SystemSSRc\resurved.ini";
        var iniParser = new FileIniDataParser();

        try
        {
            IniData allDataFromIniFile = iniParser.ReadFile(iniFilePath);

            // Проверка на секции System и Compilers
            if (section != "System" && section != "Compilers")
            {
                Console.WriteLine("Access denied. Only System and Compilers sections are allowed.");
                return;
            }

            if (!allDataFromIniFile.Sections.ContainsSection(section))
            {
                allDataFromIniFile.Sections.AddSection(section);
            }

            allDataFromIniFile[section][key] = value;

            iniParser.WriteFile(iniFilePath, allDataFromIniFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine($".ini can't being read: {ex.Message}");
        }
    }

    public override void RemovePath(string section, string key)
    {
        var iniFilePath = @"C:\.SERVER\.system\SystemSSRc\resurved.ini"; // Путь к вашему файлу resurved.ini
        var iniParser = new FileIniDataParser();

        try
        {
            IniData allDataFromIniFile = iniParser.ReadFile(iniFilePath);

            // Проверка на секции System и Compilers
            if (section != "System" && section != "Compilers")
            {
                Console.WriteLine("Access denied. Only System and Compilers sections are allowed.");
                return;
            }

            if (allDataFromIniFile.Sections.ContainsSection(section) && allDataFromIniFile[section].ContainsKey(key))
            {
                allDataFromIniFile[section].RemoveKey(key);
                iniParser.WriteFile(iniFilePath, allDataFromIniFile);
            }
            else
            {
                Console.WriteLine("Not found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($".ini can't being read: {ex.Message}");
        }
    }

    public override void GetPath(string section, string key)
    {
        var iniFilePath = @"C:\.SERVER\.system\SystemSSRc\resurved.ini";
        var iniParser = new FileIniDataParser();

        try
        {
            IniData allDataFromIniFile = iniParser.ReadFile(iniFilePath);

            // Проверка на секции System и Compilers
            if (section != "System" && section != "Compilers")
            {
                Console.WriteLine("Access denied. Only System and Compilers sections are allowed.");
                return;
            }

            if (allDataFromIniFile.Sections.ContainsSection(section) && allDataFromIniFile[section].ContainsKey(key))
            {
                string value = allDataFromIniFile[section][key];
                Console.WriteLine($"value of '{key}' in section '{section}': {value}");
            }
            else
            {
                Console.WriteLine("can't being founded section or key");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($".ini can't being read: {ex.Message}");
        }
    }



    // console or TCP methodes
    public override void AddUser(string Username, int Port) // remake with parms: arg1 and arg2, also key=value
                                                            // pair in resurved.ini must be: Username="PortNumber PathToUserRootDirrectory"
    {
        var iniFilePath = @"C:\.SERVER\.system\SystemSSRc\resurved.ini";

        var iniParser = new FileIniDataParser();

        try
        {

            IniData allDataFromIniFile = iniParser.ReadFile(iniFilePath);

            string section = "ActiveUsers";

            if (allDataFromIniFile.Sections.ContainsSection(section))
            {

                allDataFromIniFile[section].AddKey(Username, "active");

            }
            else
            {

                allDataFromIniFile.Sections.AddSection(section);

                allDataFromIniFile[section].AddKey(Username, "active");

            }

            iniParser.WriteFile(iniFilePath, allDataFromIniFile);

        }
        catch (Exception ex)
        {

            Console.WriteLine($".ini can't being read: {ex.Message}");

        }

    }


    public override void StopUser(string Username)
    {

        var iniFilePath = @"C:\.SERVER\.system\SystemSSRc\resurved.ini";
        
        var iniParser = new FileIniDataParser();


        try
        {
            IniData allDataFromIniFile = iniParser.ReadFile(iniFilePath);

            string activeSection = "ActiveUsers";
            string stoppedSection = "StoppedUsers";

            if (allDataFromIniFile.Sections.ContainsSection(activeSection))
            {

                if (allDataFromIniFile[activeSection].ContainsKey(Username))
                {

                    allDataFromIniFile[activeSection].RemoveKey(Username);

                }

            }

            if (allDataFromIniFile.Sections.ContainsSection(stoppedSection))
            {

                allDataFromIniFile[stoppedSection].AddKey(Username, "stopped");

            }
            else
            {

                allDataFromIniFile.Sections.AddSection(stoppedSection);

                allDataFromIniFile[stoppedSection].AddKey(Username, "stopped");

            }

            iniParser.WriteFile(iniFilePath, allDataFromIniFile);

        }
        catch (Exception ex)
        {

            Console.WriteLine($".ini can't being read: {ex.Message}");

        }
    }


    public override void RefreshUser(string Username)
    {
        var iniFilePath = @"C:\.SERVER\.system\SystemSSRc\resurved.ini";
        var iniParser = new FileIniDataParser();


        try
        {
            IniData allDataFromIniFile = iniParser.ReadFile(iniFilePath);

            string activeSection = "ActiveUsers";

            string stoppedSection = "StoppedUsers";

            if (allDataFromIniFile.Sections.ContainsSection(stoppedSection))
            {

                if (allDataFromIniFile[stoppedSection].ContainsKey(Username))
                {

                    allDataFromIniFile[stoppedSection].RemoveKey(Username);

                }
            }

            if (allDataFromIniFile.Sections.ContainsSection(activeSection))
            {

                allDataFromIniFile[activeSection].AddKey(Username, "active");

            }
            else
            {

                allDataFromIniFile.Sections.AddSection(activeSection);

                allDataFromIniFile[activeSection].AddKey(Username, "active");

            }

            iniParser.WriteFile(iniFilePath, allDataFromIniFile);

        }
        catch (Exception ex)
        {

            Console.WriteLine($".ini can't being read: {ex.Message}");

        }
    }

    public override void DeleteUser(string Username)
    {
        var iniFilePath = @"C:\.SERVER\.system\SystemSSRc\resurved.ini";

        var iniParser = new FileIniDataParser();


        try
        {

            IniData allDataFromIniFile = iniParser.ReadFile(iniFilePath);

            string activeSection = "ActiveUsers";

            if (allDataFromIniFile.Sections.ContainsSection(activeSection))
            {

                if (allDataFromIniFile[activeSection].ContainsKey(Username))
                {

                    allDataFromIniFile[activeSection].RemoveKey(Username);

                }

            }

            string stoppedSection = "StoppedUsers";

            if (allDataFromIniFile.Sections.ContainsSection(stoppedSection))
            {

                if (allDataFromIniFile[stoppedSection].ContainsKey(Username))
                {

                    allDataFromIniFile[stoppedSection].RemoveKey(Username);

                }

            }

            iniParser.WriteFile(iniFilePath, allDataFromIniFile);

        }
        catch (Exception ex)
        {

            Console.WriteLine($".ini can't being read: {ex.Message}");

        }
    }


}



// users connections listener class
public class UserListenerSSRc(int port_) : Listener(port_)
{


    public override async void ListenProcess(List<UserListenerSSRc>? userList_)
    {

        while (true)
        {

            TcpClient client = await listener.AcceptTcpClientAsync();

            var clientStream = client.GetStream();


            // Чтение ответа
            byte[] buffer = new byte[1024];
            
            int bytesRead;
            
            StringBuilder responseData = new StringBuilder();


            do
            {

                bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length);
                
                responseData.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            
            } while (clientStream.DataAvailable);


            string responseMessage = responseData.ToString().Trim();
            
            Console.WriteLine($"Response message: {responseMessage}");

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
    {"run", "80", "root\path\to\file"},
    */

    private SystemListenerSSRc systemL;

    private List<UserListenerSSRc>? userList;


    public ConsoleSSRc()
    {

        systemL = new SystemListenerSSRc();

        // ONLY FOR TEST
        userList = new List<UserListenerSSRc>() { new UserListenerSSRc(5000) };
        // ONLY FOR TEST

        Console.WriteLine("console ready to work...");

    }


    public async void UsingConsole()
    {

        systemL.StartListen();

        systemL.ListenProcess(userList);

        // ONLY FOR TEST
        userList[0].StartListen();

        userList[0].ListenProcess(null);
        // ONLY FOR TEST

        while (true)
        {

            string? consoleInput = await Console.In.ReadLineAsync();


            if (consoleInput != null)
            {

                string[] consoleInputArray= consoleInput.Split(' ');

                switch (consoleInputArray[0])
                {

                    case "add":

                        if (consoleInputArray.Length > 2 && consoleInputArray.Length < 6)
                        {

                            if (consoleInputArray[1] == "-pa")
                            {

                                systemL.AddPath(consoleInputArray[2], consoleInputArray[3], consoleInputArray[4]);

                            }
                            else if (consoleInputArray[1] == "-u")
                            {
                                systemL.AddUser(consoleInputArray[3], Convert.ToInt32(consoleInputArray[2]));

                            }
                            else
                            {

                                Console.WriteLine("It is not an internal or external command...");

                            }

                        }

                        break;



                    case "remove":

                        if (consoleInputArray.Length == 4)
                        {

                            if (consoleInputArray[1] == "-pa")
                            {

                                systemL.RemovePath(consoleInputArray[2], consoleInputArray[3]);

                            }
                            else if (consoleInputArray[1] == "user")
                            {

                                systemL.DeleteUser(consoleInputArray[2]);

                            }
                            else
                            {

                                Console.WriteLine("It is not an internal or external command...");

                            }

                        }

                        break;

                    case "find":

                        if (consoleInputArray.Length == 4)
                        {

                            if (consoleInputArray[1] == "-pa")
                            {

                                systemL.GetPath(consoleInputArray[2], consoleInputArray[3]);

                            }
                            else
                            {

                                Console.WriteLine("It is not an internal or external command...");

                            }

                        }

                        break;

                    case "stop":

                        if (consoleInputArray.Length == 3)
                        {

                            if (consoleInputArray[1] == "user")
                            {

                                systemL.StopUser(consoleInputArray[2]);

                            }
                            else
                            {

                                Console.WriteLine("It is not an internal or external command...");

                            }

                        }

                        break;

                    case "refresh":

                        if (consoleInputArray.Length == 3)
                        {

                            if (consoleInputArray[1] == "user")
                            {

                                systemL.RefreshUser(consoleInputArray[2]);

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

                                Listener.RunProcess(consoleInputArray[2]);

                            }
                            else
                            {

                                foreach (var item in userList)
                                {

                                    if (item.Port == Convert.ToInt32(consoleInputArray[1]))
                                    {

                                        Listener.RunProcess(consoleInputArray[2]);

                                        break;

                                    }

                                }

                            }
                            

                        }
                        else
                        {

                            Console.WriteLine("It is not an internal or external command...");

                        }

                        break;

                    case "exit":

                        System.Environment.Exit(0);

                        break;

                    case "":

                        break;


                    default:

                        Console.WriteLine("It is not an internal or external command...");

                        break;

                }

            }


        }



    }


}


