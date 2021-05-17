using JSONtoXML.Universal;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace JSONtoXML.Parser
{
    public class Parser : IParser
    {
        private ExpandoObject Buffer;
        private List<char> list = new List<char>();
        private List<char> docWithoutSpaces = new List<char>();

        private List<string> keys;
        string key;

        //private Dictionary<>

        enum LevelTypes { _object, massive }

        enum paramType { key, value }

        paramType current;

        private List<LevelTypes> levels;

        Dictionary<char, int> repeats;


        public void loadJSON(string path)
        {
            using (FileStream fs = File.Open(path, FileMode.Open))
            {
                byte[] array = new byte[fs.Length];

                fs.Read(array, 0, array.Length);

                ParseString(new StringBuilder(System.Text.Encoding.Default.GetString(array)));
            }


            //Buffer.TryAdd<string, object>();
        }
        public ExpandoObject GetUniversal()
        {


            return Buffer;
        }

        void ParseString(StringBuilder str)
        {
            for (int j = 0; j < str.Length; ++j)
            {
                if (str[j] == '{')
                {
                    str.Remove(0, 1);
                    str.Insert(0, "<root>");
                    list.Insert(0, '{');
                    levels.Insert(0, LevelTypes._object);
                    FigureOpen(new StringBuilder(str.ToString().Substring(1)));
                    break;
                }
                else if(str[j] == '[')
                {
                    str.Remove(0, 1);
                    str.Insert(0, "<root>");
                    list.Insert(0, '[');
                    levels.Insert(0, LevelTypes.massive);
                    QuadOpen(new StringBuilder(str.ToString().Substring(1)));
                    break;
                }
            }
        }

        void FigureOpen(StringBuilder str)
        {
            int i = 0;

            while (i < str.Length)
            {
                switch (str[i])
                {
                    case '"':
                        str[i] = '<';
                        list.Insert(0, '"');
                        DoubleQuatesOpen(new StringBuilder(str.ToString().Substring(i + 1)), paramType.key);
                        break;
                    case '}':
                        list.Insert(0, '}');
                        levels.RemoveAt(0);
                        FigureClose(new StringBuilder(str.ToString().Substring(i + 1)));
                        break;
                    default:
                        if (!char.IsWhiteSpace(str[i]))
                        {
                            Console.WriteLine("Ожидалось закрытие объекта или начало ключа. Получили: " + str[i]);
                        }

                        //levels.Insert(i, LevelTypes._object);
                        break;
                }
                ++i;
            }
        }

        void FigureClose(StringBuilder str)
        {
            int i = 0;

            while (i < str.Length)
            {
                switch (str[i])
                {
                    case ',':
                        list.Insert(0, ',');
                        comma(new StringBuilder(str.ToString().Substring(i + 1)));
                        break;
                    case ']':
                        list.Insert(0, ']');
                        if (levels[0] != LevelTypes.massive)
                        {
                            Console.WriteLine("Попытка закрыть объект квадратной скобкой");
                        }
                        QuadClose(new StringBuilder(str.ToString().Substring(i + 1)));
                        break;
                    case '}':
                        list.Insert(0, '}');
                        if (levels[0] != LevelTypes.massive)
                        {
                            Console.WriteLine("Попытка закрыть объект квадратной скобкой");
                        }
                        FigureClose(new StringBuilder(str.ToString().Substring(i + 1)));
                        break;
                    default:
                        if (!char.IsWhiteSpace(str[i]))
                        {
                            Console.WriteLine("Недопустимый символ после закрывающей фигурной скобки");
                        }
                        break;
                }
                ++i;
            }
        }

        void DoubleQuatesOpen(StringBuilder str, paramType ParamType)
        {
            int i = 0;

            while (i < str.Length)
            {
                switch (str[i])
                {
                    case '"':
                        if (ParamType == paramType.key)
                        {
                            keys.Insert(0, key);
                            str[i] = '>';
                        }
                        list.Insert(0, '"');
                        DoubleQuatesClose(new StringBuilder(str.ToString().Substring(i + 1)), ParamType);
                        break;
                    default:
                        if (ParamType == paramType.key)
                        {
                            key += str[i];
                        }
                        break;
                }
                ++i;
            }

            Console.WriteLine("Ошибка, кавычки не были закрыты");
        }

        void DoubleQuatesClose(StringBuilder str, paramType ParamType)
        {
            int i = 0;

            while (i < str.Length)
            {
                switch (str[i])
                {
                    case ':':
                        if (levels[0] == LevelTypes.massive)
                        {
                            Console.WriteLine("Ошибка, ожидалась запятая или закрывающая квадратная скобка");
                            break;
                        }
                        else if (ParamType == paramType.value)
                        {
                            Console.WriteLine("Ошибка. Ожидалась запятая или закрывающая фигурная скобка");
                            break;
                        }

                        str.Remove(i, 1);
                        list.Insert(0, ':');
                        DoublePoints(new StringBuilder(str.ToString().Substring(i + 1)));
                        break;
                    case ',':
                        if (ParamType == paramType.key)
                        {
                            Console.WriteLine("Ошибка. Ожидалось ':'. Получили: ','");
                            break;
                        }
                        else
                        {
                            str.Insert(i+1, keys[0]);
                            str.Insert(i + 2, '/');
                        }

                        str.Remove(i, 1);
                        list.Insert(0, ',');
                        comma(new StringBuilder(str.ToString().Substring(i+1)));
                        break;
                    case ']':
                        if (levels[0] == LevelTypes._object || ParamType != paramType.value)
                        {
                            Console.WriteLine("Ошибка. Вызвано закрытие массива в блоке объекта");
                        }
                        list.Insert(0, ']');
                        levels.RemoveAt(0);
                        QuadClose(new StringBuilder(str.ToString().Substring(i + 1)));
                        break;
                    case '}':
                        if (levels[0] == LevelTypes.massive || ParamType != paramType.value)
                        {
                            Console.WriteLine("Ошибка. Вызвано закрытие объекта в блоке массива");
                        }
                        list.Insert(0, '}');
                        levels.RemoveAt(0);
                        FigureClose(new StringBuilder(str.ToString().Substring(i + 1)));
                        break;
                    default:
                        if (!char.IsWhiteSpace(str[i]))
                        {
                            Console.WriteLine("Недопустимый символ после закрытия двойных ковычек: " + str[i]);
                        }
                        break;
                }
                ++i;
            }
        }

        void DoublePoints(StringBuilder str)
        {
            int i = 0;

            while (i < str.Length)
            {
                switch (str[i])
                {
                    case '"':
                        str.Remove(i, 1);
                        list.Insert(0, '"');
                        DoubleQuatesOpen(new StringBuilder(str.ToString().Substring(i + 1)), paramType.value);
                        break;
                    case '{':
                        str.Remove(i,1);
                        list.Insert(0, '{');
                        levels.Insert(0, LevelTypes._object);
                        FigureOpen(new StringBuilder(str.ToString().Substring(i + 1)));
                        break;
                    case '[':
                        str.Remove(i, 1);
                        list.Insert(0, '[');
                        levels.Insert(0, LevelTypes._object);
                        QuadOpen(new StringBuilder(str.ToString().Substring(i + 1)));
                        break;
                    case ',':
                        str.Remove(i, 1);
                        list.Insert(0, ',');
                        str.Insert(i, "</" + keys[0] + '>');
                        comma(new StringBuilder(str.ToString().Substring(i + 3 + keys[0].Length)));
                        break;
                    default:
                        if (char.IsDigit(str[i]) || str[i] == '.')
                        {

                        }
                        else if (!char.IsWhiteSpace(str[i]))
                        {
                            Console.WriteLine("Ожидалось число, открытие двойных кавычек или скобок. Получили символ: " + str[i]);
                        }
                        break;
                }
                ++i;
            }
        }

        void comma(StringBuilder str)
        {
            int i = 0;

            while (i < str.Length)
            {
                switch (str[i])
                {
                    case '[':
                        if (levels[0] != LevelTypes.massive)
                        {
                            Console.WriteLine("Попытка задать массив без ключа");
                            break;
                        }
                        str.Remove(i, 1);
                        list.Insert(0, ',');
                        str.Insert(i, "</" + keys[0] + '>');
                        levels.Insert(0, LevelTypes.massive);
                        QuadOpen(new StringBuilder(str.ToString().Substring(i + 2 + keys[0].Length)), true);
                        break;
                    case '{':
                        if (levels[0] != LevelTypes.massive)
                        {
                            Console.WriteLine("Попытка задать объект без ключа");
                        }
                        list.Insert(0, ',');
                        levels.Insert(0, LevelTypes._object);
                        FigureOpen(new StringBuilder(str.ToString().Substring(i + 1)));
                        break;
                    case '"':
                        if ((levels[0] == LevelTypes.massive) && (list[0] != '"'))
                        {
                            Console.WriteLine("Не совпадает тип перечисления в массиве, встречен символ " + str[i]);
                        }

                        DoubleQuatesOpen(new StringBuilder(str.ToString().Substring(i + 1)), paramType.key);
                        break;
                    case ',':
                        list.Insert(0, ',');
                        str.Remove(i, 1);
                        comma(new StringBuilder(str.ToString().Substring(i + 1)));
                        break;
                    default:
                        if (char.IsDigit(str[i]) || (str[i] == '.'))
                        {

                        }
                        else if (char.IsWhiteSpace(str[i]))
                        {
                            Console.WriteLine("недопустимый символ после запятой: " + str[i]);
                        }
                        break;
                }
                ++i;
            }
        }

        void QuadOpen(StringBuilder str, bool isNeedTag = false)
        {
            int i = 0;

            levels.Insert(0, LevelTypes.massive);

            while (i < str.Length)
            {
                switch (str[i])
                {
                    case '"':
                        DoubleQuatesOpen(new StringBuilder(str.ToString().Substring(i + 1)), paramType.value);
                        break;
                    case '{':
                        FigureOpen(new StringBuilder(str.ToString().Substring(i + 1)));
                        break;
                    case '[':
                        QuadOpen(new StringBuilder(str.ToString().Substring(i + 1)));
                        break;
                    case ']':
                        levels.RemoveAt(0);
                        QuadClose(new StringBuilder(str.ToString().Substring(i + 1)));
                        break;
                    default:
                        if (char.IsDigit(str[i]))
                        {

                        }
                        else
                        {
                            Console.WriteLine("недопустимый символ после открывающей квадратной скобки: " + str[i]);
                        }
                        break;
                }
                ++i;
            }
        }

        void QuadClose(StringBuilder str)
        {
            int i = 0;
            levels.RemoveAt(0);
            while (i < str.Length)
            {
                switch (str[i])
                {
                    case ',':
                        comma(new StringBuilder(str.ToString().Substring(i + 1)));
                        break;
                    case '}':
                        if (levels[0] != LevelTypes._object)
                        {
                            Console.WriteLine("Попытка закрыть массив закрывающей фигурной скобкой");
                        }
                        levels.RemoveAt(0);
                        FigureClose(new StringBuilder(str.ToString().Substring(i + 1)));
                        break;
                    case ']':
                        if (levels[0] != LevelTypes._object)
                        {
                            Console.WriteLine("Попытка закрыть массив закрывающей фигурной скобкой");
                        }
                        levels.RemoveAt(0);
                        QuadClose(new StringBuilder(str.ToString().Substring(i + 1)));
                        break;
                    default:

                        if (!char.IsWhiteSpace(str[i]))
                        {
                            Console.WriteLine("Указан неверный символ после закрывающей квадратной скобки");
                        }

                        break;
                }
                ++i;
            }
        }
    }
}