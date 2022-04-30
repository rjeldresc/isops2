﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DiscUtils.Iso9660;

namespace isops2
{
    internal class Program
    {
        static void Main(string[] args)
        {            
            if (args.Length == 1)
            {
                String pathIso = args[0];
                Console.WriteLine(pathIso);
                using (FileStream isoRom = File.Open(pathIso, FileMode.Open, FileAccess.Read))
                {
                    string romId = null;

                    byte[] magicNumberArray = new byte[11];
                    isoRom.Position = 65881;
                    isoRom.Read(magicNumberArray, 0, 11);
                    String magicNumberString = Encoding.ASCII.GetString(magicNumberArray);
                    isoRom.Position = 0;

                    if (magicNumberString != "PLAYSTATION")
                        Console.WriteLine("No es iso de ps2 :c");
                    else
                    {
                        var regex = new Regex(@"[A-Z]{4}_[0-9]+.[0-9]+"); // the regex matches a string of the form "<4 characters>_<some number>.<some number>"
                        CDReader reader = new(isoRom, true);
                        var id = reader.Root.GetFiles()
                            .Select(file => file.FullName) // Get the full name of all files in the root directory
                            .Where(file => regex.IsMatch(file)) // Check the filename against the regex
                            .First();
                        int posicion = id.IndexOf(";");
                        romId = id.Substring(0, posicion);                     
                        Console.WriteLine("RomId: {0}", romId);
                    }
                    isoRom.Close();
                    Console.WriteLine(Path.GetDirectoryName(pathIso) + "\\" + romId + "." + Path.GetFileName(pathIso));                    
                }
            }
            else
                Console.WriteLine("error: no se define archivo iso");
        }
    }
}