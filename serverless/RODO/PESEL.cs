using System.Net;
using System;
using System.Security.Cryptography;
using System.Text;


public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    // parse query parameter
    string pesel = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "pesel", true) == 0)
        .Value;

    if (pesel == null)
    {
        // Get request body
        dynamic data = await req.Content.ReadAsAsync<object>();
        pesel = data?.pesel;
    }

    if (pesel != null) {
        string y;
        string m;
        string d;
        string r;
        using (MD5 md5Hash = MD5.Create())
        {
            byte[] hash = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(pesel));
            byte[] data = new byte[4];
            Array.Copy(hash , data , data.Length);
            Array.Reverse(data);
            string hex = BitConverter.ToString(data);

            y = "0" + (Math.Abs(BitConverter.ToInt32(data, 0)) %20 +1);
            y = y.Substring(y.Length - 2);
            m = "0" + (Math.Abs(BitConverter.ToInt32(data, 0)) %12 +1);
            m = m.Substring(m.Length - 2);
            d = "0" + (Math.Abs(BitConverter.ToInt32(data, 0)) %27 +1);
            d = d.Substring(d.Length - 2);
            r = "00000" + Math.Abs(BitConverter.ToInt32(data, 0)).ToString().Substring(0, 5);
            r = r.Substring(r.Length - 5);
            string result =  y+m+d+ r;

            int control = 
                (Int32.Parse(result.Substring(0,1))*9 +
                Int32.Parse(result.Substring(1,1))*7 +
                Int32.Parse(result.Substring(2,1))*3 +
                Int32.Parse(result.Substring(3,1))*1 +
                Int32.Parse(result.Substring(4,1))*9 +
                Int32.Parse(result.Substring(5,1))*7 +
                Int32.Parse(result.Substring(6,1))*3 +
                Int32.Parse(result.Substring(7,1))*1 +
                Int32.Parse(result.Substring(8,1))*9 +
                Int32.Parse(result.Substring(9,1))*7 ) % 10;



return req.CreateResponse(HttpStatusCode.BadRequest, 
//hex.Replace("-","") + BitConverter.ToInt32(data, 0) + "pes:"+
result.Substring(0, 10) + control.ToString());
        }
        
    } else {
        return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a pesel on the query string or in the request body");
    }