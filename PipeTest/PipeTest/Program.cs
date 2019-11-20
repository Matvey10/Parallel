using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Diagnostics;

namespace PipeTest
{   // 4 клиентских процесса считают факториал числа, которое им передается через аргументы процесса, они должны вернуть
    // его в серверный поток
    class ProgramPipeTest
    {
        const int numThreads = 4;
        static ResultsList Results;
        static int finishedTasks = 0;
        private static List<Process> processes = new List<Process>();
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ServerProcess();
            }
            else
            {
                ClientProcess(args);
            }
        }
        public static void ServerProcess()
        { 
            //запускаю 4 потока-сервера, каждый из которых работает с 1 клиентом
            int i;
            Results= new ResultsList(); //хранит результаты факториалов
            Thread[] servers = new Thread[numThreads];
            for (i = 0; i < numThreads; i++)
            {
                int[] Values = { i + 3, i + 4, i + 5 };
                servers[i] = new Thread((new ParameterizedThreadStart(ServerThread)));
                servers[i].Start(Values);
            }
            Thread.Sleep(1000);
            for (int j = 0; j < numThreads; j++)
            {
                Process process = new Process();
                process.StartInfo.FileName = Process.GetCurrentProcess().MainModule.ModuleName;
                Console.WriteLine(Process.GetCurrentProcess().MainModule.ModuleName);
                process.StartInfo.Arguments = "1";
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.UseShellExecute = true;
                process.EnableRaisingEvents = true;
                //process.Exited += ProcessOnExited;
                processes.Add(process);
            }
            foreach (var pr in processes)
                pr.Start();
            //i = numThreads;
            while (i > 0)
            {
                for (int j = 0; j < numThreads; j++)
                {
                    if (servers[j] != null)
                    {
                        if (servers[j].Join(1000)) // если завершился j-ий поток или через 250 мс
                        {
                            Console.WriteLine("Server thread[{0}] finished.", servers[j].ManagedThreadId);
                            servers[j] = null;
                            i--;    // decrement the thread watch count
                        }
                    }
                }
            }
            Console.WriteLine("\nServer threads exhausted, exiting.");
            Console.ReadKey();
        }
        public static void ServerThread(object obj)
        {
            int[] Values = (int[])obj;
            NamedPipeServerStream pipeServer = new NamedPipeServerStream("mytestpipe", PipeDirection.InOut, numThreads);
            Console.WriteLine("[Server] Pipe created {0}", pipeServer.GetHashCode());
            pipeServer.WaitForConnection();
            Console.WriteLine("[Server] Pipe connection established");
            StreamWriter sw = new StreamWriter(pipeServer);
            StreamReader sr = new StreamReader(pipeServer);
            using (var mutex = new Mutex(false, "MyMutex1"))
            {
                foreach (int val in Values)
                {
                    try
                    {
                        sw.WriteLine(val);
                        sw.Flush();
                        string input = sr.ReadLine();
                        int fn = Convert.ToInt32(input);
                        Console.WriteLine($"факториал = {fn}");
                        mutex.WaitOne();
                        Results.AddResult(fn);
                        Console.WriteLine($"Добавили элемент {fn}");
                        finishedTasks++;
                        if (finishedTasks == 3*numThreads)
                        {
                            Console.WriteLine("Все посчитано");
                            foreach (var res in Results.getResults())
                            {
                                Console.WriteLine(res);
                            }
                        }
                        mutex.ReleaseMutex();
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                sw.WriteLine("");
                sw.Flush();
            }
            //pipeServer.Close();
            Console.WriteLine("Connection lost");
        }
        
       static void ClientProcess(string[] args)
        {
            //int k = Convert.ToInt32(args[0]);
            NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "mytestpipe", PipeDirection.InOut);
            Console.WriteLine("Connecting to server...\n");
            pipeClient.Connect();
            Console.WriteLine("Connect is succesful...\n");
            StreamWriter sw = new StreamWriter(pipeClient);
            StreamReader sr = new StreamReader(pipeClient);
            string input;
            while ((input=sr.ReadLine()) != "")
            {
                try
                {
                    int k = Convert.ToInt32(input);
                    int n = Factorial(k);
                    sw.WriteLine(n);
                    sw.Flush();
                    Console.WriteLine($"факториал {n}");
                    //Thread.Sleep(500);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Console.ReadKey();
        }

       /* public void ThreadStartClient(object obj)
        {
            using (NamedPipeClientStream pipeStream = new NamedPipeClientStream("mytestpipe"))
            {
                // The connect function will indefinately wait for the pipe to become available
                // If that is not acceptable specify a maximum waiting time (in ms)
                pipeStream.Connect();

                Console.WriteLine("[Client] Pipe connection established");
                using (StreamWriter sw = new StreamWriter(pipeStream))
                {
                    sw.AutoFlush = true;
                    string temp;
                    Console.WriteLine(
                       "Please type a message and press [Enter], or type 'quit' to exit the program");
                    while ((temp = Console.ReadLine()) != null)
                    {
                        sw.WriteLine(temp);
                        if (temp == "quit") break;
                        
                    }
                }
            }
        }*/
        public static int Factorial(int n)
        {
            //int n = (int)obj;
            int k = 1;
            for (int i = 1; i <=n; i++)
            {
                k *= i;
            }
            return k;
        }

    }

    
}

/* 
 *  try
                    {
                        string temp;
                        // We read a line from the pipe and print it together with the current time
                        while ((temp = sr.ReadLine()) != null)
                        {
                            if (temp == "quit")
                                break;
                            Console.WriteLine("{0}: {1}", DateTime.Now, temp);
                        }
                    }
 * static int Main(string[] args)
        {
            if (args.Length == 0)
                return MainProcess();
            else
                return SubProcess(args);
        }
        private static List<Process> processes = new List<Process>();
        private static int numOfDoneProcesses = 0;
        private static int numberOfProcesses = 10;
        private static Process savedProcess;
        private static int size;
        private static int numOfStep = 1;
        //private static double mx;
        //public delegate void InOutputHandler();
        //public event InOutputHandler endOfWriting;
        static int MainProcess()
        {
            Console.WriteLine("This is Main Process!");
            Console.WriteLine($"File: {Process.GetCurrentProcess().MainModule.ModuleName}");
            Console.WriteLine("Введите кол-во элементов выборки: ");
            size = Convert.ToInt32(Console.ReadLine());
            int dimension = size / numberOfProcesses;
            int[] X = new int[size];
            Random rand = new Random();
            for (var i = 0; i < size; i++)
            {
                X[i] = rand.Next(1, 1000);
            }
            FileInfo file1 = new FileInfo("C:\\Users\\user\\source\\repos\\ProcessTest\\data.txt");
            if (!file1.Exists)
            {
                using (StreamWriter sw = file1.CreateText())
                {
                    foreach (var el in X)
                        sw.WriteLine(el);
                }
            }
            
                for (int i = 0; i < numberOfProcesses; i++)
                {
                    Process process = new Process();
                    //savedProcess = process;
                    process.StartInfo.FileName = Process.GetCurrentProcess().MainModule.ModuleName;
                    int startPoint = i * dimension;
                    int endPoint = startPoint + dimension - 1;
                    process.StartInfo.Arguments = $"{startPoint} {endPoint} {numOfStep} 0";
                    process.StartInfo.CreateNoWindow = false;
                    process.StartInfo.UseShellExecute = true;
                    process.EnableRaisingEvents = true;
                    process.Exited += ProcessOnExited;
                    processes.Add(process);
                    //process.Start();
                }
                foreach (var pr in processes)
                    pr.Start();
            Console.ReadKey();
            return 0;
        }

        private static void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine($"Process {((Process)sender).Id} send: {e.Data}");
        }

        private static void ProcessOnExited(object sender, EventArgs e)
        {
            //processes.Remove((Process)sender);
            using (var mutex = new Mutex(false, "MyMutex1"))
            {
                mutex.WaitOne();
                Console.WriteLine($"Exited type:{sender.GetType().FullName}");
                //Console.WriteLine($"Same process:{sender == savedProcess}");
                numOfDoneProcesses++;
                Console.WriteLine($"Количество отработанных процессов:{numOfDoneProcesses}");
                if (numOfDoneProcesses == numberOfProcesses)
                {
                    //endOfWriting?.Invoke();
                    processes.Clear();
                    if (numOfStep == 1)
                        computeMx();
                    else if (numOfStep == 2)
                        computeDx();
                }
                mutex.ReleaseMutex();
            }
            Console.ReadKey();
            //Console.WriteLine($"Same process:{savedProcess.ExitCode}");
        }
        private static void computeMx()
        {
            double res = 0;
            string line;
            string writePath = "C:\\Users\\user\\source\\repos\\ProcessTest\\dataOutput.txt";
            try
            {
                using (StreamReader sr = new StreamReader(writePath, System.Text.Encoding.Default))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        res += Convert.ToDouble(line);
                        //res += Convert.ToDouble(sr.ReadLine()); 
                        //Console.WriteLine(res);
                    }
                }
                Console.WriteLine($"Итоговая сумма {res}");
                double mx = res / size;
                Console.WriteLine($"Выборочное среднее: {mx}");
                numOfStep = 2;
                numOfDoneProcesses = 0;
                File.Delete(writePath);
                computeDxStart(mx);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
        private static void computeDxStart(double mx)
        {
            //int numberOfProcesses = 10;
            int dimension = size / numberOfProcesses;
            for (int i = 0; i < numberOfProcesses; i++)
            {
                Process process = new Process();
                process.StartInfo.FileName = Process.GetCurrentProcess().MainModule.ModuleName;
                int startPoint = i * dimension;
                int endPoint = startPoint + dimension - 1;
                process.StartInfo.Arguments = $"{startPoint} {endPoint} {numOfStep} {mx}";
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.UseShellExecute = true;
                process.EnableRaisingEvents = true;
                process.Exited += ProcessOnExited;
                processes.Add(process);
            }
            foreach (var pr in processes)
                pr.Start();
        }
        private static void computeDx()
        {
            double res = 0;
            string line;
            string writePath = "C:\\Users\\user\\source\\repos\\ProcessTest\\dataOutput.txt";
            try
            {
                using (StreamReader sr = new StreamReader(writePath, System.Text.Encoding.Default))
                {
                    while ((line = sr.ReadLine()) != null)
                    { 
                        res += Convert.ToDouble(line); 
                        //Console.WriteLine(res); 
                    }
                    //res += Convert.ToDouble(sr.ReadLine());
                }
                Console.WriteLine($"Итоговая сумма {res}");
                double Dx = res / size;
                Console.WriteLine($"Выборочная дисперсия: {Dx}");
            } 
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static int SubProcess(string[] args)
        {
            var startTime = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine($"This is SUB Process! {args[0]}, {args[1]}, {args[2]}, {args[3]}");
            int startPoint = Convert.ToInt32(args[0]);
            int endPoint = Convert.ToInt32(args[1]);
            int numOfStep = Convert.ToInt32(args[2]);
            double mx = Convert.ToDouble(args[3]);
            string readPath = "C:\\Users\\user\\source\\repos\\ProcessTest\\data.txt";
            double tmpSum;
            if (numOfStep == 1) 
                tmpSum = computeSumforMx(startPoint, endPoint, readPath);
            else
                tmpSum = computeSumforDx(startPoint, endPoint, mx, readPath);
            string writePath = "C:\\Users\\user\\source\\repos\\ProcessTest\\dataOutput.txt";
            using (var mutex = new Mutex(false, "MyMutex"))
            {
                try
                {
                    mutex.WaitOne();
                    using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                    {
                        sw.WriteLine(tmpSum);
                        sw.Flush();
                        sw.BaseStream.Flush();
                    }
                    mutex.ReleaseMutex();
                    //Console.WriteLine("Запись выполнена");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            startTime.Stop();
            var resultTime = startTime.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
            resultTime.Hours,
            resultTime.Minutes,
            resultTime.Seconds,
            resultTime.Milliseconds);
            Console.WriteLine($"Время выполнения процесса: {elapsedTime}");
            Console.ReadKey();
            return 0;
            //Console.ReadKey();
        }
        static int computeSumforMx(int startPoint, int endPoint, string writePath)
        {
            IEnumerable<string> result = File.ReadLines(writePath).Skip(startPoint).Take(endPoint - startPoint + 1);
            int tmpSum = 0;
            foreach (string str in result)
            {
                int val = Convert.ToInt32(str);
                tmpSum += val;
                //Console.WriteLine(val);
            }
            return tmpSum;
        }
        static double computeSumforDx(int startPoint, int endPoint, double mx, string writePath)
        {
            IEnumerable<string> result = File.ReadLines(writePath).Skip(startPoint).Take(endPoint - startPoint + 1);
            double tmpSum = 0;
            foreach (string str in result)
            {
                double val = Convert.ToDouble(str);
                tmpSum += Math.Pow(val - mx, 2);
                //Console.WriteLine(val - mx);
            }
            return tmpSum;
        }


    }
}
*/
