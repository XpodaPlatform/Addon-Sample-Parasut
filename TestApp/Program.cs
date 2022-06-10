using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.LoadFrom(@"C:\Users\Devran\source\repos\XpodaParasutAddon\XpodaParasutAddon\bin\Debug\XpodaParasutAddon.dll");
            var type = assembly.GetType("XpodaParasutAddon.ParasutClass");
            var method = type.GetMethod("GetToken");
            var paramss = new object[1];
            var paramlist = new List<Dictionary<string, object>>();

            paramlist.Add(new Dictionary<string, object>
            {
                {"username", "devran@digitalbueno.com" },
                {"password", "gug67kml!" }
            });

            paramss[0] = paramlist;

            var obj = Activator.CreateInstance(type);

            var rs = method.Invoke(obj, paramss) as Dictionary<string, object>;

            if (!string.IsNullOrEmpty(rs["Error"].ToString()))
                Console.WriteLine(rs["Error"].ToString());
            else
            {
                List<Dictionary<string, object>> x = (List<Dictionary<string, object>>)rs["List"];

                Console.WriteLine(x[0]["Result"].ToString());

                var token = x[0]["Result"].ToString();

                var method2 = type.GetMethod("GetCustomerList");

                paramss = new object[1];
                paramlist = new List<Dictionary<string, object>>();
                paramlist.Add(new Dictionary<string, object>
                {
                    {"Token", token }
                });

                paramss[0] = paramlist;

                var rs2 = method2.Invoke(obj, paramss) as Dictionary<string, object>;

                if (!string.IsNullOrEmpty(rs2["Error"].ToString()))
                    Console.WriteLine(rs2["Error"].ToString());
                else
                {
                    List<Dictionary<string, object>> x2 = (List<Dictionary<string, object>>)rs2["List"];
                    foreach (var cust in x2)
                    {
                        foreach (var key in cust.Keys)
                        {
                            Console.WriteLine(cust[key].ToString());
                        }
                    }
                }
            }



            Console.ReadKey();
        }
    }
}
