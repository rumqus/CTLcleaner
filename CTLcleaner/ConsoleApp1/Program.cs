using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{

    class Program
    {
        static void Main(string[] args)
        {
            string mainPath = $@"out\ENV"; // корневой каталог в котором происходит поиск файлов
            string removePath = $@"removedCTL\"; // папка в которую переносится файл
            string[] allDirs; // массив со всеми папками out
            string tempFile; // файл
            string textPattern = "*.CTL"; // паттерн поиска файлов
            double daysTriger = 0; // количество дней
            DateTime nowDate = DateTime.Now; // текущая дата, сегодняшний день

            GetAllDirs();
            GetFilesInDir();
            Console.ReadLine();

            /// <summary>
            /// Метод инициализации массива с путями до подкаталогов out/ENV
            /// выводит в консоль имена всех подкаталогов
            /// </summary>
            void GetAllDirs()
            {
                allDirs = Directory.GetDirectories(mainPath);
                if (allDirs != null)
                {
                    for (int i = 0; i < allDirs.Length; i++)
                    {
                        Console.WriteLine(allDirs[i]);
                    }
                }
                else
                {
                    Console.WriteLine("ERROR - root array is null");
                }
            }

            /// <summary>
            /// Основной метод обработки
            /// Перебирает файлы в каталоге и сравнивает с условиями
            /// </summary>
            void GetFilesInDir()
            {
                if (allDirs != null)
                {
                    for (int i = 0; i <allDirs.Length; i++)
                    {
                        if (Directory.GetFiles(allDirs[i]).Length > 0)
                        {
                            string[] tempLocalDir = Directory.GetFiles(allDirs[i], textPattern); // отбираем все файлы с расширением CTL
                            Console.WriteLine($@"папка {allDirs[i]} считана - ОК");
                            CompareFiles(tempLocalDir);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ERROR - root array is null");
                }
            }

            /// <summary>
            /// Метод сравнения файлов с уловиями
            /// Перебирает файлы в каталоге и сравнивает с условиями
            /// </summary>
            void CompareFiles(string[] tempDir) 
            {
                for (int i = 0; i < tempDir.Length; i++)
                {
                    DateTime fileDate = new FileInfo(tempDir[i]).CreationTime; // получаем дату создания файла
                    Console.WriteLine($@"{tempDir[i]} {fileDate.ToShortDateString()}");
                    TimeSpan diff = nowDate.Subtract(fileDate);
                    if (diff.TotalDays > daysTriger)
                    {
                        RemoveFile(tempDir[i]);
                    }

                }
            
            }

            /// <summary>
            /// Метод переноса\удаления
            /// принимает файл в качестве аргумента, переносит в путь removePath
            /// </summary>
            void RemoveFile(string pathToFile) 
            {
                string newPath = $@"{removePath}{Path.GetFileName(pathToFile)}";
                if (!File.Exists(newPath))
                {
                    File.Move(pathToFile, newPath);
                    Console.WriteLine($@"file {Path.GetFileName(pathToFile)} moved to RemovedCTL");
                }
                else 
                {
                    Console.WriteLine();
                }
                
            }
        }

        void WriteError(string message) 
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        void WriteOK(string message) 
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

    }
}
