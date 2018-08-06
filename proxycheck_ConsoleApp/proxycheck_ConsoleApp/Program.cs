using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proxycheck_ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ProxyCheck _ProxyCheck = new ProxyCheck();
                string _Path = Directory.GetCurrentDirectory();
                string path_output = _Path + "//output-file//file.txt";
                string path_input = _Path + "//input-file//file.txt";

                Console.WriteLine("the default reading path is : {0} {1} please enter your file full path or press enter to start ", path_input, Environment.NewLine);
                
                var temp_path_input = Console.ReadLine();
                if (!string.IsNullOrEmpty(temp_path_input))
                {
                    path_input = temp_path_input;
                }
                Console.WriteLine("reading file : {0}" , path_input);

                _ProxyCheck.Check(path_input, path_output);

                Console.WriteLine("done ! {0} your out put file path is : {1} .{2} press any key to exit.", Environment.NewLine, Environment.NewLine ,path_output);
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                string exception ="error !"+  Environment.NewLine + ex.ToString() + "press any key to exit";
                Console.WriteLine(exception);
                Console.ResetColor();
            }
            Console.ReadLine();
        }
    }
}
