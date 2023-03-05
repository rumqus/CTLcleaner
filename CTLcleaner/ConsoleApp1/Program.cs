using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            double daysTriger = 13; // количество дней
            DateTime trigerDate = DateTime.Now.AddDays(daysTriger); // текущая дата, сегодняшний день
            List<string> tmp = new List<string>();
            Stopwatch benchmark = new Stopwatch();

            
            // Основной поток выполнения 
            Console.WriteLine($@"Searching for CTLs older than {trigerDate}");
            Console.ReadLine();
            ReadPath(); // получаем путь к папке ENV
            GetAllDirs(); // Получаем все директории в ENV
            GetFilesInDir(); // ищем и переносим файлы в целевой директории
            Console.WriteLine($@"В листе _{tmp.Count}_ объектов ");
            Console.ReadLine();
            RemoveFile(tmp); 
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
                    foreach (string dir in allDirs)
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
                        List<string> currentDir = Directory.EnumerateFiles($@"{dir}", textPattern).ToList();
                        if (currentDir.Count > 0)
                        {
                            foreach (string file in currentDir)
                            {
                                CompareFiles($@"{file}");
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
                //List<string> tmp = new List<string>(); // лист в который пишем подходящие файлы в папке
                DateTime fileDate = new FileInfo(tempFile).CreationTime; // получаем дату создания файла
                Console.WriteLine($@"{tempFile} {fileDate}");
                TimeSpan diff = DateTime.Now.Subtract(fileDate);
                if (diff.TotalDays > daysTriger)
                {
                    tmp.Add($@"{tempFile}");
                }
            }

            /// <summary>
            /// Метод переноса\удаления
            /// принимает файл в качестве аргумента, переносит в путь removePath
            /// </summary>
            void RemoveFile(List<string> files)
            {
                benchmark.Start();
                if (files.Count > 0)
                {
                    foreach (string fi in files)
                    {
                        string newPath = Path.Combine(removePath,Path.GetFileName(fi));
                        FileInfo fileinfo = new FileInfo(fi);
                        //fileinfo.MoveTo(Path.Combine(rootDir, fileYear, fileMonth, fileName));
                        fileinfo.MoveTo($@"{newPath}");
                        //File.Move($@"{fi}", $@"{newPath}");
                    }
                }
                benchmark.Stop();
                Console.WriteLine($@"{benchmark.ElapsedMilliseconds}");
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
