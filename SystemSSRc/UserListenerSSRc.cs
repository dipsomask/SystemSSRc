using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


using SystemSSRc.SettingClasses;


namespace SystemSSRc
{

    // users connections listener class
    internal class UserListenerSSRc(int port_) : Listener(port_), IComparable
    {


        public override async void ListenProcess(List<UserListenerSSRc>? arg)
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

                                if (Convert.ToInt32(responseMessageArray[1]) == Port)
                                {

                                    byte[] requestMessage = Encoding.UTF8.GetBytes(RunProcess(responseMessageArray[2]));

                                    await clientStream.WriteAsync(requestMessage);

                                    break;

                                }

                            }

                            await clientStream.WriteAsync(
                                Encoding.UTF8.GetBytes("EROR 404\nThe connection attempt failed."));

                            break;

                        default:

                            await clientStream.WriteAsync(Encoding.UTF8.GetBytes("It is not an internal or external command..."));

                            break;

                    }

                }

                client.Close();

            }

        }

        public override string DownloadObject(object? _)
        {

            return "Sucessfuly downlowd all objects";

        }


        public override byte? ImportObject(object? _)
        {

            Console.WriteLine("ok");

            return null;

        }


        public int CompareTo(object? obj)
        {

            if (obj is UserListenerSSRc userL)
            {

                if (userL.Port == Port)
                {

                    return 1;

                }

            }

            return 0;

        }
    }

}
