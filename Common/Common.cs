using System;
using System.Collections;
using System.IO;
using System.Reflection;
//using System.Threading.Tasks;

namespace FlashCard.Common
{
    public class Common
    {
        public static void CreateDbFileFirstTime()
        {
            if (Properties.Settings.Default.KanjiDbFilePath == "notset")
            {
                Properties.Settings.Default.KanjiDbFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.Combine("FlashCard", "kanjilist.db3"));
                Properties.Settings.Default.Save();
            }

            if (IsFileExist(Properties.Settings.Default.KanjiDbFilePath))
            {
                return;
            }

            //File.Copy()
        }


        public static Hashtable GetFontFamilyNameFromResource()
        {
            Hashtable fontFamilyDict = new Hashtable();

            var propertyList = typeof(Asset.Font).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            foreach(var prop in propertyList)
            {
                if (prop.Name.ToLower().Contains("_ttf"))
                {
                    string familyName;

                    switch (prop.Name)
                    {
                        case "Handwriting_ttf":
                            familyName = Asset.Font.Handwriting_ttf;
                            break;
                        case "KanjiStrokeOrders_ttf":
                            familyName = Asset.Font.KanjiStrokeOrders_ttf;
                            break;
                        case "Tahoma_ttf":
                            familyName = Asset.Font.Tahoma_ttf;
                            break;
                        case "TimeNewRoman_ttf":
                            familyName = Asset.Font.TimeNewRoman_ttf;
                            break;
                        case "教科書体_TTF":
                            familyName = Asset.Font.教科書体_TTF;
                            break;
                        case "楷書体_TTF":
                            familyName = Asset.Font.楷書体_TTF;
                            break;
                        default:
                            familyName = Asset.Font.教科書体_TTF;
                            break;
                    }

                    fontFamilyDict.Add(prop.Name, familyName);
                }
            }

            return fontFamilyDict;
        }

        public static bool IsFileExist(string filename)
        {
            if(!File.Exists(filename))
            {
                return false;
            }

            return true;
        }
    }
}
