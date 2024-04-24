using IniParser.Model;
using IniParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;


using SystemSSRc.SettingClasses;


namespace SystemSSRc
{

    // modified TCPListener abstract class
    internal abstract class Listener
    {

        protected TcpListener listener;

        public int Port { get; private set; }


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
        public virtual string AddUser(string arg1, int arg2) { return ""; }


        public virtual void StopUser(string arg1) { }


        public virtual string RefreshUser(string arg1) { return ""; }


        public virtual void DeleteUser(string arg1, List<UserListenerSSRc> arg2) { }


        public virtual void AddPath(string arg1, string arg2, string arg3) { }


        public virtual void RemovePath(string arg1, string arg2) { }


        public virtual void GetPath(string arg1, string arg2) { }



        // methodes for UserListenerSSRc
        public static string RunProcess(string _)
        {

            string @mainRunFile = _;

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


                if(userName == "root")
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

            return $"Process {_.Split('\\').Last()} successfuly ran...";

        }


        public virtual string DownloadObject(object? _) { return ""; }


        public virtual byte? ImportObject(object? _) { return null; }



    }

}
