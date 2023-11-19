using System.Net.Sockets;
using System.Net;
using System.Text;
using MyCommand;
using MyProc;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Diagnostics;
using System.Windows.Forms;
using Serialization;

namespace WinForms_Server_TaskManager
{
    [DataContract]
    public partial class FormServer : Form
    {
        [DataMember]
        public SynchronizationContext uiContext;
        [DataMember]
        MyCommand.MyCommand myCommand;
        [DataMember]
        MyProc.MyProcess myProcess;
        Serialization_Deserialization serializer = new Serialization_Deserialization();

        public FormServer()
        {
            InitializeComponent();
            // Получим контекст синхронизации для текущего потока 
            uiContext = SynchronizationContext.Current;
        }

        /*

        SOCKET – это некоторое логическое гнездо, которое позволяет двум приложениям обмениватся информацией 
        по сети не задумываяся о месте расположения. SOCKET – это комбинация IP-address и номера порта.

        Internet Protocol (IP) - широко используемый протокол как в локальных, так и в глобальных сетях.
        Этот протокол не требует установления соединения и не гарантирует доставку данных. Поэтому для
        передачи данных поверх IP используются два протокола более высокого уровня: TCP, UDP.

        Transmission Control Protocol (TCP) реализует связь с установлением соединения, обеспечивая 
        безошибочную передачу данных между компьютерами. 

        Связь без установления соединения выполняется при помощи User Datagram Protocol (UDP). Не гарантируя
        надёжности, UDP может осуществлять передачу данных множеству адресатов и принимать данные от множества
        источников. Например, данные, отправляемые клиентом на сервер, передаются немедленно, независимо от того,
        готов ли сервер к приёму. При получении данных от клиента, сервер не подтверждает их приём. Данные 
        передаются в виде дейтаграмм. И TCP, и UDP передают данные по IP, поэтому обычно говорят об использовании 
        TCP/IP или UDP/IP.
        */

