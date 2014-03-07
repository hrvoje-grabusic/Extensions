using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var str = "来自Kooboo Team的测试标题；;";
            var result = Kooboo.CMS.Content.UserKeyGenerator.Chinese.PinyinConverter.GetPinyin(str,"-");
            Console.WriteLine(result);

            Console.ReadKey();

        }
    }
}
