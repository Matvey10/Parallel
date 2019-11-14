using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ProcessTest2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите кол-во элементов выборки: ");
            int size = Convert.ToInt32(Console.ReadLine());
            Random rand = new Random();
            using (ValuesContext db = new ValuesContext())
            {
                for (int i = 0; i < size; i++)
                {
                    Integer val = new Integer(i, rand.Next(1,1000));
                    db.Values.Add(val);
                }
                db.SaveChanges();
                Console.WriteLine("Объекты успешно сохранены");
                var Values = db.Values;
                foreach (Integer val in Values)
                {
                    Console.WriteLine(val.value);
                }
            }
            Console.Read();
        }
    }
}
/*using ParallelLaba1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Linq;

namespace SimpleProcessExample
{
    class Program
    {
        static int Main(string[] args)
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
                Console.WriteLine($"Same process:{sender == savedProcess}");
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
            //Console.ReadKey();
            return 0;
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
}*/