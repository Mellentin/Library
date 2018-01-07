using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Library
{
    class Program
    {
        //Основная структура таблицы данных
        public struct LibraryData
        {
            //Здесь описываются три поля: два простого типа и один массив структур

            public string Name; //Поле простого типа (строка)
            public string Owner; //Поле простого типа (строка)
            public Book[] Books; //Массив структур
        }

        public struct Book //Cтруктура книг, из которой потом будет создаваться массив книг (массив структур)
        {
            public string Writer;
            public string Title;
            public string PubHouse;
            public int Year; //Простой тип (числовой (integer))
            public string StoragePlace;
        }

        //Функция, которая имеет тип структуры LibraryData (это значит, что данные, которая она возратит будет с той структурой,
        //которую мы создали ранее. Эта функция считает с файла данные и подготовит ее в той структуре, которая нам нужна.
        static public LibraryData WriteFile(string path) //в скобочках указано значение, которое будет передаваться в эту функцию извне
        {
            LibraryData data = new LibraryData(); //Экземпляр нашей основной структуры

            data.Books = new Book[10]; //Указываем число книг (например 10). На самом деле это неправильно, ибо мы не знаем сколько у нас книг в файле. Для этого хорошо бы юзать List<> (список), там длину массива указывать не требуется, но со структурами это хз как выглядеть будет.

            int i = 0; //Объявляем счетчик итераций (численный тип). Он нам будет помогать считать какая итерация массива сейчас идет.

            StreamReader rdr = new StreamReader(path, Encoding.Default); //Открываем файл в режиме чтения (поток чтения) в нужной кодировке по пути, который передали

            string[] primaryInfo = rdr.ReadLine().Split('|'); // Для начала считываем название библиотеки и имя владельца библиотеки
            data.Name = primaryInfo[0]; //И заносим их в экземпляр структуры
            data.Owner = primaryInfo[1];

            while (!rdr.EndOfStream) //Объявляем цикл и пробежимся по всему файла до конца (грубо говоря: ПОКА (НЕ КОНЧИТСЯ ФАЙЛ))
            {
                //Данный код будет выполняться при чтении каждой строки в файле
                string tempStr = rdr.ReadLine(); //Читаем строку и заносим ее в специальную переменную, где она будет храниться во время данной итерации
                string[] tempArr = tempStr.Split('|'); //Разбиваем эту строку по сепаратору (разделителю) '|', делаем из этого массив подстрок

                //Теперь заносим полученную и уже разделенную строку в наш массив структур
                data.Books[i].Writer = tempArr[0].ToString();
                data.Books[i].Title = tempArr[1].ToString();
                data.Books[i].PubHouse = tempArr[2].ToString();
                data.Books[i].Year = Convert.ToInt32(tempArr[3]); //Т.к. в структуре год - числовой тип, а в файле - строка, то принудительно конвертируем строку в числовой тип
                data.Books[i].StoragePlace = tempArr[4].ToString();

                //В конце итерации увеличиваем счетчик на 1 (i++ это то же самое, что и i = i + 1;)
                i++;
            }

            //Цикл закончится только когда файл полностью прочитается. Как это произойдет код перейдет сюда
            //Последним этапом будет просто возвратить массив
            return data;
        }

        //Функция меню №1: добавление значение. В функцию мы получаем все данные, проводим действия с данными, и выводим готовые. Прямо как перерабатывающий завод.
        static public LibraryData AddBook(LibraryData data, string writer, string title, string pubhouse, int year, string storageplace)
        {
            Array.Resize(ref data.Books, data.Books.Length + 1); //Увеличиваем длину массива на 1, чтобы записать новые данные
            data.Books[data.Books.Length - 1].Writer = writer; //И забиваем новые данные 
            data.Books[data.Books.Length - 1].Title = title;
            data.Books[data.Books.Length - 1].PubHouse = pubhouse;
            data.Books[data.Books.Length - 1].Year = year;
            data.Books[data.Books.Length - 1].StoragePlace = storageplace;

            return data; //Потом просто возвращаем измененные данные
        }

        //Функция меню №2: удаление записи. Почти тоже самое, что и в №1. Поиск нужной записи будем делать по названию книги.
        static public LibraryData RemoveBook(LibraryData data, string title)
        {
                int index = Array.IndexOf(data.Books, title); //Ищем в массиве Books нужное значение

                if (index > -1) //Если такое значение есть, то в переменную index передастся номер (индекс) элемента массива от 0 до n. Если такого значения нет, то в index перейдет -1.
                {
                    data.Books[index].Writer = null; //Забиваем просто null во все щели
                    data.Books[index].Title = null;
                    data.Books[index].PubHouse = null;
                    data.Books[index].Year = 0; // а здесь 0, ибо в числовом типе нет null (он может быть, но программно никогда не задастся).
                    data.Books[index].StoragePlace = null;
                }

            return data; //Возвращаем измененные данные
        }

        //Функция меню №3: поиск по автору. Набор стандартный)
        static public LibraryData FindByWriter(LibraryData data, string writer)
        {
            int i = 0;

            foreach (var item in data.Books)
            {
                if (item.Writer != writer) //Если в данной итерации элемент массива не соответствует поиску, то 
                {
                    data.Books[i].Writer = null; // просто удаляем (очищаем) этот элемент
                    data.Books[i].Title = null;
                    data.Books[i].PubHouse = null;
                    data.Books[i].Year = 0;
                    data.Books[i].StoragePlace = null;
                }

                i++;
            }

            return data; //на выходе у нас останутся только отборные данные, либо никаких, если по заданным данным ничего не удалось найти)
        }

        //Функция меню №4: поиск по автору. Полная копия функции №3, только другие данные
        static public LibraryData FindTitle(LibraryData data, string title)
        {
            int i = 0;

            foreach (var item in data.Books)
            {
                if (item.Title != title) 
                {
                    data.Books[i].Writer = null; 
                    data.Books[i].Title = null;
                    data.Books[i].PubHouse = null;
                    data.Books[i].Year = 0;
                    data.Books[i].StoragePlace = null;
                }

                i++;
            }

            return data;
        }

        //Невозвращаемая функция (типа void). Она ничего не возвращает. Просто сделана для удобства, чтобы не громоздить программу большим кол-вом кода, ибо эта процедура будет выполняться много раз
        static public void ShowTable(LibraryData data)
        {
            Console.WriteLine("-------------------------------------------------------------------------------------------");
            Console.Write("| {0,15} |", data.Name);
            Console.WriteLine(" {0,63} |", data.Owner);
            Console.WriteLine("-------------------------------------------------------------------------------------------");
            foreach (var item in data.Books) //Пройдемся по массиву (грубо говоря: ДЛЯ КАЖДОГО)
            {
                if (item.Writer != null) //Если значение "Автор" данной итерации не пустое, то:
                {
                    //Выводим все на экран консоли + небольшое подобие таблицы
                    Console.Write("| {0,15} |", item.Writer);
                    Console.Write(" {0,15} |", item.Title);
                    Console.Write(" {0,15} |", item.PubHouse);
                    Console.Write(" {0,15} |", item.Year);
                    Console.WriteLine(" {0,15} |", item.StoragePlace);
                    Console.WriteLine("-------------------------------------------------------------------------------------------");
                }
            }

            Console.WriteLine(); //Пропускаем строку вниз
            Console.WriteLine("Операции с данными:");
            Console.WriteLine("1. Добавить запись");
            Console.WriteLine("2. Удалить запись");
            Console.WriteLine("3. Поиск книги по автору");
            Console.WriteLine("4. Поиск книги по названию");
            Console.Write("Нажмите соответствующую цифру на клавиатуре: ");
        }

        //Это основная функция консоли, где выполняется код запуска
        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory() + "/Data.txt"; //Задаем переменную и указываем путь к файлу (путь к исп. файлу + имя файла)

            if (File.Exists(path)) //Если данный файл существует по пути, то
            {
                LibraryData data = WriteFile(path); //Получаем все данные из файла по пути в массив структур

                ShowTable(data); //Показываем таблицу

                switch (Console.ReadKey(true).Key) //Ожидаем нажатие клавиши (true в скобочках не показывает нажатый символ на экране) и сразу начинаем обрабатывать нажатую клавишу
                {

                    case ConsoleKey.D1:

                        string writer = "", title = "", pubhouse = "", year = "", storageplace = "";

                        Console.WriteLine();
                        Console.Write("Введите имя автора: ");
                        writer = Console.ReadLine();
                        if (writer != "")
                        {
                            Console.Write("Введите название книги: ");
                            title = Console.ReadLine();
                            if (title != "")
                            {
                                Console.Write("Введите издательство: ");
                                pubhouse = Console.ReadLine();
                                if (pubhouse != "")
                                {
                                    Console.Write("Введите год издания: ");
                                    Regex pattern = new Regex(@"^\d+$"); //Оо, так называемые "регулярки". Лютая вещь, но здесь необходимая. Эта строка является шаблоном, который означается, грубо говоря, все цифры. То есть нам надо проверить строку есть ли в ней что то кроме цифр.
                                    year = Console.ReadLine();
                                    if (year != "" && pattern.IsMatch(year) == true) //если строка не пустая и является полностью числом
                                    {
                                        Console.WriteLine("Введите место хранения: ");
                                        storageplace = Console.ReadLine();
                                        if (storageplace != "")
                                        {
                                            data = AddBook(data, writer, title, pubhouse, Convert.ToInt32(year), storageplace);
                                        }
                                        else
                                            Console.WriteLine("Поле не может быть пустым!");
                                    }
                                    else
                                        Console.WriteLine("Поле не может быть пустым!");
                                }
                                else
                                    Console.WriteLine("Поле не может быть пустым!");
                            }
                            else
                                Console.WriteLine("Поле не может быть пустым!");
                        }
                        else
                            Console.WriteLine("Поле не может быть пустым!");
                        if (Array.IndexOf(data.Books, writer) > -1 && Array.IndexOf(data.Books, title) > -1 && Array.IndexOf(data.Books, pubhouse) > -1)
                        {
                            Console.WriteLine("Запись успешна добавлена. Сохранить данные в файл?");
                            ShowTable(data);
                        }
                        else
                        {
                            Console.WriteLine("Запись не была добавлена.");
                        }
                        break;
                }
            }
            else //Если файла нет, то
            {
                Console.WriteLine("Файл не обнаружен");
            }
        }
    }
}
