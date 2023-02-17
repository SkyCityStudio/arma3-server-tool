using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace a3.Tools
{
    public class JsonConversionTool
    {
        public static string ObjectToJson(object obj)
        {
            JsonSerializerSettings jss = new JsonSerializerSettings();

            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";//日期转化为字符串类型
            jss.Converters.Add(timeConverter);

            return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, jss);
        }

        public static object JsonToObject(string jsonString, object obj)
        {
            return JsonConvert.DeserializeObject(jsonString, obj.GetType());
        }
    }
}
