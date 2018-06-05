﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace CsvToXmlJson
{
    class Program
    {
        public static void Main(string[] args)
        {
            string CsvPath = ConfigurationManager.AppSettings["CsvPath"];
            try
            {
                List<Hotels> values = File.ReadAllLines(CsvPath)
                    .Select(v => Hotels.FromCsv(v))
                    .ToList();
                values.Sort();
                Converter con=new Converter();
                
                bool flag = true;
                while (flag)
                {
                    Console.WriteLine("If you want to convert to XML, press 1.");
                    Console.WriteLine("If you want to convert to JSON, press 2.");
                    Console.WriteLine("If you want to see LIST, press 3.");
                    Console.WriteLine("Press any other key to EXIT.");
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine(new string('=', 120));
                    Console.ResetColor();
                    string result = Console.ReadLine();
                    switch (result)
                    {
                        case "1":
                            con.CreateXml(values);
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("You can find XML in CsvToXmlJson/bin/Debug folder");
                            Console.ResetColor();
                            break;
                        case "2":
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("You can find JSON in CsvToXmlJson/bin/Debug folder");
                            Console.WriteLine("Pls enter PERENOS PO SLOVAM in your notepad :D");
                            Console.ResetColor();
                            con.CreateJSON(values);
                            break;
                        case "3":
                            foreach (var v in values)
                            {
                                Console.WriteLine(v.Id + "    " + v.Name + "  " + v.FoundedDate.Date + "   " + v.Raiting + "   " + v.Capacity);
                            }
                            Console.WriteLine(new string('-', 120));
                            break;
                        default:
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine("HAVE A NICE DAY MAN");
                            flag = false;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }
            Console.ReadKey();
        }
    }

    public class Converter
    {
        public void CreateXml(List<Hotels> values)
        {
            string XmlPath = ConfigurationManager.AppSettings["XmlPath"];

            var xmlWriter = new XmlTextWriter("hotels.xml", null);
            xmlWriter.WriteStartDocument();

            xmlWriter.Close();
            var hotelsXml = values.Select(i => new XElement("Hotel",
                new XElement("Id", i.Id),
                new XElement("Name", i.Name),
                new XElement("Date", i.FoundedDate.Date),
                new XElement("Capasity", i.Capacity),
                new XElement("Raiting", i.Raiting)));
            var bodyXml = new XElement("Hotels", hotelsXml);
            bodyXml.Save(XmlPath);
        }

        public  void CreateJSON(List<Hotels> values)
        {
            string JsonPath = ConfigurationManager.AppSettings["JsonPath"];

            string json = JsonConvert.SerializeObject(values.ToArray());
            File.WriteAllText(JsonPath, json);
        }
    }
    public class Hotels : IComparable<Hotels>
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public DateTime FoundedDate { get; set; }
        public int Capacity { get; set; }
        public double Raiting { get; set; }

        public int CompareTo(Hotels obj)
        {
            return obj.Raiting.CompareTo(Raiting);
        }

        public static Hotels FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            Hotels ho = new Hotels();
            ho.Name = values[0];
            ho.Id = Convert.ToInt32(values[1]);
            ho.FoundedDate = Convert.ToDateTime(values[2]);
            ho.Capacity = Convert.ToInt32(values[3]);
            ho.Raiting = values.Length == 4 ? Convert.ToDouble(0) : Convert.ToDouble(values[4]);
           
            return ho;
        }
    }
}
