using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text;
using IniParser;
using IniParser.Model;
using Settings.SettingClasses;

using SSRc;



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

    private ListenerSSRc systemListener;



    public ConsoleSSRc()
    {

        systemListener = new ListenerSSRc();

        Console.WriteLine("console ready to work...");

    }


    public async void UsingConsole()
    {

        systemListener.StartListen();

        systemListener.ListenProcess();

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

                                systemListener.AddPath(consoleInputArray[2], consoleInputArray[3], consoleInputArray[4]);

                            }
                            else if (consoleInputArray[1] == "-u")
                            {

                                if(consoleInputArray.Length < 4)
                                {

                                    Console.WriteLine("It is not an internal or external command...");

                                    break;

                                }

                                systemListener.AddUser(consoleInputArray[2], Convert.ToInt32(consoleInputArray[3]));

                                switch(consoleInputArray[2])
                                {

                                    case "user5000":

                                        if(systemListener.user5000 == null)
                                        {

                                            systemListener.user5000 = new ListenerSSRc(5000);

                                            systemListener.user5000.StartListen();

                                            systemListener.user5000.ListenProcess();

                                        }
                                        else
                                        {

                                            Console.WriteLine($"There is a user with port {systemListener.user5000.Port}. Can't add it.");

                                        }

                                        break;

                                    case "user5001":

                                        if (systemListener.user5001 == null)
                                        {

                                            systemListener.user5001 = new ListenerSSRc(5001);

                                            systemListener.user5001.StartListen();

                                            systemListener.user5001.ListenProcess();

                                        }
                                        else
                                        {

                                            Console.WriteLine($"There is a user with port {systemListener.user5001.Port}. Can't add it.");

                                        }

                                        break;

                                    case "user5002":

                                        if (systemListener.user5002 == null)
                                        {

                                            systemListener.user5002 = new ListenerSSRc(5000);

                                            systemListener.user5002.StartListen();

                                            systemListener.user5002.ListenProcess();

                                        }
                                        else
                                        {

                                            Console.WriteLine($"There is a user with port {systemListener.user5002.Port}. Can't add it.");

                                        }

                                        break;

                                    case "user5003":

                                        if (systemListener.user5003 == null)
                                        {

                                            systemListener.user5003 = new ListenerSSRc(5000);

                                            systemListener.user5003.StartListen();

                                            systemListener.user5003.ListenProcess();

                                        }
                                        else
                                        {

                                            Console.WriteLine($"There is a user with port {systemListener.user5003.Port}. Can't add it.");

                                        }

                                        break;

                                    case "user5004":

                                        if (systemListener.user5004 == null)
                                        {

                                            systemListener.user5004 = new ListenerSSRc(5000);

                                            systemListener.user5004.StartListen();

                                            systemListener.user5004.ListenProcess();

                                        }
                                        else
                                        {

                                            Console.WriteLine($"There is a user with port {systemListener.user5004.Port}. Can't add it.");

                                        }

                                        break;

                                    default:

                                        Console.WriteLine("It is not an internal or external command...");

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

                                systemListener.RemovePath(consoleInputArray[2], consoleInputArray[3]);

                            }
                            else if (consoleInputArray[1] == "-u")
                            {

                                systemListener.DeleteUser(consoleInputArray[2]);

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

                                systemListener.GetPath(consoleInputArray[2], consoleInputArray[3]);

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

                                systemListener.StopUser(consoleInputArray[2]);

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

                                Console.WriteLine(systemListener.RefreshUser(consoleInputArray[2]));

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

                                systemListener.RunProcess(consoleInputArray[2]);

                            }
                            else
                            {
                                
                                var iniFilePath = Ini.reserved_ini_path;

                                var iniParser = new FileIniDataParser();

                                IniData allDataFromIniFile = iniParser.ReadFile(iniFilePath);

                                if (allDataFromIniFile["ActiveUsers"].Count == 0)
                                {

                                    Console.WriteLine($"Невозможно запустить файл: {consoleInputArray[2]}");

                                    break;

                                }

                                var _ = allDataFromIniFile["ActiveUsers"].ToArray();

                                foreach (var user in _)
                                {

                                    if (user.KeyName.Substring(4) == consoleInputArray[1])
                                    {

                                        switch (consoleInputArray[1])
                                        {

                                            case "5000":
                                                systemListener.user5000?.RunProcess(consoleInputArray[2]);
                                                break;
                                            case "5001":
                                                systemListener.user5001?.RunProcess(consoleInputArray[2]);
                                                break;
                                            case "5002":
                                                systemListener.user5002?.RunProcess(consoleInputArray[2]);
                                                break;
                                            case "5003":
                                                systemListener.user5003?.RunProcess(consoleInputArray[2]);
                                                break;
                                            case "5004":
                                                systemListener.user5004?.RunProcess(consoleInputArray[2]);
                                                break;
                                            default:
                                                break;

                                        }

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

                            var iniFilePath = Ini.reserved_ini_path;

                            var iniParser = new FileIniDataParser();

                            IniData allDataFromIniFile = iniParser.ReadFile(iniFilePath);

                            var activeArray = allDataFromIniFile["ActiveUsers"].ToArray();
                            var stoppedArray = allDataFromIniFile["StoppedUsers"].ToArray();

                            Console.WriteLine("[ActiveUsers]");

                            foreach(var user in activeArray)
                            {
                                Console.WriteLine(user.KeyName.ToString() + ", " +  user.Value.ToString());
                            }

                            Console.WriteLine();

                            Console.WriteLine("[StoppedUsers]");

                            foreach (var user in stoppedArray)
                            {
                                Console.WriteLine(user.KeyName.ToString() + ", " + user.Value.ToString());
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


