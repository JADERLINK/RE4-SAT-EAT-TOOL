using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_SAT_EAT_REPACK
{
    public static class IdxLoader
    {
        public static IdxEsat Loader(StreamReader idxFile) 
        {
            IdxEsat idx = new IdxEsat();

            while (!idxFile.EndOfStream)
            {
                string line = idxFile.ReadLine();
                if (line != null && line.Length != 0)
                {
                    line = line.Trim().ToUpperInvariant();
                    if (line.StartsWith(":") || line.StartsWith("#") || line.StartsWith("/") || line.StartsWith("\\"))
                    {
                        continue;
                    }

                    var split = line.Split(new char[] { ':' });

                   if (split.Length >= 2)
                   {
                        string key = split[0].Trim();
                        string value = split[1].Trim();

                        if (key.Contains("MAGIC"))
                        {
                            try
                            {
                                value = Utils.ReturnValidHexValue(value);
                                idx.Magic = byte.Parse(value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else if (key.Contains("COUNT"))
                        {
                            try
                            {
                                value = Utils.ReturnValidDecValue(value);
                                idx.Count = byte.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else if (key.Contains("DUMMY"))
                        {
                            try
                            {
                                value = Utils.ReturnValidHexValue(value);
                                idx.Dummy = ushort.Parse(value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                            }
                            catch (Exception)
                            {
                            }

                        }
                    }

                }
            }

            return idx;
        }


    }


    public class IdxEsat 
    {
        public byte Magic;
        public byte Count;
        public ushort Dummy;
    }




}
