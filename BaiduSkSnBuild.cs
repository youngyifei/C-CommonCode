using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldUnion.CustomerManagement.Common
{
    public class BaiduSkSnBuild
    {
        private const string CONST_AK = "fBFIgFKRn0MQdZEVy4fVZ5MP05Cd1VNR";
        private const string CONST_SK = "UqUhqBj703nBnUK9ORpYcihD2H8vCMRH";
        private static string MD5(string password)
        {
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(password);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler;
                cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    ret += a.ToString("x2");
                }
                return ret;
            }
            catch
            {
                throw;
            }
        }

        private static string UrlEncode(string str)
        {
            str = System.Web.HttpUtility.UrlEncode(str);
            byte[] buf = Encoding.ASCII.GetBytes(str);//等同于Encoding.ASCII.GetBytes(str)
            for (int i = 0; i < buf.Length; i++)
                if (buf[i] == '%')
                {
                    if (buf[i + 1] >= 'a') buf[i + 1] -= 32;
                    if (buf[i + 2] >= 'a') buf[i + 2] -= 32;
                    i += 2;
                }
            return Encoding.ASCII.GetString(buf);//同上，等同于Encoding.ASCII.GetString(buf)
        }

        private static string HttpBuildQuery(IDictionary<string, string> querystring_arrays)
        {

            StringBuilder sb = new StringBuilder();
            foreach (var item in querystring_arrays)
            {
                sb.Append(UrlEncode(item.Key));
                sb.Append("=");
                sb.Append(UrlEncode(item.Value));
                sb.Append("&");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public static string CaculateAKSN(string ak, string sk, string url, IDictionary<string, string> querystring_arrays)
        {
            var queryString = HttpBuildQuery(querystring_arrays);

            var str = UrlEncode(url + "?" + queryString + sk);

            return MD5(str);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Latitude"></param>
        /// <param name="Longitude"></param>
        /// <returns></returns>
        public static string Geocoder(string latitude, string longitude)
        {
            string uri = "/geocoder/v2/";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("location", latitude+","+longitude);
            dic.Add("src", "WorldUnionCustomer");
            dic.Add("output", "json");
            dic.Add("coord_type", "bd09");
            dic.Add("ak", CONST_AK);
            string sn = CaculateAKSN(CONST_AK, CONST_SK, uri, dic);
            dic.Add("sn", sn);
            string queryString = HttpBuildQuery(dic);
            string str = "http://api.map.baidu.com"+uri + "?" + queryString;
            return str;
        }
    }
}
