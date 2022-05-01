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
                //Llega 1 parametro, el nombre de archivo iso
                case 1:
                    string pathIso = args[0];
                    string extension = Path.GetExtension(pathIso).ToLower();
                    if (extension == ".iso")
                    {
                        using FileStream isoRom = File.Open(pathIso, FileMode.Open, FileAccess.Read);
                        byte[] magicNumberArray = new byte[11];
                        isoRom.Position = 65881;
                        isoRom.Read(magicNumberArray, 0, 11);
                        string magicNumberString = Encoding.ASCII.GetString(magicNumberArray);
                        isoRom.Position = 0;
                        if (magicNumberString == "PLAYSTATION")
                        {
                            string romId = GetId(isoRom);
                            isoRom.Close();
                            if (!RomExiste(pathIso, romId))
                                RenombrarRom(pathIso, romId);
                            else
                                Console.WriteLine("Error: Ya existe una rom {0} con el mismo nombre .", romId);
                        }
                        else
                        {
                            Console.WriteLine("Error: No es .iso de PS2 :c");
                            Console.WriteLine("Tamaño: {0} MB , Nombre: {1}", new FileInfo(pathIso).Length / 1024 / 1024, Path.GetFileName(pathIso));
                            Console.WriteLine("Posiblemente sea un archivo tipo CD de PS2, igualmente forzar renombrar [S] / [N].");
                            char opcion = Convert.ToChar(Console.Read());
                            switch (opcion)
                            {
                                case 's':
                                case 'S':
                                    string romId = GetId(isoRom);
                                    isoRom.Close();
                                    if(!RomExiste(pathIso, romId))
                                        RenombrarRom(pathIso, romId);
                                    else
                                        Console.WriteLine("Error: Ya existe una rom {0} con el mismo nombre .", romId);
                                    break;
                                case 'n':
                                case 'N':
                                default:
                                    isoRom.Close();
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: No se define archivo .iso");
                    }
                    break;
                //No llegan parametros
                default:
                    Console.WriteLine("Error: No se ingresaron argumentos.");
                    break;
            }
        }

        /// <summary>
        /// Retorna el id de la iso
        /// </summary>
        /// <param name="isoRom">ruta completa de la rom</param>
        /// <returns>id de la rom , SLUS / SLES</returns>
        private static string GetId(FileStream isoRom)
        {
            string romId = null;
            var regex = new Regex(@"[A-Z]{4}_[0-9]+.[0-9]+");
            CDReader reader = new(isoRom, true);
            var id = reader.Root.GetFiles()
                .Select(file => file.FullName)
                .Where(file => regex.IsMatch(file))
                .First();
            int posicion = id.IndexOf(";");
            romId = id[..posicion];
            return romId;
        }

        /// <summary>
        /// Renombra la rom
        /// </summary>
        /// <param name="pathIso">ruta completa del archivo</param>
        /// <param name="romId">id de .iso SLUS / SLES</param>
        private static void RenombrarRom(string pathIso, string romId)
        {
            System.IO.File.Move(pathIso, Path.GetDirectoryName(pathIso) + "\\" + romId + "." + Path.GetFileName(pathIso));
            Console.WriteLine("RomId: {0}", romId);
            Console.WriteLine(".iso renombrada correctamente");
        }

        /// <summary>
        /// Valida si ya existe la rom .iso 
        /// </summary>
        /// <param name="pathIso">ruta completa del archivo</param>
        /// <param name="romId"></param>
        /// <returns>id de .iso SLUS / SLES</returns>
        private static bool RomExiste(string pathIso, string romId)
        {
            if (File.Exists(Path.GetDirectoryName(pathIso) + "\\" + romId + "." + Path.GetFileName(pathIso)))
                return true;
            else return false;
        }
    }
}
