using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
//2 procesai keičia a ir b reikšmes į a=a-10 ir b=b+10
//3procesai skaito a ir b reikšmes ir spausdina į ekraną jų reikšmes ir patį procesą
//Keisti galima tik tada, kai visi trys procesai perskaito a ir b
//Veiksmai vykdomi,kol a-b > 20
namespace Lab2GynimasGege
{
    class Program
    {
        public static CustomMonitor MyMonitor = new CustomMonitor();
        public static Changer writer = new Changer(MyMonitor);
        public static Printer reader = new Printer(MyMonitor);

        public static int PrintedCount = 0;
        static void Main(string[] args)
        {
           
            List<Thread> threads = new List<Thread>();
            for (int i = 1; i < 3; i++)
            {
                threads.Add(newWriter(i));
            }

            for (int i = 3; i < 6; i++)
            {
                threads.Add(newReader(i));
            }
            foreach (Thread thread in threads)
            {
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }
     
        public static Thread newWriter(int i)
        {
            //  Console.WriteLine("Changer metodas ir jo reiksme i: " + i);
            return new Thread(x => writer.Change(i));
        }

        public static Thread newReader(int i)
        {
            // Console.WriteLine("Printer metodas ir jo reiksme i: " + i);
            return new Thread(x => reader.Write(i)); ;
        }
    }
    public class Changer
    {
        private CustomMonitor _monitor;
        public Changer(CustomMonitor monitor)
        {
            _monitor = monitor;
        }

        public void Change(int place)
        {
            while (_monitor.a - _monitor.b > 20)
            {
                _monitor.ChangeValue(place);
            }
        }
    }

    public class Printer
    {
        private CustomMonitor _monitor;
     
        public Printer(CustomMonitor monitor)
        {
            _monitor = monitor;
        }

        public void Write( int place)
        {
            while (_monitor.a -_monitor.b > 20)
            {
                _monitor.ReadValue(place);
            }
        }
    }
    public class CustomMonitor
    {
        private readonly object _locker;      
        public int a;
        public int b;
        private int counter; //to check when all 3processes read a and b
        private bool CanChange;
        private bool CanPrint;
       
        private List<int> readers;
        // public int outputCounter = 0;
        public const int CmaxOutput = 20;

        public CustomMonitor()
        {
            _locker = new object();
            readers = new List<int>();


            a = 100;
            b = 0;
            counter = 0;
            CanPrint = true; 
            CanChange = false; //galima keisti tik,kai perskaito
        }


        

    /*    bool IsAll3ThreadsRead()
        {
            for (int i = 0; i < 3; i++)
                if (read[i])
                    return true;
            return false;
        }*/
        public void ChangeValue(int gijosNr) //vieta - iterpimo vieta
        {
            lock (_locker)
            {
                while (!CanChange) { Monitor.Wait(_locker); }
              //  if (a - b > 20) 
              //  {
                    a -= 10;
                    b += 10;
                   
                    CanPrint = true;
                    CanChange = false;
             //   }
                   
                Monitor.PulseAll(_locker);         
            }
        }

        public void ReadValue(int gijosNr)//, int thredNo, , ref int threadOutCount)
        {
            lock (_locker)
            {
                while (!CanPrint) { Monitor.Wait(_locker); }

               // if (!readers.Contains(gijosNr)) //be šito neveikia
               // {
                    if (a - b > 20)  //kodėl nesustoja laiku dirbti, jei čia nėra if'o šito?
                    {
                        Console.WriteLine("a: " + a + " b: " + b + " gija: " + gijosNr);
                        readers.Add(gijosNr);
                        counter++;
                        if (counter == 3)
                        {
                            Console.WriteLine("--------------------------------");
                            CanChange = true;
                            CanPrint = false;
                            readers.Clear();
                            counter = 0;
                        }
                    }
               // }

                Monitor.PulseAll(_locker);
            }
        } 
    }
}

