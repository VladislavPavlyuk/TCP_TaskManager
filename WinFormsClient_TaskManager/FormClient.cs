using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using MyCommand;
using MyProc;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Serialization;

namespace Client_TaskManager
{

    [DataContract]
    public partial class FormClient : Form
    {
        [DataMember]
        public SynchronizationContext uiContext;
        [DataMember]
        MyProc.MyProcess myProcess = new MyProc.MyProcess();
        [DataMember]
        MyCommand.MyCommand myCommand = new MyCommand.MyCommand();
        Socket sock;
        Serialization_Deserialization serializer = new Serialization_Deserialization();


        public FormClient()
        {
            InitializeComponent();
            // получим контекст синхронизации для текущего потока 
            uiContext = SynchronizationContext.Current;
        }

        private async void UpdateProcessList_Click(object sender, EventArgs e)
        {
            // при нажатии кнопки "Обновить список процессов"
            
            Exchange("ListProcess");
        }


        private void CreateProcess_Click(object sender, EventArgs e)
        {
            // при нажатии кнопки "Создать процесс"
          
            Exchange("CreateProcess");
        }

        private void ServerResponse(byte[] bytes, int bytesRec)
        {
            if (bytesRec == 0)
            {
                sock.Shutdown(SocketShutdown.Both);                                // принимаем данные, переданные сервером. Если данных нет, поток блокируется
                sock.Close();
                return;
            }

            listBox1.Items.Clear();
            List<MyProc.MyProcess> processList = serializer.DeserializeObject<List<MyProc.MyProcess>>(bytes, bytesRec);
            foreach (var process in processList)
            {
                listBox1.Items.Add(process);
            }
        }


        private async Task Exchange(string command)
        {
            await Task.Run(() =>
            {
                try
                {
                    MyCommand.MyCommand myCommand = new MyCommand.MyCommand();
                    myCommand.NameOfCommand = command;
                    myCommand.Path = textBox1.Text;
                    if(listBox1.SelectedIndex != -1)
                    myCommand.IDProcess = (listBox1.SelectedItem as MyProc.MyProcess).ProcessId;             // сериализуем объект MyCommand в формат JSON

                    byte[] jsonDataBytes = serializer.SerializeObject(myCommand);
                   
                    // отправляем данные клиенту
                    sock.Send(jsonDataBytes);                   

                    if (command == "ListProcess")                                                      // команда - запрос списка процессов
                    {
                        byte[] bytes = new byte[50000];
                        int bytesRec = sock.Receive(bytes);                                            
                      
                        ServerResponse(bytes, bytesRec);
                        MessageBox.Show("Server (" + sock.RemoteEndPoint.ToString() + ") response: List of the processes got successfully!.");
                    }                
                    else if  (command == "CreateProcess" ) 
                    {
                        byte[] bytes = new byte[50000];
                        int bytesRec = sock.Receive(bytes);
                                            
                        ServerResponse(bytes, bytesRec);
                        MessageBox.Show("Server (" + sock.RemoteEndPoint.ToString() + ") response: New process added successfully!.");
                    }
                    else if( command == "KillProcess")
                    {
                        byte[] bytes = new byte[50000];
                        int bytesRec = sock.Receive(bytes);
                        
                        ServerResponse(bytes, bytesRec);
                        MessageBox.Show("Server (" + sock.RemoteEndPoint.ToString() + ") response: Process killed successfully.");

                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Client: " + ex.Message);
                }
            });
        }

        private async void Connect()
        {
            await Task.Run(() =>
            {
                // соединяемся с удаленным устройством
                try
                {

                    IPAddress ipAddr = IPAddress.Parse(ip_address.Text);
                    // устанавливаем удаленную конечную точку для сокета
                    // уникальный адрес для обслуживания TCP/IP определяется комбинацией IP-адреса хоста с номером порта обслуживания
                    IPEndPoint ipEndPoint = new IPEndPoint(ipAddr /* IP-адрес */, 49152 /* порт */);

                    // создаем потоковый сокет
                    sock = new Socket(AddressFamily.InterNetwork /*схема адресации*/, SocketType.Stream /*тип сокета*/, ProtocolType.Tcp /*протокол*/);
                    /* Значение InterNetwork указывает на то, что при подключении объекта Socket к конечной точке предполагается использование IPv4-адреса.
                      SocketType.Stream поддерживает надежные двусторонние байтовые потоки в режиме с установлением подключения, без дублирования данных и 
                      без сохранения границ данных. Объект Socket этого типа взаимодействует с одним узлом и требует предварительного установления подключения 
                      к удаленному узлу перед началом обмена данными. Тип Stream использует протокол Tcp и схему адресации AddressFamily.
                    */

                    // соединяем сокет с удаленной конечной точкой
                    sock.Connect(ipEndPoint);
                    byte[] msg = Encoding.Default.GetBytes(Dns.GetHostName() /* имя узла локального компьютера */);// конвертируем строку, содержащую имя хоста, в массив байтов
                    int bytesSent = sock.Send(msg); // отправляем серверу сообщение через сокет
                    MessageBox.Show("Client " + Dns.GetHostName() + " connected to " + sock.RemoteEndPoint.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Client: " + ex.Message);
                }
            });
        }


        private void EndProcess_Click(object sender, EventArgs e)
        {

            MyProc.MyProcess selectedProcess = (MyProc.MyProcess)listBox1.SelectedItem;           //  выбранный процесс из списка

            if (selectedProcess != null)
            {                
                myCommand.NameOfCommand = "KillProcess";                              // устанавливаем команду и идентификатор процесса
                myCommand.IDProcess = selectedProcess.ProcessId;
                                                
                Exchange("KillProcess"); // отправляем команду на сервер
            }
            else
            {
                MessageBox.Show("Choose process to kill, please.");
            }
            
        }


        private void buttonConnection_Click(object sender, EventArgs e)
        {
            Connect();
        }

        private void FormClient_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                sock.Shutdown(SocketShutdown.Both); // Блокируем передачу и получение данных для объекта Socket.
                sock.Close(); // закрываем сокет
            }
            catch (Exception ex)
            {
                MessageBox.Show("Client: " + ex.Message);
            }
        }

       
    }
}
