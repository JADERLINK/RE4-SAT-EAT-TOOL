using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_SAT_EAT_Repack
{
    public static class IdxLoader
    {
        public static IdxEsat Loader(StreamReader idxFile) 
        {
            IdxEsat idx = new IdxEsat();

            Dictionary<string, string> pair = new Dictionary<string, string>();

            string line = "";
            while (line != null)
            {
                line = idxFile.ReadLine();
                if (line != null && line.Length != 0)
                {
                    var split = line.Trim().Split(new char[] { ':' });

                    if (line.TrimStart().StartsWith(":") || line.TrimStart().StartsWith("#") || line.TrimStart().StartsWith("/"))
                    {
                        continue;
                    }
                    else if (split.Length >= 2)
                    {
                        string key = split[0].ToUpper().Trim();

                        if (!pair.ContainsKey(key))
                        {
                            pair.Add(key, split[1].Trim());
                        }

                    }

                }
            }

            //-------

            //MAGIC
            try
            {
                string value = Utils.ReturnValidHexValue(pair["MAGIC"]);
                idx.Magic = byte.Parse(value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
            }

            //COUNT
            try
            {
                string value = Utils.ReturnValidDecValue(pair["COUNT"]);
                idx.Count = byte.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
            }

            //DUMMY
            try
            {
                string value = Utils.ReturnValidHexValue(pair["DUMMY"]);
                idx.Dummy = ushort.Parse(value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
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
