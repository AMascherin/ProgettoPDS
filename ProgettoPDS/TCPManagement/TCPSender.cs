using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Globalization;

namespace ProgettoPDS
{
    public class TCPSender : INotifyPropertyChanged
    {
       // private int PORT_NO = 13370;

        private TcpClient client;

        UserConfiguration uc = new UserConfiguration();

        private readonly object _UserDatalocker = new object();

        private bool _suspend;
        private double _progress;
        private ImageSource _userImage;
        private string _userName;
        private ICommand _stopCommand;
        private string estimatedUploadTime;

        public bool Suspend
        {
            get
            {
                lock (_UserDatalocker)
                {
                    return _suspend;
                }
            }

            set
            {
                lock (_UserDatalocker)
                {
                    _suspend = value;
                }
            }
        }

        public double Progress
        {
            get
            {
                return _progress;
            }

            set
            {
                _progress = value;
                OnPropertyChanged("Progress");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public ImageSource UserImage
        {
            get
            {
                lock (_UserDatalocker)
                {
                    BitmapImage imagebitmap = new BitmapImage(new Uri(Nu.Imagepath, UriKind.Absolute));
                    return imagebitmap;
                }
            }

            set
            {
                lock (_UserDatalocker)
                {
                    _userImage = value;
                    OnPropertyChanged("UserImage");
                }
            }
        }

        public string UserName
        {
            get
            {
                lock (_UserDatalocker)
                {
                    
                    return Nu.Username;
                }
            }

            set
            {
                lock (_UserDatalocker)
                {
                    _userName = value;
                    OnPropertyChanged("UserName");
                }
            }
        }

        public string EstimatedUploadTime
        {
            get
            {
                lock (_UserDatalocker)
                {

                    return estimatedUploadTime;
                }
            }

            set
            {
                lock (_UserDatalocker)
                {
                    estimatedUploadTime = value;
                    OnPropertyChanged("EstimatedUploadTime");
                }
            }
        }

        public NetworkUser Nu { get; set; }

        public ICommand StopCommand
        {
            get
            {
                if (_stopCommand == null) {

                    _stopCommand = new RelayCommand(

                        param => {
                            this.SetSuspended();
                            Console.WriteLine("Annullato");
                        }

                        );
                }

                return _stopCommand;
            }

        }

        public void SetSuspended() {

            Suspend = true;
            //Models.ActiveTCPSenderManager.RemoveSender(this);
            Models.ActiveTCPSenderManager.TcpSenderCollection.Remove((TCPSender)this);
        }

        public TCPSender(NetworkUser user)
        {
            Nu = user;

            IPAddress ipAd = IPAddress.Parse(Nu.Ipaddress);

            client = new TcpClient(ipAd.ToString(), uc.GetTCPPort());

            Suspend = false;
            Progress = 0;

            //System.Windows.MessageBox.Show("TCP Sender created");
        }

        private void SendData(string pathToObj, NetworkStream nwStream)
        {
            try
            {
                //NetworkStream nwStream = client.GetStream();

                byte[] bytesToSend = File.ReadAllBytes(pathToObj);

                //Send data into stream
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (IOException e)
            {
                Console.WriteLine("IOException: {0}", e);
            }
        }

        private void CloseConnection()
        {
            var ns = client.GetStream();
            ns.Close();         
            client.Close();
            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate {
                SetSuspended();
            });
        }
      

        public void handleFileSend(List<String> filesPathToSend)
        {
            int chunkSize = 2048;

            //---create a NetworkStream---
            NetworkStream nwStream = client.GetStream();

            JObject jsonfile = new JObject();
            int i = 0;
            foreach (String file in filesPathToSend)
            {
                if (File.Exists(file))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    jsonfile.Add(
                         new JProperty("File" + i,
                             new JObject(
                                 new JProperty("nome", fileInfo.Name),
                                 new JProperty("estensione", fileInfo.Extension),
                                 new JProperty("dimensione", fileInfo.Length)
                             )
                         )
                     );
                    i++;
                }
                if (Directory.Exists(file)) {
                    DirectoryInfo d = new DirectoryInfo(file);
                    //Aggiungere al JSon una qualche informazione relativa alla cartella (nome, peso, contenuto)
                    jsonfile.Add(
                        new JProperty("File" + i,
                            new JObject(
                                new JProperty("nome", Path.GetDirectoryName(file)),
                                new JProperty("estensione", "Folder"),
                                new JProperty("dimensione", GetDirectorySize(file))
                            )
                        )
                    );
                    i++;
                }
            }


            byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes((string)jsonfile.ToString());
            nwStream.Write(bytesToSend, 0, bytesToSend.Length); //Send to the server the file information data
            nwStream.Flush();
            byte[] inStream = new byte[chunkSize];

            EstimatedUploadTime = "Waiting for the other user response";

            try
            {
                int clientMessage = nwStream.Read(inStream, 0, inStream.Length); //Leggo il messaggio dell'utente
                String returndata = System.Text.Encoding.UTF8.GetString(inStream).TrimEnd('\0');
                
                if (returndata.Equals("200 OK"))
                {
                    //https://www.codeguru.com/csharp/.net/zip-and-unzip-files-programmatically-in-c.htm
                    EstimatedUploadTime = "Compressing... ";

                    string zipPath = DateTime.Now.ToString("yyyyMMddTHHmmss") + "_zip"+".zip";

                    //Compressing the data to send in a zip archive
                    using (ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                    {
                        foreach (String inputPath in filesPathToSend)
                        {
                            if (Suspend)
                            {
                                if (File.Exists(zipPath))
                                    File.Delete(zipPath);
                                CloseConnection();
                                return;
                            }

                            if (File.Exists(inputPath))
                            {
                                zip.CreateEntryFromFile(inputPath, Path.GetFileName(inputPath), CompressionLevel.Fastest);
                            }
                            else if (Directory.Exists(inputPath))
                            {
                                var directoryInfo = Directory.GetParent(inputPath);

                                foreach (var filePath in System.IO.Directory.GetFiles(inputPath, "*.*", SearchOption.AllDirectories))
                                {
                                    var cleanPath = Path.GetFullPath(filePath);
                                    var relativePath = cleanPath.Replace(directoryInfo.FullName, string.Empty);
                                    using (Stream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                                    using (Stream fileStreamInZip = zip.CreateEntry(relativePath).Open())
                                        fileStream.CopyTo(fileStreamInZip);
                                }

                            }

                            //https://stackoverflow.com/questions/21259703/how-to-receive-large-file-over-networkstream-c
                        }

                        zip.Dispose();
                    }

                    Stopwatch stopWatch = new Stopwatch();
                    List<long> previousTime = new List<long>(); //list for the moving average

                    using (var fileIO = File.OpenRead(zipPath))
                    {
                        var bytesArrayToSend = new byte[1024 * 8];
                        long totalSize = new FileInfo(zipPath).Length; //Total size of the zip in byte

                        long totalByteSent = 0;
                        long byteSentInInterval = 0;
                        int count = 0;
                        long remainingTime = 0;
                        stopWatch.Reset();
                        stopWatch.Start();

                        while ((count = fileIO.Read(bytesArrayToSend, 0, bytesArrayToSend.Length)) > 0)
                        {
                            if (!Suspend)
                            {
                                nwStream.Write(bytesArrayToSend, 0, count);

                                if (stopWatch.Elapsed.TotalMilliseconds > 500)
                                {
                                    //moving average...
                                    remainingTime = (totalSize - totalByteSent) / byteSentInInterval * stopWatch.ElapsedMilliseconds;
                                    previousTime.Add(remainingTime);
                                    if (previousTime.Count > 10)
                                    {
                                        previousTime = previousTime.GetRange(previousTime.Count - 10, 10); //moving average of the last 10 elements
                                    }
                                    long sum = previousTime.Sum();
                                    remainingTime = sum / previousTime.Count();
                                    var date = (new DateTime(1970, 1, 1)).AddMilliseconds(remainingTime);
                                    // Console.WriteLine("Remaining time: " + date.ToString("T", DateTimeFormatInfo.InvariantInfo));
                                    EstimatedUploadTime = "Remaining time: "+date.ToString("T", DateTimeFormatInfo.InvariantInfo);
                                    byteSentInInterval = 0;
                                    stopWatch.Restart();
                                }
                                totalByteSent += count;
                                byteSentInInterval += count;

                                Progress = (int)Math.Floor((double)(totalByteSent * 100 / totalSize));
                                
                            }
                            else
                            {
                                stopWatch?.Stop();
                                break;
                            }
                        }
                    }


                    CloseConnection();
                    if (File.Exists(zipPath))
                        File.Delete(zipPath);

                }
                else
                {
                    System.Windows.MessageBox.Show("Request rejected by the server, closing connection");
                    CloseConnection();
                }
            }
            catch (System.IO.IOException e) {
                Console.WriteLine("Connection interrupted or IO error");
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
                CloseConnection();
            }
        }
                
        public void SendImageRequest(string filename) {
            NetworkStream nwStream = client.GetStream();
            byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes("Send Image");
            nwStream.Write(bytesToSend, 0, bytesToSend.Length); 
            nwStream.Flush();

            lock (this)
            {
                try
                {                    
                    Stream fileStream = File.OpenWrite(filename);
                    byte[] clientData = new byte[2048];
                    int bytesread = nwStream.Read(clientData, 0, clientData.Length); 
                    while (bytesread > 0)
                    {
                        fileStream.Write(clientData, 0, bytesread);
                        bytesread = nwStream.Read(clientData, 0, clientData.Length);
                    }
                    fileStream.Close();
                }
                catch (Exception e)
                {
                    //System.Windows.MessageBox.Show(e.Message);
                    //System.Windows.MessageBox.Show(e.StackTrace);
                }
            }
            nwStream.Close();
            client.Close();
        }

        static long GetDirectorySize(string p)
        {
            // 1.
            // Get array of all file names.
            string[] a = Directory.GetFiles(p, " *.*");

            // 2.
            // Calculate total bytes of all files in a loop.
            long b = 0;
            foreach (string name in a)
            {
                // 3.
                // Use FileInfo to get length of each file.
                FileInfo info = new FileInfo(name);
                b += info.Length;
            }
            // 4.
            // Return total size
            return b;
        }
    }
}

