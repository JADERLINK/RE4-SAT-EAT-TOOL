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
        public static string Version = "V.1.2.0 (2024-11-25)";

        public static string HeaderText()
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

            Console.WriteLine(HeaderText());

            if (args.Length == 0)
            {
                Console.WriteLine("For more information read:");
                Console.WriteLine("https://github.com/JADERLINK/RE4-SAT-EAT-TOOL");
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }
            else if (args.Length >= 2)
            {
                if (File.Exists(args[0]))
                {
                    IsVersion isVersion = IsVersion.Null;
                    SwitchStatus switchStatus = SwitchStatus.Null;

                    string arg = args[1].ToUpper();
                    if (arg.Contains("UHD"))
                    {
                        switchStatus = SwitchStatus.TrueUHD;
                        isVersion = IsVersion.IsUHD;
                    }
                    else if (arg.Contains("2007PS2") || arg.Contains("PS2") || arg.Contains("2007"))
                    {
                        switchStatus = SwitchStatus.FalsePs2;
                        isVersion = IsVersion.IsPS2;
                    }
                    else if (arg.Contains("PS4NS") || arg.Contains("PS4") || arg.Contains("NS"))
                    {
                        switchStatus = SwitchStatus.TrueUHD;
                        isVersion = IsVersion.IsPS4NS;
                    }
                    else if (arg.Contains("BIG"))
                    {
                        switchStatus = SwitchStatus.BigEndian;
                        isVersion = IsVersion.IsBigEndian;
                    }
                    else if (arg.Contains("RE4VR"))
                    {
                        switchStatus = SwitchStatus.TrueUHD;
                        isVersion = IsVersion.IsRE4VR;
                    }

                    if (isVersion != IsVersion.Null && switchStatus != SwitchStatus.Null)
                    {
                        bool enableDebugFiles = false;
                        if (args.Length >= 3 && args[2].ToUpper().Contains("TRUE"))
                        {
                            enableDebugFiles = true;
                        }

                        FileInfo fileInfo = null;

                        try
                        {
                            fileInfo = new FileInfo(args[0]);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error in the path: " + Environment.NewLine + ex);
                        }

                        if (fileInfo != null) 
                        {
                            Console.WriteLine(fileInfo.Name);

                            if (fileInfo.Extension.ToUpperInvariant() == ".SAT" ||
                                fileInfo.Extension.ToUpperInvariant() == ".EAT" ||
                                fileInfo.Extension.ToUpperInvariant() == ".SCE")
                            {
                                try
                                {
                                    Action(fileInfo, switchStatus, isVersion, enableDebugFiles);
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
                    }
                    else
                    {
                        Console.WriteLine("The second argument is invalid.");
                    }
                }
                else
                {
                    Console.WriteLine("File specified does not exist.");
                }
            }
            else
            {
                Console.WriteLine("The second argument is required.");
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }

            Console.WriteLine("Finished!!!");
        }

        private static void Action(FileInfo fileInfo, SwitchStatus switchStatus, IsVersion isVersion, bool enableDebugFiles) 
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

            FileStream stream = fileInfo.OpenRead();
            ESatHeader esatHeader = null;

            try
            {
                esatHeader = Extractor.Extract(stream, isVersion);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                stream.Close();
            }

            FileInfo idx = new FileInfo(Path.Combine(baseDirectory, baseFileName + ".idx" + baseExtension));
            Console.WriteLine("Creating the file: " + idx.Name);
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

    }
}
