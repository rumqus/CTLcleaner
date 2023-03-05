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
            string mainPath = ""; // корневой каталог в котором происходит поиск файлов
            string removePath = $@"removedCTL\"; // папка в которую переносится файл
            //string[] allDirs; // массив со всеми папками out
            string[] allDirs; 
            string textPattern = "*.CTL"; // маска файлов
            double daysTriger = 14; // количество дней
            DateTime nowDate = DateTime.Now; // текущая дата, сегодняшний день

            // Основной поток выполнения 
            ReadPath(); // получаем путь к папке ENV
            GetAllDirs(); // Получаем все директории в ENV
            GetFilesInDir(); // ищем м переносим файлы в целевой директории
            Console.ReadLine();

            /// <summary>
            /// Метод инициализации массива с путями до подкаталогов out/ENV
            /// выводит в консоль имена всех подкаталогов
            /// </summary>
            void GetAllDirs()
            {
                allDirs = Directory.EnumerateDirectories(mainPath).ToArray();
                //allDirs = Directory.GetDirectories(mainPath);
                if (allDirs != null) 
                {
                    WriteOK("root path OK");
                    foreach(string dir in allDirs) 
                    {
                        if (Directory.Exists(dir))
                        {
                            WriteOK($@"PATH OK {dir}");
                        }
                        else 
                        {
                            WriteError($@"BAD PATH {dir}");
                        }
                        
                    }
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
                    foreach (string dir in allDirs) 
                    {
                        string[] currentDir = Directory.GetFiles(dir, textPattern);
                        if (currentDir.Length > 0)
                        {
                            for (int i = 0; i < currentDir.Length; i++)
                            {
                                CompareFiles(currentDir[i]);
                            }
                        }
                    }
                }
                else
                {
                    WriteError("ERROR - root array is null");
                }
            }

            /// <summary>
            /// Метод сравнения файлов с уловиями
            /// Перебирает файлы в каталоге и сравнивает с условиями
            /// </summary>
            void CompareFiles(string tempFile)
            {
                DateTime fileDate = new FileInfo(tempFile).CreationTime; // получаем дату создания файла
                Console.WriteLine($@"{tempFile} {fileDate.ToShortDateString()}");
                TimeSpan diff = nowDate.Subtract(fileDate);
                if (diff.TotalDays > daysTriger)
                {
                    RemoveFile(tempFile);
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
                    WriteOK($@"file {Path.GetFileName(pathToFile)} moved to RemovedCTL");
                }
                else
                {
                    WriteError("File alredy exist in RemovedCTL folder");
                }
            }
            /// <summary>
            /// Метод получения пути с расположением папки ENV
            /// принимает файл в качестве аргумента расположение path.ini(в самом path.ini указываем путь к файлу)
            /// </summary>
            void ReadPath() 
            {
                using (StreamReader reader = new StreamReader("path.ini"))
                {
                    string text = reader.ReadToEnd();
                    
                    mainPath = $@"{text}";
                }
                if (Directory.Exists(mainPath))
                {
                    WriteOK("path.ini - read OK");
                }
                else
                {
                    WriteError("path.ini - FAILED");
                    Console.ReadLine();
                }
            }


            //Служебные
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
}
