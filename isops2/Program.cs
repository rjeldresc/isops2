using DiscUtils.Iso9660;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace isops2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            switch(args.Length)
            {                
                case 1:
                    string pathIso = args[0];
                    string extension = Path.GetExtension(pathIso).ToLower();
                    if (extension == ".iso")
                    {
                        using (FileStream isoRom = File.Open(pathIso, FileMode.Open, FileAccess.Read))
                        {
                            string romId = null;
                            byte[] magicNumberArray = new byte[11];
                            isoRom.Position = 65881;
                            isoRom.Read(magicNumberArray, 0, 11);
                            string magicNumberString = Encoding.ASCII.GetString(magicNumberArray);
                            isoRom.Position = 0;
                            if (magicNumberString == "PLAYSTATION")
                            {
                                var regex = new Regex(@"[A-Z]{4}_[0-9]+.[0-9]+");
                                CDReader reader = new(isoRom, true);
                                var id = reader.Root.GetFiles()
                                    .Select(file => file.FullName)
                                    .Where(file => regex.IsMatch(file))
                                    .First();
                                int posicion = id.IndexOf(";");
                                romId = id.Substring(0, posicion);
                                isoRom.Close();
                                System.IO.File.Move(pathIso, Path.GetDirectoryName(pathIso) + "\\" + romId + "." + Path.GetFileName(pathIso));
                                Console.WriteLine("RomId: {0}", romId);
                                Console.WriteLine(".iso renombrada correctamente");
                            }
                            else
                            {
                                Console.WriteLine("Error: No es .iso de Ps2 :c");
                                isoRom.Close();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: No se define archivo .iso");
                    }
                    break;
                default:
                    Console.WriteLine("Error: No se ingresaron argumentos");
                    break;
            }
        }
    }
}
