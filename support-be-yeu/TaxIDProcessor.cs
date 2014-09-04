using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using WebserverInteractionClassLibrary;

namespace TaxIDProcessor
{
    public class TaxIDInfo
    {
        public string CMND { get; set; }
        public string MaSoThue { get; set; }
        public string HoTen { get; set; }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}", CMND, MaSoThue, HoTen);
        }
    }

    public class TaxIDProcessor
    {
        public List<TaxIDInfo> GetTaxIDs(IEnumerable<string> personalIDs)
        {
            return personalIDs.Select(personalID => GetTaxID(personalID)).ToList();
        }

        public TaxIDInfo GetTaxID(string personalID)
        {
            var ma = new RequestManager();
            var url = "http://www.gdt.gov.vn/wps/portal/!ut/p/b1/04_Sj9CPykssy0xPLMnMz0vMAfGjzOINTCw9fSzCgv2dzLxdDTxDHV2NLAM8jC3CzIAKIoEKnN0dPUzMfQwMLEzcDQw8TZz8_TycAw0NPI0J6Q_XjwIrwWcCxAwcwNFA388jPzdVvyA3wiDLxFERAIgooeI!/dl4/d5/L2dJQSEvUUt3QS80SmtFL1o2XzA0OUlMOFZTT1JVMjYwSVVKU0pKQTcyS1Q1/?WCM_GLOBAL_CONTEXT=/wps/wcm/connect/sa_home";
            var pa = string.Format("id=&page=1&action=action&mst=&fullname=&address=&cmt={0}", personalID);
            var req = ma.GeneratePOSTRequest(url, pa, string.Empty, string.Empty, false);
            var res = ma.GetResponse(req);
            var content = ma.GetResponseContent(res);

            var s = GetSubString(content, "<table class=\"ta_border\">", "</table>");

            return ParseTable(s);
        }

        private string GetSubString(string content, string begin, string end)
        {
            var i = content.IndexOf(begin);
            var temp = content.Substring(i);
            var j = temp.IndexOf(end);

            return temp.Substring(0, j + end.Length);
        }

        //private string ParseTable(string table)
        //{
        //    table = RemoveTagAttribute(table, "<table");
        //    table = RemoveTagAttribute(table, "<th");
        //    table = RemoveTagAttribute(table, "<tr");
        //    table = RemoveTagAttribute(table, "<td");
        //    table = RemoveTagAttribute(table, "<a");

        //    table = table.Replace("&nbsp;", "");
        //    var doc = XElement.Parse(table);
        //    var nodes = doc.Descendants("a");
        //    if (nodes.Count() < 5)
        //    {
        //        return string.Empty;
        //    }

        //    return string.Format("{0}\t{1}\t{2}", nodes.ElementAt(4).Value, nodes.ElementAt(1).Value,
        //                         nodes.ElementAt(2).Value);
        //}

        private TaxIDInfo ParseTable(string table)
        {
            table = RemoveTagAttribute(table, "<table");
            table = RemoveTagAttribute(table, "<th");
            table = RemoveTagAttribute(table, "<tr");
            table = RemoveTagAttribute(table, "<td");
            table = RemoveTagAttribute(table, "<a");

            table = table.Replace("&nbsp;", "");
            var doc = XElement.Parse(table);
            var nodes = doc.Descendants("a");
            if (nodes.Count() < 5)
            {
                return null;
            }

            return new TaxIDInfo()
            {
                CMND = nodes.ElementAt(4).Value,
                MaSoThue = nodes.ElementAt(1).Value,
                HoTen = nodes.ElementAt(2).Value
            };
        }

        private static string RemoveTagAttribute(string table, string tag)
        {
            var tl = tag.Length;
            var i = 0;
            while (i < table.Length)
            {
                var begin = table.IndexOf(tag, i);
                if (begin == -1)
                    break;

                begin = begin + tl;
                var end = table.IndexOf(">", begin);
                var l = end - begin;
                if (l > 0)
                    table = table.Replace(table.Substring(begin, l), "");

                i = begin + 1;
            }

            return table;
        }
    }
}
