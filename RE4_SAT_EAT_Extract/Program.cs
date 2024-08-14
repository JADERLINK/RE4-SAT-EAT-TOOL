using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_SAT_EAT_EXTRACT
{
    class Program
    {
        public static string Version = "B.1.0.2 (2024-08-13)";

        public static string headerText()
        {
            return "# RE4_SAT_EAT_EXTRACT" + Environment.NewLine +
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

                if (fileInfo.Extension.ToUpperInvariant() == ".SAT" ||
                    fileInfo.Extension.ToUpperInvariant() == ".EAT")
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
                        string baseDirectory = Path.GetDirectoryName(fileInfo.FullName);
                        string baseFileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                        string baseExtension = Path.GetExtension(fileInfo.FullName).ToLowerInvariant().Replace(".", "");

                        string baseFilePath = Path.Combine(baseDirectory, baseFileName);

                        string pattern = "^(00)([0-9]{2})$";
                        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.CultureInvariant);

                        if (regex.IsMatch(baseFileName))
                        {
                            baseFilePath = Path.Combine(baseDirectory, baseFileName + "_" + baseExtension.ToUpperInvariant());
                        }

                        var stream = fileInfo.OpenRead();
                        var esatHeader = Extractor.Extract(stream);
                        stream.Close();

                        FileInfo idx = new FileInfo(Path.Combine(baseDirectory, baseFileName + ".idx" + baseExtension));
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
                    Console.WriteLine("The extension is not valid: " + fileInfo.Extension);
                }

            }
            else
            {
                Console.WriteLine("File specified does not exist.");
            }

            Console.WriteLine("Finished!!!");
        }
    }
}
