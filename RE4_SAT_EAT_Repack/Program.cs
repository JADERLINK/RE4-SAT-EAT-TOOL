using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_SAT_EAT_REPACK
{
    class Program
    {
        public static string Version = "B.1.0.2 (2024-08-13)";

        public static string headerText()
        {
            return "# RE4_SAT_EAT_REPACK" + Environment.NewLine +
                   "# by: JADERLINK" + Environment.NewLine +
                   "# youtube.com/@JADERLINK" + Environment.NewLine +
                   "# Thanks to \"mariokart64n\" and \"zatarita\"" + Environment.NewLine +
                  $"# Version {Version}";
        }

        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine(headerText());

            if (args.Length == 0)
            {
                Console.WriteLine("For more information read:");
                Console.WriteLine("https://github.com/JADERLINK/RE4-SAT-EAT-TOOL");
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }
            else if (args.Length >= 1 && File.Exists(args[0]))
            {
                FileInfo fileInfo = new FileInfo(args[0]);
                Console.WriteLine(fileInfo.Name);

                if (fileInfo.Extension.ToUpperInvariant() == ".IDXSAT" ||
                    fileInfo.Extension.ToUpperInvariant() == ".IDXEAT")
                {
                    bool switchStatus = false;
                    bool enableDebugFiles = false;

                    if (args.Length >= 2 && args[1].ToUpper() == "TRUE")
                    {
                        switchStatus = true;
                    }

                    if (args.Length >= 3 && args[2].ToUpper() == "TRUE")
                    {
                        enableDebugFiles = true;
                    }

                    try
                    {
                        Action(fileInfo, switchStatus, enableDebugFiles);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                    }
                }
                else
                {
                    Console.WriteLine("The extension is not valid: " + fileInfo.Extension);
                }

            }
            else
            {
                Console.WriteLine("File specified does not exist.");
            }

            Console.WriteLine("Finished!!!");
        }

        private static void Action(FileInfo fileInfo, bool switchStatus, bool enableDebugFiles) 
        {
            string baseDirectory = Path.GetDirectoryName(fileInfo.FullName);
            string baseFileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
            string baseExtension = Path.GetExtension(fileInfo.FullName).ToLowerInvariant().Replace(".", "");
            string finalExtension = baseExtension.ToUpperInvariant().Replace("IDX", "");

            string baseFilePath = Path.Combine(baseDirectory, baseFileName);

            string pattern = "^(00)([0-9]{2})$";
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.CultureInvariant);

            if (regex.IsMatch(baseFileName))
            {
                baseFilePath = Path.Combine(baseDirectory, baseFileName + "_" + finalExtension);
            }

            var stream = new StreamReader(fileInfo.OpenRead(), Encoding.ASCII); // idxsat/idxeat
            var idx = IdxLoader.Loader(stream);
            stream.Close();

            if ( !(idx.Magic == 0x80 || idx.Magic == 0x20))
            {
                Console.WriteLine("The magic is incorrect, it should be 80 or 20");
                return;
            }

            if (idx.Count == 0)
            {
                Console.WriteLine("Count must be greater than 0");
                return;
            }

            bool ObjDoesNotExist = false;

            for (int i = 0; i < idx.Count; i++)
            {
                string obj = baseFilePath + "_" + i + ".obj";
                if (!File.Exists(obj))
                {
                    ObjDoesNotExist = true;
                }
            }

            if (ObjDoesNotExist)
            {
                Console.WriteLine("One of the required .Obj files does not exist");
                return;
            }

            // fim das validações
            //---------------

            Console.WriteLine("Start of the time count to recreate the collision file");
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            //-------------------

            (Structures.FinalStructure final, Group.FinalGroupStructure group)[] esatObj = new (Structures.FinalStructure final, Group.FinalGroupStructure group)[idx.Count];

            for (int i = 0; i < idx.Count; i++)
            {
                string obj = baseFilePath + "_" + i + ".obj";
                FileInfo objFileInfo = new FileInfo(obj);
                Console.WriteLine("Reading obj file: " + objFileInfo.Name);
                var streamObj = new StreamReader(objFileInfo.OpenRead(), Encoding.ASCII);
                
                var final = RepackOBJ.Repack(streamObj);
                var startGroups = Group.StartGroup.FirstStep(final);
                var finalGroups = Group.FinalGroupSteps.GetFinalGroupStructure(startGroups);

                esatObj[i] = (final, finalGroups);

                if (enableDebugFiles)
                {
                    FileInfo debugr = new FileInfo(baseFilePath + "_" + i + ".repack.obj");
                    Console.WriteLine("Creating the file: " + debugr.Name);
                    DebugR.EsatDebugOBJ(debugr, final);

                    FileInfo groupPlane = new FileInfo(baseFilePath + "_" + i + ".repack.GroupPlane.obj");
                    Console.WriteLine("Creating the file: " + groupPlane.Name);
                    DebugR.EsatGroupPlaneOBJ(groupPlane, startGroups);

                    FileInfo finalGroupPlane = new FileInfo(baseFilePath + "_" + i + ".repack.FinalGroupPlane.obj");
                    Console.WriteLine("Creating the file: " + finalGroupPlane.Name);
                    DebugR.EsatGroupPlaneOBJ(finalGroupPlane, finalGroups);
                }
            }

            
            FileInfo esatinfo = new FileInfo(Path.Combine(baseDirectory, baseFileName + "." + finalExtension));
            Console.WriteLine("Creating the file: " + esatinfo.Name);

            MakeFile.CreateFile(esatinfo, idx, esatObj, switchStatus);

            //------------------
            sw.Stop();
            Console.WriteLine("Taken time in Milliseconds: " + sw.ElapsedMilliseconds);
        }



    }


}
