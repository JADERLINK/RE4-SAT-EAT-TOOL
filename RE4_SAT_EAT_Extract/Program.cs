using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_SAT_EAT_Extract
{
    class Program
    {
        public static string Version = "B.1.0.0.0 (2023-12-27)";

        public static string headerText()
        {
            return "# RE4_SAT_EAT_Extract" + Environment.NewLine +
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

                if (file.Extension.ToUpperInvariant() == ".SAT" ||
                    file.Extension.ToUpperInvariant() == ".EAT")
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
                        string baseFilePath = file.FullName.Substring(0, file.FullName.Length - file.Extension.Length);

                        var stream = file.OpenRead();
                        var esatHeader = Extractor.Extract(stream);
                        stream.Close();

                        FileInfo idx = new FileInfo(baseFilePath + ".idx" + file.Extension.ToLowerInvariant().Replace(".", ""));
                        Console.WriteLine("Creating the file: "+ idx.Name);
                        OutputFiles.IDX(idx, esatHeader);

                        for (int i = 0; i < esatHeader.Esat.Length; i++)
                        {
                            FileInfo obj = new FileInfo(baseFilePath + "_" + i + ".obj");
                            Console.WriteLine("Creating the file: " + obj.Name);
                            OutputFiles.EsatOBJ(obj, esatHeader.Esat[i], switchStatus);

                            if (enableDebugFiles)
                            {
                                FileInfo debugObj = new FileInfo(baseFilePath + "_" + i + ".Debug.obj");
                                Console.WriteLine("Creating the file: " + debugObj.Name);
                                Debug.EsatDebugOBJ(debugObj, esatHeader.Esat[i], switchStatus);

                                FileInfo linesObj = new FileInfo(baseFilePath + "_" + i + ".Lines.obj");
                                Console.WriteLine("Creating the file: " + linesObj.Name);
                                Debug.EsatLinesOBJ(linesObj, esatHeader.Esat[i], switchStatus);

                                FileInfo groupBox = new FileInfo(baseFilePath + "_" + i + ".GroupBox.obj");
                                FileInfo groupArrow = new FileInfo(baseFilePath + "_" + i + ".GroupArrow.obj");
                                FileInfo groupPlane = new FileInfo(baseFilePath + "_" + i + ".GroupPlane.obj");

                                Console.WriteLine("Creating the file: " + groupBox.Name);
                                Debug.EsatGroupBoxOBJ(groupBox, esatHeader.Esat[i]);
                                Console.WriteLine("Creating the file: " + groupArrow.Name);
                                Debug.EsatGroupArrowOBJ(groupArrow, esatHeader.Esat[i]);
                                Console.WriteLine("Creating the file: " + groupPlane.Name);
                                Debug.EsatGroupPlaneOBJ(groupPlane, esatHeader.Esat[i]);
                            }
                        }
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
    }
}
