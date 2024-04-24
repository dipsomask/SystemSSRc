using IniParser.Model;
using IniParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


using SystemSSRc.SettingClasses;


namespace SystemSSRc
{

    // system listener class
    internal class SystemListenerSSRc : Listener
    {

        public override async void ListenProcess(List<UserListenerSSRc>? userList_)
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

                                    break;

                                }

                            }

                            await clientStream.WriteAsync(
                                Encoding.UTF8.GetBytes("EROR 404\nThe connection attempt failed."));

                            break;

                        case "add":

                            if (responseMessageArray.Length == 4)
                            {

                                if (responseMessageArray[1] == "-u")
                                {

                                    int port_ = Convert.ToInt32(responseMessageArray[3]);

                                    if (userList_.FirstOrDefault(x => x.Port == port_) != null)
                                    {

                                        byte[] requestMessage = Encoding.UTF8.GetBytes("There is user with entered username");

                                        await clientStream.WriteAsync(requestMessage);

                                        break;

                                    }
                                    else
                                    {

                                        UserListenerSSRc newUser = new UserListenerSSRc(port_);

                                        userList_.Add(newUser);

                                        newUser.StartListen();

                                        byte[] requestMessage = Encoding.UTF8.GetBytes(AddUser(responseMessageArray[2], port_));

                                        await clientStream.WriteAsync(requestMessage);

                                    }

                                }

                            }

                            break;

                        default:

                            await clientStream.WriteAsync(Encoding.UTF8.GetBytes("It is not an internal or external command..."));

                            break;

                    }

                }

                client.Close();

            }

        }


        // console methodes
        public override void AddPath(string section, string key, string value)
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


        public override void RemovePath(string section, string key)
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


        public override void GetPath(string section, string key)
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



        // console or TCP methodes
        public override string AddUser(string Username, int Port)
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


        public override void StopUser(string Username)
        {

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


        public override string RefreshUser(string Username)
        {

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

            return "Your account was sucessfuly refresh."; 

        }


        public override void DeleteUser(string Username, List<UserListenerSSRc> UserList)
        {

            var userDirectoryPath = $@"C:\.SERVER\{Username}";

            if (!Directory.Exists(userDirectoryPath))
            {

                Console.WriteLine($"Произошла ошибка при удалении директории: {userDirectoryPath}");

            }

            var iniFilePath = Ini.reserved_ini_path;
            var iniParser = new FileIniDataParser();

            IniData allDataFromIniFile = iniParser.ReadFile(iniFilePath);
            string activeSection = "ActiveUsers";
            string stoppedSection = "StoppedUsers";

            if (UserList == null)
            {

                Console.WriteLine("Ошибка удаления дирректории.");

                return;

            }

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

            UserList.FirstOrDefault(x => x.Port == Convert.ToInt32(Username.Substring(4))).StopListen();

            UserList.Remove(item: UserList.FirstOrDefault(x => x.Port == Convert.ToInt32(Username.Substring(4))));

            Directory.Delete(userDirectoryPath, true);

            Console.WriteLine("Директория успешно удалена.");

        }
    
    
    }

}
