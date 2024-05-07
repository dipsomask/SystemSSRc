using IniParser.Model;
using IniParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Settings.SettingClasses;
using System.Diagnostics;

namespace SSRc
{
    internal class ListenerSSRc
    {

        public TcpListener listener { get; set; }

        public ListenerSSRc? user5000 { get; set; }
        public ListenerSSRc? user5001 { get; set; }
        public ListenerSSRc? user5002 { get; set; }
        public ListenerSSRc? user5003 { get; set; }
        public ListenerSSRc? user5004 { get; set; }

        public int Port { get; private set; }


        /// <summary>
        /// Конструктор с назначением порта (по умолчанию 80 - системный).
        /// </summary>
        public ListenerSSRc(int port_ = 80)
        {

            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port_);

            Port = port_;

            if(port_ == 80)
            {

                user5000 = null;
                user5001 = null;
                user5002 = null;
                user5003 = null;
                user5004 = null;

                // Восстановленее ранее активных пользователей после перезагрузки
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

                    port_ = Convert.ToInt32(pair.KeyName[4..]);

                    switch (port_)
                    {

                        case 5000:

                            user5000 = new ListenerSSRc(5000);

                            user5000.StartListen();

                            user5000.ListenProcess();

                            break;

                        case 5001:

                            user5001 = new ListenerSSRc(5001);

                            user5001.StartListen();

                            user5001.ListenProcess();

                            break;

                        case 5002:

                            user5002 = new ListenerSSRc(5002);

                            user5002.StartListen();

                            user5002.ListenProcess();

                            break;

                        case 5003:

                            user5003 = new ListenerSSRc(5003);

                            user5003.StartListen();

                            user5003.ListenProcess();

                            break;

                        case 5004:

                            user5004 = new ListenerSSRc(5004);

                            user5004.StartListen();

                            user5004.ListenProcess();

                            break;

                        default:

                            break;

                    }

                }

            }

        }

        /// <summary>
        /// TCP метод. Запускает процесс ожидания входящий запросов.
        /// </summary>
        public void StartListen()
        {

            listener.Start();

        }

        /// <summary>
        /// TCP метод. Останавливает процесс ожидания входящий запросов.
        /// </summary>
        public void StopListen()
        {

            listener.Stop();

        }

        /// <summary>
        /// TCP метод. Запускает процесс обрабоки входящий запросов.
        /// </summary>
        public async void ListenProcess()
        {
           
            while (true)
            {

                TcpClient client = await listener.AcceptTcpClientAsync();


                var clientStream = client.GetStream();


                // Чтение ответа
                byte[] buffer = new byte[2048];

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

                if (responseMessage != null)
                {

                    string[] responseMessageArray = responseMessage.Split(' ');

                    switch (responseMessageArray[0])
                    {

                        case "run":

                            if (responseMessageArray.Length == 3)
                            {

                                int port_ = Convert.ToInt32(responseMessageArray[1]);

                                if(port_ == Port)
                                {

                                    string str = RunProcess(responseMessageArray[2]);

                                    byte[] requestMessage = Encoding.UTF8.GetBytes(str);

                                    await clientStream.WriteAsync(requestMessage);
                                }
                                else
                                {
                                    await clientStream.WriteAsync(Encoding.UTF8.GetBytes("EROR 404\nThe connection attempt failed."));
                                }

                            }

                            break;

                        case "add":

                            if (responseMessageArray.Length == 4)
                            {

                                if (responseMessageArray[1] == "-u")
                                {

                                    int port_ = Convert.ToInt32(responseMessageArray[3]);

                                    if(Port == 80)
                                    {
                                        switch (port_)
                                        {
                                            case 5000:

                                                if (user5000 != null)
                                                {
                                                    break;
                                                }

                                                user5000 = new ListenerSSRc(5000);

                                                user5000.StartListen();

                                                byte[] requestMessage_0 = Encoding.UTF8.GetBytes(AddUser(responseMessageArray[2], port_));

                                                await clientStream.WriteAsync(requestMessage_0);

                                                break;

                                            case 5001:

                                                if (user5001 != null)
                                                {
                                                    break;
                                                }

                                                user5001 = new ListenerSSRc(5001);

                                                user5001.StartListen();

                                                byte[] requestMessage_1 = Encoding.UTF8.GetBytes(AddUser(responseMessageArray[2], port_));

                                                await clientStream.WriteAsync(requestMessage_1);

                                                break;

                                            case 5002:

                                                if (user5002 != null)
                                                {
                                                    break;
                                                }

                                                user5002 = new ListenerSSRc(5002);

                                                user5002.StartListen();

                                                byte[] requestMessage_2 = Encoding.UTF8.GetBytes(AddUser(responseMessageArray[2], port_));

                                                await clientStream.WriteAsync(requestMessage_2);

                                                break;

                                            case 5003:

                                                if (user5003 != null)
                                                {
                                                    break;
                                                }

                                                user5003 = new ListenerSSRc(5003);

                                                user5003.StartListen();

                                                byte[] requestMessage_3 = Encoding.UTF8.GetBytes(AddUser(responseMessageArray[2], port_));

                                                await clientStream.WriteAsync(requestMessage_3);

                                                break;

                                            case 5004:

                                                if (user5004 != null)
                                                {
                                                    break;
                                                }

                                                user5004 = new ListenerSSRc(5004);

                                                user5004.StartListen();

                                                byte[] requestMessage_4 = Encoding.UTF8.GetBytes(AddUser(responseMessageArray[2], port_));

                                                await clientStream.WriteAsync(requestMessage_4);

                                                break;

                                            default:

                                                byte[] requestMessage_5 = Encoding.UTF8.GetBytes("There is user with entered username");

                                                await clientStream.WriteAsync(requestMessage_5);

                                                break;
                                        }
                                    }
                                    else
                                    {
                                        await clientStream.WriteAsync(Encoding.UTF8.GetBytes("EROR 404\nThe connection attempt failed."));
                                    }

                                }

                            }

                            break;

                        default:

                            await clientStream.WriteAsync(Encoding.UTF8.GetBytes("EROR 404\nThe connection attempt failed."));

                            break;

                    }

                }

                client.Close();

            }

        }



        /// <summary>
        /// Консольный метод. Добавляет указанный путь в каталог путей.
        /// </summary>
        public void AddPath(string section, string key, string value)
        {
            var iniFilePath = Ini.reserved_ini_path;
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

        /// <summary>
        /// Консольный метод. Удаляет указанный путь из каталога путей.
        /// </summary>
        public void RemovePath(string section, string key)
        {
            var iniFilePath = Ini.reserved_ini_path;
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

        /// <summary>
        /// Консольный метод. Выводит путь до указанного файла.
        /// </summary>
        public void GetPath(string section, string key)
        {
            var iniFilePath = Ini.reserved_ini_path;
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




        /// <summary>
        /// Консольный и TCP метод. Добавляет пользователя в каталог пользователей в поле "Активный",
        /// создаёт его дирректорию в системной папке и начинает прослушивание входящих на него соединений.
        /// </summary>
        public string AddUser(string Username, int Port)
        {

            var iniFilePath = Ini.reserved_ini_path;
            var userDirectoryPath = $@"C:\.SERVER\{Username}";

            if (!Directory.Exists(userDirectoryPath))
            {
                Directory.CreateDirectory(userDirectoryPath);
            }

            var iniParser = new FileIniDataParser();

            IniData allDataFromIniFile = iniParser.ReadFile(iniFilePath);
            string section = "ActiveUsers";

            string valueWithQuotes = @userDirectoryPath;

            if (allDataFromIniFile.Sections.ContainsSection(section))
            {
                allDataFromIniFile[section][Username] = valueWithQuotes;
            }
            else
            {
                allDataFromIniFile.Sections.AddSection(section);
                allDataFromIniFile[section].AddKey(Username, valueWithQuotes);
            }

            iniParser.WriteFile(iniFilePath, allDataFromIniFile);

            return "Sucessfull registration...";

        }

        /// <summary>
        /// Консольный и TCP метод. Отмечает пользователя в каталоег пользователей как "Остановленный" и 
        /// останавливает прослушивание входящих на него соединений.
        /// </summary>
        public void StopUser(string Username)
        {
            switch (Username)
            {

                case "user5000":
                    user5000?.StopListen();
                    break;
                case "user5001":
                    user5001?.StopListen();
                    break;
                case "user5002":
                    user5002?.StopListen();
                    break;
                case "user5003":
                    user5003?.StopListen();
                    break;
                case "user5004":
                    user5004?.StopListen();
                    break;
                default:
                    Console.WriteLine($"Нет такого пользователя: {Username}");
                    return;

            }

            var iniFilePath = Ini.reserved_ini_path;
            var iniParser = new FileIniDataParser();

            IniData allDataFromIniFile = iniParser.ReadFile(iniFilePath);
            string activeSection = "ActiveUsers";
            string stoppedSection = "StoppedUsers";

            if (allDataFromIniFile.Sections.ContainsSection(activeSection) && allDataFromIniFile[activeSection].ContainsKey(Username))
            {
                string userInfo = allDataFromIniFile[activeSection][Username];

                allDataFromIniFile[activeSection].RemoveKey(Username);

                if (!allDataFromIniFile.Sections.ContainsSection(stoppedSection))
                {

                    allDataFromIniFile.Sections.AddSection(stoppedSection);
                }

                allDataFromIniFile[stoppedSection].AddKey(Username, userInfo);
            }
            iniParser.WriteFile(iniFilePath, allDataFromIniFile);


        }

        /// <summary>
        /// Консольный и TCP метод. Отмечает пользователя в каталоег пользователей как "Активный" и 
        /// возобнавляет прослушивание входящих на него соединений.
        /// </summary>
        public string RefreshUser(string Username)
        {

            switch (Username)
            {

                case "user5000":
                    user5000?.StartListen();
                    user5000?.ListenProcess();
                    break;
                case "user5001":
                    user5001?.StartListen();
                    user5001?.ListenProcess();
                    break;
                case "user5002":
                    user5002?.StartListen();
                    user5002?.ListenProcess();
                    break;
                case "user5003":
                    user5003?.StartListen();
                    user5003?.ListenProcess();
                    break;
                case "user5004":
                    user5004?.StartListen();
                    user5004?.ListenProcess();
                    break;
                default:
                    return $"Нет такого пользователя: {Username}";

            }

            var iniFilePath = Ini.reserved_ini_path;
            var iniParser = new FileIniDataParser();

            IniData allDataFromIniFile = iniParser.ReadFile(iniFilePath);
            string activeSection = "ActiveUsers";
            string stoppedSection = "StoppedUsers";

            if (allDataFromIniFile.Sections.ContainsSection(stoppedSection) && allDataFromIniFile[stoppedSection].ContainsKey(Username))
            {

                string userInfo = allDataFromIniFile[stoppedSection][Username];

                allDataFromIniFile[stoppedSection].RemoveKey(Username);

                if (!allDataFromIniFile.Sections.ContainsSection(activeSection))
                {

                    allDataFromIniFile.Sections.AddSection(activeSection);
                }
                allDataFromIniFile[activeSection].AddKey(Username, userInfo);
            }

            iniParser.WriteFile(iniFilePath, allDataFromIniFile);

            return $"Аккаунт успешно восстановлен: {Username}";

        }

        /// <summary>
        /// Консольный и TCP метод. Удаляет пользователя из каталога пользователей,
        /// удаляет его дирректорию в системной папке и прекращает прослушивание входящих на него соединений.
        /// </summary>
        public void DeleteUser(string Username)
        {

            switch (Username)
            {

                case "user5000":

                    if (user5000 != null)
                    {

                        user5000.StopListen();

                    }
                    else
                    {

                        Console.WriteLine("Ошибка удаления дирректории.");

                        return;

                    }

                    break;

                case "user5001":

                    if (user5001 != null)
                    {

                        user5001.StopListen();

                    }
                    else
                    {

                        Console.WriteLine("Ошибка удаления дирректории.");

                        return;

                    }

                    break;

                case "user5002":

                    if (user5002 != null)
                    {

                        user5002.StopListen();

                    }
                    else
                    {

                        Console.WriteLine("Ошибка удаления дирректории.");

                        return;

                    }

                    break;

                case "user5003":

                    if (user5003 != null)
                    {

                        user5003.StopListen();

                    }
                    else
                    {

                        Console.WriteLine("Ошибка удаления дирректории.");

                        return;

                    }

                    break;

                case "user5004":

                    if (user5004 != null)
                    {

                        user5004.StopListen();

                    }
                    else
                    {

                        Console.WriteLine("Ошибка удаления дирректории.");

                        return;

                    }

                    break;

                default:

                    Console.WriteLine("Ошибка удаления дирректории.");

                    return;

            }

            var userDirectoryPath = $@"C:\.SERVER\{Username}";

            if (!Directory.Exists(userDirectoryPath))
            {

                Console.WriteLine($"Произошла ошибка при удалении директории: {userDirectoryPath}");

                return;

            }

            var iniFilePath = Ini.reserved_ini_path;
            var iniParser = new FileIniDataParser();

            IniData allDataFromIniFile = iniParser.ReadFile(iniFilePath);
            string activeSection = "ActiveUsers";
            string stoppedSection = "StoppedUsers";

            if (!allDataFromIniFile[activeSection].ContainsKey(Username))
            {

                if (!allDataFromIniFile[stoppedSection].ContainsKey(Username))
                {

                    Console.WriteLine("Ошибка удаления дирректории.");

                    return;

                }

                allDataFromIniFile[stoppedSection].RemoveKey(Username);

            }
            else
            {

                allDataFromIniFile[activeSection].RemoveKey(Username);

            }

            iniParser.WriteFile(iniFilePath, allDataFromIniFile);

            

            Directory.Delete(userDirectoryPath, true);

            Console.WriteLine("Директория успешно удалена.");

        }

        /// <summary>
        /// Консольный и TCP метод. Запускает и возвращает результат процесса выполнения 
        /// программы, хранящейся по переданному пути.
        /// </summary>
        public virtual string RunProcess(string path)
        {

            string @mainRunFile = path;

            string @argsToRun = string.Empty;

            var iniParser = new FileIniDataParser();

            IniData allDataFromIniFile = iniParser.ReadFile(Ini.reserved_ini_path);

            string @userName = mainRunFile.Split('\\').First();

            if (!mainRunFile.Contains(".exe"))
            {

                string extension = mainRunFile.Split('.').Last();

                string @extensionPath = allDataFromIniFile["Compilers"][extension];

                if (extensionPath == null)
                {

                    return $"Can't start process with file extension: {extension}...";

                }


                if (userName == "root")
                {
                    @mainRunFile = @mainRunFile.Replace(@userName, allDataFromIniFile["System"][@userName]);
                }
                else
                {
                    @mainRunFile = @mainRunFile.Replace(@userName, allDataFromIniFile["ActiveUsers"][@userName]);
                }

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

                if (userName == "root")
                {
                    @mainRunFile = @mainRunFile.Replace(@userName, allDataFromIniFile["System"][@userName]);
                }
                else
                {
                    @mainRunFile = @mainRunFile.Replace(@userName, allDataFromIniFile["ActiveUsers"][@userName]);
                }

            }

            processToRun.StartInfo.FileName = @mainRunFile;


            processToRun.Start();

            return $"Process {path.Split('\\').Last()} successfuly ran...\n";

        }

        public virtual string DownloadObject(object? _) { return ""; }

        public virtual byte? ImportObject(object? _) { return null; }

    }
}