        // обслуживание очередного запроса будем выполнять в отдельном потоке
        
        
        private async Task HandleClientRequest(Socket handler)
        {
           await Task.Run(() =>
            {
                try
                {
                    string client = null;
                    string data = null;
                    byte[] bytes = new byte[1024];
                    // Получим от клиента DNS-имя хоста.
                    // Метод Receive получает данные от сокета и заполняет массив байтов, переданный в качестве аргумента
                    int bytesRec = handler.Receive(bytes); // Возвращает фактически считанное число байтов
                    client = Encoding.Default.GetString(bytes, 0, bytesRec); // конвертируем массив байтов в строку
                    client += "(" + handler.RemoteEndPoint.ToString() + ")";
                   
                    while (true)
                    {
                        bytesRec = handler.Receive(bytes); // принимаем данные, переданные клиентом. Если данных нет, поток блокируется
                        if (bytesRec == 0)
                        {
                            handler.Shutdown(SocketShutdown.Both); // Блокируем передачу и получение данных для объекта Socket.
                            handler.Close();                       // закрываем сокет
                            return;
                        }
                        data = Encoding.Default.GetString(bytes, 0, bytesRec); // конвертируем массив байтов в строку
                        
                        // uiContext.Send отправляет синхронное сообщение в контекст синхронизации
                        // SendOrPostCallback - делегат указывает метод, вызываемый при отправке сообщения в контекст синхронизации. 
                        //uiContext.Send(d => listBox1.Items.Add(client) /* Вызываемый делегат SendOrPostCallback */,
                        //    null /* Объект, переданный делегату */); // добавляем в список имя клиента

                        uiContext.Send(d => listBox1.Items.Add(data), null); // добавляем в список сообщение от клиента

                        //MessageBox.Show(data);
                        myCommand = serializer.DeserializeObject<MyCommand.MyCommand>(Encoding.Default.GetBytes(data), bytesRec);// десериализуем JSON в объект MyCommand
                        List<MyProc.MyProcess> processList;
                        switch (myCommand.NameOfCommand)
                        {
                            case "ListProcess":
                                // получаем список процессов
                                 processList = ListProcess();
                                // cериализуем список процессов в формат JSON с использованием  метода SerializeObject
                                byte[] jsonDataBytes = serializer.SerializeObject(processList);                               
                                handler.Send(jsonDataBytes);// oтправляем данные клиенту

                                break;
                            case "CreateProcess":

                                CreateProcess(); // команда для создания нового процесса                                               
                                processList = ListProcess();                    
                                byte[] jsonDataBytes1 = serializer.SerializeObject(processList);
                                handler.Send(jsonDataBytes1);

                                break;
                            case "KillProcess":
                               // MessageBox.Show(myCommand.IDProcess.ToString());

                                KillProcess(myCommand.IDProcess); // обработка команды для завершения процесса
                                processList = ListProcess();
                                byte[] jsonDataBytes2 = serializer.SerializeObject(processList);
                                handler.Send(jsonDataBytes2);

                                break;
                            default:
                                MessageBox.Show("Unknown command");
                                break;
                        }
                    }
                        //Отправляем ответ клиенту
                        //string response = "Сервер успешно обработал ваш запрос";
                        //byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                        //handler.Send(new ArraySegment<byte>(responseBytes), SocketFlags.None);

                        //handler.Shutdown(SocketShutdown.Both);
                        //handler.Close();


                }
                catch (Exception ex)
                {
                    MessageBox.Show("Server: " + ex.Message);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            });
        }

        //  ожидать запросы на соединение будем в отдельном потоке
        private async void Accept()
        {
            await Task.Run(() =>
            {
                try
                {
                    // установим для сокета адрес локальной конечной точки
                    // уникальный адрес для обслуживания TCP/IP определяется комбинацией IP-адреса хоста с номером порта обслуживания
                    IPEndPoint ipEndPoint = new IPEndPoint(
                    IPAddress.Any /* Предоставляет IP-адрес, указывающий, что сервер должен контролировать действия клиентов на всех сетевых интерфейсах.*/,
                    49152 /* порт */);

                    // создаем потоковый сокет
                    Socket sListener = new Socket(AddressFamily.InterNetwork /*схема адресации*/, SocketType.Stream /*тип сокета*/, ProtocolType.Tcp /*протокол*/ );
                    /* Значение InterNetwork указывает на то, что при подключении объекта Socket к конечной точке предполагается использование IPv4-адреса.
                       SocketType.Stream поддерживает надежные двусторонние байтовые потоки в режиме с установлением подключения, без дублирования данных и 
                       без сохранения границ данных. Объект Socket этого типа взаимодействует с одним узлом и требует предварительного установления подключения 
                       к удаленному узлу перед началом обмена данными. Тип Stream использует протокол Tcp и схему адресации AddressFamily.
                     */

                    // Чтобы сокет клиента мог идентифицировать потоковый сокет TCP, сервер должен дать своему сокету имя
                    sListener.Bind(ipEndPoint); // Свяжем объект Socket с локальной конечной точкой.

                    // Установим объект Socket в состояние прослушивания.
                    sListener.Listen(10 /* Максимальная длина очереди ожидающих подключений.*/ );
                   
                    while (true)
                    {
                        /* Блокируется поток до поступления от клиента запроса на соединение. При этом устанавливается связь имен клиента и сервера. 
                           Метод Accept извлекает из очереди ожидающих запросов первый запрос на соединение и создает для его обработки новый сокет.
                           Хотя новый сокет создан, первоначальный сокет продолжает слушать и может использоваться с многопоточной обработкой для 
                           приема нескольких запросов на соединение от клиентов. Сервер не должен закрывать слушающий сокет, который продолжает работать
                           наряду с сокетами, созданными методом Accept для обработки входящих запросов клиентов.
                         */
                        Socket handler = sListener.Accept();
                        // обслуживание текущего запроса будем выполнять в отдельной асинхронной задаче
                        // Receive(handler);
                        HandleClientRequest(handler);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Server: " + ex.Message);
                }
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Accept();
        }
        
        private List<MyProc.MyProcess> ListProcess()
        {
            List<MyProc.MyProcess> processList = new List<MyProc.MyProcess>();
            try
            {
                uiContext.Send(d => listBox1.Items.Clear(), null);

                // получаем список всех запущенных процессов
                Process[] processes = Process.GetProcesses();

                // добавляем каждый процесс в ListBox в UI потоке и в список processList
                foreach (Process process in processes)
                {
                    uiContext.Send(d =>
                    {
                        listBox1.Items.Add(new MyProc.MyProcess
                        {
                            ProcessId = process.Id,
                            Name = process.ProcessName
                        });

                        processList.Add(new MyProcess
                        {
                            ProcessId = process.Id,
                            Name = process.ProcessName
                        });
                    }, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return processList;
        }

        private void CreateProcess()
        {
            try
            {
                // получаем путь из объекта MyCommand
                string fileName = myCommand.Path;
                // проверяем, что путь к файлу не пустой
                if (!string.IsNullOrEmpty(fileName))
                {
                    // создаем новый процесс
                    Process proc = new Process();
                    // устанавливаем путь 
                    proc.StartInfo.FileName = fileName;
                    proc.Start();

                    ListProcess();
                }
                else
                {
                    MessageBox.Show("Enter the path to the file!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void KillProcess(int processId)
        {
            try
            {
                // получаем процесс по его идентификатору
                Process process = Process.GetProcessById(processId);
                process.Kill();
                ListProcess();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сервер: " + ex.Message);
            }
        }


    } 
}


