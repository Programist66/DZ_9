using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace DZ_9
{
    abstract class Storage
    {
        protected string name;
        protected string model;
        protected double memorySize;
        protected double freeMemorySize;


        public abstract double GetMemorySize();
        public abstract void CopyData(double dataSize);
        public abstract double GetFreeMemory();
        public abstract void GetDeviceInfo();

        public abstract double GetTimeForCopy(double dataSize);
    }

    class Flash : Storage
    {
        
        private double usbSpeed;

        public Flash(string name, string model, double memorySize, double usbSpeed)
        {
            this.name = name;
            this.model = model;
            this.memorySize = memorySize;
            this.usbSpeed = usbSpeed;
            freeMemorySize = memorySize;
        }

        public override double GetMemorySize()
        {
            return memorySize;
        }

        public override void CopyData(double dataSize)
        {
            if (dataSize > freeMemorySize)
            {
                throw new Exception("not enough space");
            }
            else
            {
                freeMemorySize -= dataSize;
            }
        }

        public override double GetFreeMemory()
        {
            return freeMemorySize;
        }

        public override void GetDeviceInfo()
        {
            Console.WriteLine($"Name: {name}\nModel: {model}\nSpeed: {usbSpeed}\nMemory Size: {memorySize}\nFree Memory Size: {freeMemorySize}");
        }

        public override double GetTimeForCopy(double dataSize)
        {
            return dataSize / usbSpeed;
        }
    }

    class DVD : Storage
    {
        private double readWriteSpeed;
        private string type;

        public DVD(string name, string model, double readWriteSpeed, string type)
        {
            this.name = name;
            this.model = model;
            this.readWriteSpeed = readWriteSpeed;
            this.type = type;
            if (type == "unilateral")
                memorySize = 4.7;
            else if (type == "bilateral")
                memorySize = 9.0;
            else
            {
                throw new Exception("Inncorect type disk");
            }
            freeMemorySize = memorySize;
        }

        public override double GetMemorySize()
        {
            return memorySize;
        }

        public override void CopyData(double dataSize)
        {
            if (dataSize > freeMemorySize)
            {
                throw new Exception("not enough space");
            }
            else
            {
                freeMemorySize -= dataSize;
            }
        }

        public override double GetFreeMemory()
        {
            return freeMemorySize;
        }

        public override void GetDeviceInfo()
        {
            Console.WriteLine($"Name: {name}\nModel: {model}\nType{type}\nSpeed: {readWriteSpeed}\nMemory Size: {memorySize}\nFree Memory Size: {freeMemorySize}");
        }

        public override double GetTimeForCopy(double dataSize)
        {
            return dataSize / readWriteSpeed;
        }
    }

    class HDD : Storage
    {
        private double usbSpeed;
        private double[] partitions;
        private double partitionSize;

        public HDD(string name, string model, double usbSpeed, int countPartitions, double partitionSize)
        {
            this.name = name;
            this.model = model;
            this.usbSpeed = usbSpeed;
            partitions = new double[countPartitions];
            this.partitionSize = partitionSize;
            for (int i = 0; i < partitions.Length; i++)
            {
                partitions[i] = partitionSize;
            }
            memorySize = countPartitions * partitionSize;
            freeMemorySize = memorySize;
        }

        public override double GetMemorySize()
        {
            return memorySize;
        }

        public override void CopyData(double dataSize)
        {
            bool isDone = false;
            for (int i = 0; i < partitions.Length; i++)
            {
                if (dataSize <= partitions[i] && isDone == false)
                {
                    partitions[i] -= dataSize;
                    isDone = true;
                }
            }
            if (isDone == false)
            {
                throw new Exception("not enough space");
            }
        }

        public override double GetFreeMemory()
        {
            memorySize = partitions.Sum();
            return memorySize;
        }

        public override void GetDeviceInfo()
        {
            string s = "";
            for (int i = 0; i < partitions.Length; i++)
            {
                s += $"Disk {i}: {partitions[i]} / {partitionSize}\n";
            }
            Console.WriteLine($"Name: {name}\nModel: {model}\nSpeed: {usbSpeed}\nMemory Free/All\n{s}");
        }
        public override double GetTimeForCopy(double dataSize)
        {
            return dataSize / usbSpeed;
        }
    }

    internal class Program
    {
        static void Main()
        {
            Storage[] storages = new Storage[]
            {
            new Flash("Flash Drive", "Model A", 64, 3.0),
            new DVD("DVD Disc", "Model B", 8.0, "unilateral"),
            new HDD("External HDD", "Model C", 2.0, 2, 500)
            };

            Console.WriteLine($"Total memory: {storages.Sum(s => s.GetMemorySize())}");
            Console.WriteLine($"Total free memory: {storages.Sum(s => s.GetFreeMemory())}");
            storages[2].CopyData(101);
            Console.WriteLine($"Time for Copy on HDD: {storages[2].GetTimeForCopy(101)} sec");            
            Console.WriteLine($"Total memory: {storages.Sum(s => s.GetMemorySize())}");
            Console.WriteLine($"Total free memory: {storages.Sum(s => s.GetFreeMemory())}");         
            foreach ( Storage s in storages ) 
            {
                Console.WriteLine("=================================");
                s.GetDeviceInfo();
            }
            Console.ReadLine();
        }
    }
}
