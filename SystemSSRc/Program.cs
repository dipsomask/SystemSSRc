using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text;
using IniParser;
using IniParser.Model;

using SystemSSRc;
using SystemSSRc.SettingClasses;



ConsoleSSRc consoleSSRc = new ConsoleSSRc();

consoleSSRc.UsingConsole();



internal class ConsoleSSRc
{


    /*console comands        
    {"add", "-pa" },
    {"remove", "-pa" },
    {"find", "-pa"},
    {"run", "80", "root\path\to\file"},
    */

    private SystemListenerSSRc systemL;

    private UserListenerSSRc user5000;

    private UserListenerSSRc user5001;

    private UserListenerSSRc user5002;

    private UserListenerSSRc user5003;

    private UserListenerSSRc user5004;

    private List<UserListenerSSRc>? userList;


    public ConsoleSSRc()
    {

        systemL = new SystemListenerSSRc();

        userList = new List<UserListenerSSRc>();

        var iniFilePath = Ini.reserved_ini_path;
        var iniParser = new FileIniDataParser();

        IniData allDataFromIniFile = iniParser.ReadFile(iniFilePath);
        string activeSection = "ActiveUsers";
        string stoppedSection = "StoppedUsers";

        var activeArray = allDataFromIniFile[activeSection].ToArray();
        var stoppedArrey = allDataFromIniFile[stoppedSection].ToArray();

        var users = activeArray.Concat(stoppedArrey).ToArray();

        foreach (var pair in users)
        {

            userList.Add(new UserListenerSSRc(Convert.ToInt32(pair.KeyName[4..])));

            if(userList.Count > 0)
            {

                userList.Last().StartListen();

            }

        }

        Console.WriteLine("console ready to work...");

    }


    public async void UsingConsole()
    {

        systemL.StartListen();

        systemL.ListenProcess(userList);

        while (true)
        {

            string? consoleInput = await Console.In.ReadLineAsync();


            if (consoleInput != null)
            {

                string[] consoleInputArray = consoleInput.Split(' ');

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

                                if(consoleInputArray.Length < 4)
                                {

                                    Console.WriteLine("It is not an internal or external command...");

                                    break;

                                }

                                systemL.AddUser(consoleInputArray[2], Convert.ToInt32(consoleInputArray[3]));

                                switch(consoleInputArray[2])
                                {

                                    case "user5000":

                                        if(!userList.Contains(user5000))
                                        {

                                            userList.Add(user5000);

                                            user5000.StartListen();

                                            user5000.ListenProcess(null);

                                        }
                                        else
                                        {

                                            Console.WriteLine($"There is a user with port {user5000.Port}. Can't add it.");

                                        }

                                        break;

                                    case "user5001":

                                        if (!userList.Contains(user5001))
                                        {

                                            userList.Add(user5001);

                                            user5001.StartListen();

                                            user5001.ListenProcess(null);

                                        }
                                        else
                                        {

                                            Console.WriteLine($"There is a user with port {user5001.Port}. Can't add it.");

                                        }

                                        break;

                                    case "user5002":

                                        if (!userList.Contains(user5002))
                                        {

                                            userList.Add(user5002);

                                            user5002.StartListen();

                                            user5002.ListenProcess(null);

                                        }
                                        else
                                        {

                                            Console.WriteLine($"There is a user with port {user5002.Port}. Can't add it.");

                                        }

                                        break;

                                    case "user5003":

                                        if (!userList.Contains(user5003))
                                        {

                                            userList.Add(user5003);

                                            user5003.StartListen();

                                            user5003.ListenProcess(null);

                                        }
                                        else
                                        {

                                            Console.WriteLine($"There is a user with port {user5003.Port}. Can't add it.");

                                        }

                                        break;

                                    case "user5004":

                                        if (!userList.Contains(user5004))
                                        {

                                            userList.Add(user5004);

                                            user5004.StartListen();

                                            user5004.ListenProcess(null);

                                        }
                                        else
                                        {

                                            Console.WriteLine($"There is a user with port {user5004.Port}. Can't add it.");

                                        }

                                        break;

                                    default:

                                        break;

                                }

                            }
                            else
                            {

                                Console.WriteLine("It is not an internal or external command...");

                            }

                        }

                        break;

                    case "remove":

                        if (consoleInputArray.Length >= 3)
                        {

                            if (consoleInputArray[1] == "-pa")
                            {

                                systemL.RemovePath(consoleInputArray[2], consoleInputArray[3]);

                            }
                            else if (consoleInputArray[1] == "-u")
                            {

                                systemL.DeleteUser(consoleInputArray[2], userList);

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

                            if (consoleInputArray[1] == "-u")
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

                            if (consoleInputArray[1] == "-u")
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

                        if (consoleInputArray.Length == 3)
                        {

                            if (Convert.ToInt32(consoleInputArray[1]) == 80)
                            {

                                Listener.RunProcess(consoleInputArray[2]);

                            }
                            else
                            {
                                
                                var iniFilePath = Ini.reserved_ini_path;

                                var iniParser = new FileIniDataParser();

                                IniData allDataFromIniFile = iniParser.ReadFile(iniFilePath);

                                if (allDataFromIniFile["ActiveUsers"].Count == 0)
                                {

                                    break;

                                }

                                var _ = allDataFromIniFile["ActiveUsers"].ToArray();

                                foreach (var user in _)
                                {

                                    if (user.KeyName.Substring(4) == consoleInputArray[1])
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

                    case "-u":

                        if (consoleInputArray[1] == "-a")
                        {

                            if(userList != null)
                            {

                                int count = 0;

                                var iniFilePath = Ini.reserved_ini_path;

                                var iniParser = new FileIniDataParser();

                                IniData allDataFromIniFile = iniParser.ReadFile(iniFilePath);

                                Console.WriteLine("[ActiveUsers]");

                                string condition = string.Empty;

                                foreach(var user in allDataFromIniFile["ActiveUsers"])
                                {

                                    if(userList.FirstOrDefault(x => x.Port == Convert.ToInt32(user.KeyName.Substring(4))) != null)
                                    {

                                        condition = "connected";

                                    }
                                    else
                                    {

                                        condition = "noconnected";

                                    }

                                    Console.WriteLine($"user: {user.KeyName}, {user.Value}, {condition}");

                                }

                                Console.WriteLine();

                                Console.WriteLine("[StoppedUsers]");

                                foreach(var user in allDataFromIniFile["StoppedUsers"])
                                {

                                    if(userList.FirstOrDefault(x => x.Port == Convert.ToInt32(user.KeyName.Substring(4))) != null)
                                    {

                                        condition = "connected";

                                    }
                                    else
                                    {

                                        condition = "noconnected";

                                    }

                                    Console.WriteLine($"user: {user.KeyName}, {user.Value}, {condition}");

                                }

                            }

                        }

                        break;

                    case "exit":

                        System.Environment.Exit(0);

                        break;

                    case "cls":

                        Console.Clear();

                        Console.WriteLine("console ready to work...");

                        break;

                    default:

                        break;

                }

            }


        }



    }


}


