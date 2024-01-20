using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_SAT_EAT_Repack
{
    class Program
    {
        public static string Version = "B.1.0.0.1 (2024-01-20)";

        public static string headerText()
        {
            return "# RE4_SAT_EAT_Repack" + Environment.NewLine +
                   "# by: JADERLINK" + Environment.NewLine +
                   "# Thanks to \"mariokart64n\" and \"zatarita\"" + Environment.NewLine +
                  $"# Version {Version}";
        }

        static void Main(string[] args)
        {
            Console.WriteLine(headerText());

            if (args.Length == 0)
            {
                Console.WriteLine("For more information read:");
                Console.WriteLine("https://github.com/JADERLINK/RE4-SAT-EAT-TOOL");
            }
            else if (args.Length >= 1 && File.Exists(args[0]))
            {
                FileInfo file = new FileInfo(args[0]);
                Console.WriteLine(file.Name);

                if (file.Extension.ToUpperInvariant() == ".IDXSAT" ||
                    file.Extension.ToUpperInvariant() == ".IDXEAT")
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
                        Action(file, switchStatus, enableDebugFiles);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                    }
                }
                else
                {
                    Console.WriteLine("Wrong file");
                }

            }
            else
            {
                Console.WriteLine("The file does not exist");
            }

            Console.WriteLine("End");
        }

        private static void Action(FileInfo file, bool switchStatus, bool enableDebugFiles) 
        {
            string baseFilePath = file.FullName.Substring(0, file.FullName.Length - file.Extension.Length);

            var stream = new StreamReader(file.OpenRead(), Encoding.ASCII); // idxsat/idxeat
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

            string finalExtension = file.Extension.ToUpperInvariant().Replace("IDX", "");
            FileInfo esatinfo = new FileInfo(baseFilePath + finalExtension);
            Console.WriteLine("Creating the file: " + esatinfo.Name);

            MakeFile.CreateFile(esatinfo, idx, esatObj, switchStatus);

            //------------------
            sw.Stop();
            Console.WriteLine("Taken time in Milliseconds: " + sw.ElapsedMilliseconds);
        }



    }


}
