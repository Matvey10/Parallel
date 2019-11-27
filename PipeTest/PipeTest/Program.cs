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
        const int numThreads = 10;
        static ResultsList Results;
        static List<NamedPipeServerStream> pipes;
        static int finishedTasks = 0;
        private static List<Process> processes = new List<Process>();
        private static List<int> Values;
        private static int Size;
        private static int numOfStep = 1;
        private static double mx;
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
        class ThreadStartInfo
        {
            private int startPoint;
            private int endPoint;
            public ThreadStartInfo(int startPoint, int endPoint)
            {
                this.startPoint = startPoint;
                this.endPoint = endPoint;
            }
            public int getStartPoint()
            {
                return startPoint;
            }
            public int getEndPoint()
            {
                return endPoint;
            }
        }

        public static void ServerProcess()
        { 
            //запускаю 10 потока-сервера, каждый из которых работает с 1 клиентом
            int i;
            Console.WriteLine("Введите кол-во элементов выборки");
            Size = Convert.ToInt32(Console.ReadLine());
            int dimension = Size / numThreads;
            Results= new ResultsList(); //хранит результаты сумм
            Thread[] servers = new Thread[numThreads];
            pipes = new List<NamedPipeServerStream>();
            string writePath = "C:\\Users\\user\\source\\repos\\PipeTest\\data.txt";
            FileInfo file1 = new FileInfo(writePath);
            if (file1.Exists)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(writePath, System.Text.Encoding.Default))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            int val = Convert.ToInt32(line);
                            Values.Add(val);//new 
                        }
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Values = new List<int>();
                Random rand = new Random();
                for (int j = 0; j < Size; j++)
                {
                    Values.Add(rand.Next(1, 1000));
                }
            }            
            for (i = 0; i < numThreads; i++)
            {
                var numOfThread = i;
                NamedPipeServerStream pipeServer = new NamedPipeServerStream("mytestpipe"+numOfThread.ToString(), PipeDirection.InOut, numThreads);
                pipes.Add(pipeServer);
                int startPoint = i * dimension;
                int endPoint = startPoint + dimension - 1;
                ThreadStartInfo threadStartInfo = new ThreadStartInfo(startPoint, endPoint);
                servers[i] = new Thread((new ParameterizedThreadStart(ServerThread)));
                servers[i].Start(threadStartInfo);
            }
            Thread.Sleep(1000);
            for (int j = 0; j < numThreads; j++)
            {
                var numOfThread = j; // номер потока связанного с создаваемым процессом
                Process process = new Process();
                process.StartInfo.FileName = Process.GetCurrentProcess().MainModule.ModuleName;
                Console.WriteLine(Process.GetCurrentProcess().MainModule.ModuleName);
                int startPoint = j * dimension;
                int endPoint = startPoint + dimension - 1;
                if (numOfStep == 1)
                {
                    process.StartInfo.Arguments = $"{numOfThread} {numOfStep}";
                }
                else
                {
                    process.StartInfo.Arguments = $"{numOfThread} {numOfStep} {mx}";
                }
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.UseShellExecute = true;
                process.EnableRaisingEvents = true;
                process.Exited += ProcessOnExited;
                processes.Add(process);
            }
            foreach (var pr in processes)
                pr.Start();
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
        private static void ProcessOnExited(object sender, EventArgs e)
        {
            //processes.Remove((Process)sender);
            using (var mutex = new Mutex(false, "MyMutex2"))
            {
                mutex.WaitOne();
                finishedTasks++;
                Console.WriteLine($"Exited type:{sender.GetType().FullName}");
                Console.WriteLine($"Количество отработанных процессов:{finishedTasks}");
                if (finishedTasks == numThreads)
                {
                    Console.WriteLine("Все посчитано");
                    foreach (var res in Results.getResults())
                    {
                        Console.WriteLine(res);
                    }
                    processes.Clear();
                    if (numOfStep == 1)
                    {
                        mx = ComputeMx();
                        Console.WriteLine($"мат ожидание = {mx}");
                        numOfStep = 2;
                        Results.Clear();
                        finishedTasks = 0;
                        ComputeDxStart();
                    }
                    else if (numOfStep == 2)
                    {
                        double dx = ComputeDx();
                        Console.WriteLine($"дисперсия = {dx}");
                    }
                }
                mutex.ReleaseMutex();
            }
            Console.ReadKey();
        }
        public static void ServerThread(object obj)
        {
            ThreadStartInfo threadStartInfo = (ThreadStartInfo)obj;
            int startPoint = threadStartInfo.getStartPoint();
            int endPoint = threadStartInfo.getEndPoint();
            Console.WriteLine($"start {startPoint} end {endPoint}");
            NamedPipeServerStream pipeServer = pipes[0];
            pipes.RemoveAt(0);
            //pipeServer.WaitForPipeDrain
            //NamedPipeServerStream pipeServer = new NamedPipeServerStream("mytestpipe", PipeDirection.InOut, numThreads);
            Console.WriteLine("[Server] Pipe created {0}", pipeServer.GetHashCode());
            pipeServer.WaitForConnection();
            Console.WriteLine("[Server] Pipe connection established");
            StreamWriter sw = new StreamWriter(pipeServer);
            StreamReader sr = new StreamReader(pipeServer);
            using (var mutex = new Mutex(false, "MyMutex1"))
            {
                for (int i = startPoint; i <= endPoint; i++ )
                {
                    try
                    {
                        sw.WriteLine(Values[i]);//закончил здесь, надо передать все числа процессу
                        sw.Flush();
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                sw.WriteLine("");
                sw.Flush();
                string input = sr.ReadLine();
                Console.WriteLine($"переданная сумма {input}");
                double sum = Convert.ToDouble(input);
                Console.WriteLine($"сумма = {sum}");
                mutex.WaitOne();
                Results.AddResult(sum);
                Console.WriteLine($"Добавили сумму {sum}");
                mutex.ReleaseMutex();
            }
            //pipeServer.Close();
            Console.WriteLine("Connection lost");
        }
        
       static void ClientProcess(string[] args)
        {
            NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "mytestpipe"+args[0].ToString(), PipeDirection.InOut);
            if (Convert.ToInt32(args[0]) == 2) { Thread.Sleep(3000); }
            Console.WriteLine("Connecting to server...\n");
            pipeClient.Connect();
            Console.WriteLine("Connect is succesful...\n");
            StreamWriter sw = new StreamWriter(pipeClient);
            StreamReader sr = new StreamReader(pipeClient);
            string input;
            double sum = 0;
            if (args[1] == "1")
            {
                while ((input = sr.ReadLine()) != "")
                {
                    int k = Convert.ToInt32(input);
                    sum += k;
                    //Console.WriteLine($"{k} текущая сумма {sum}");
                }
            }
            else
            {
                double mx = Convert.ToDouble(args[2]);
                while ((input = sr.ReadLine()) != "")
                {
                    double k = Convert.ToDouble(input);
                    sum += Math.Pow(k-mx,2);
                    //Console.WriteLine($"{k} текущая сумма {sum}");
                }
            }
            Console.WriteLine($"итоговая частичная сумма {sum}");
            sw.WriteLine(sum);
            sw.Flush();
            Console.ReadKey();
        }
        static double ComputeMx()
        {
            double mx = 0;
            foreach (int res in Results.getResults())
            {
                mx += res;
            }
            return mx/Size;
        }
        static double ComputeDx()
        {
            double dx = 0;
            foreach (int res in Results.getResults())
            {
                dx += res;
            }
            return dx / Size;
        }
        static void ComputeDxStart ()
        {
            Thread[] servers = new Thread[numThreads];
            for (int i = 0; i < numThreads; i++)
            {
                var numOfThread = i;
                NamedPipeServerStream pipeServer = new NamedPipeServerStream("mytestpipe" + numOfThread.ToString(), PipeDirection.InOut, numThreads);
                pipes.Add(pipeServer);
                int dimension = Size / numThreads;
                int startPoint = i * dimension;
                int endPoint = startPoint + dimension - 1;
                ThreadStartInfo threadStartInfo = new ThreadStartInfo(startPoint, endPoint);
                servers[i] = new Thread((new ParameterizedThreadStart(ServerThread)));
                servers[i].Start(threadStartInfo);
            }
            Thread.Sleep(1000);
            for (int j = 0; j < numThreads; j++)
            {
                var numOfThread = j; // номер потока связанного с создаваемым процессом
                Process process = new Process();
                process.StartInfo.FileName = Process.GetCurrentProcess().MainModule.ModuleName;
                Console.WriteLine(Process.GetCurrentProcess().MainModule.ModuleName);
                if (numOfStep == 1)
                {
                    process.StartInfo.Arguments = $"{numOfThread} {numOfStep}";
                }
                else
                {
                    process.StartInfo.Arguments = $"{numOfThread} {numOfStep} {mx}";
                }
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.UseShellExecute = true;
                process.EnableRaisingEvents = true;
                process.Exited += ProcessOnExited;
                processes.Add(process);
            }
            foreach (var pr in processes)
                pr.Start();
        }

    }

    
}

/* 
 *  try
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
