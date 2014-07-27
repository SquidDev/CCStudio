using CCStudio.Core.Computers;
using LuaInterface;
using System;
using System.Collections;
using System.IO;
using System.Net;

namespace CCStudio.Core.APIs
{
    public class HttpAPI : ILuaAPI
    {
        public static readonly String[] _Names = new String[] { "http" };
        public static readonly String[] _MethodNames = new String[] { "request" };

        public void request(string URL, string PostData = null, LuaTable Headers = null)
        {
            WebRequest Request = WebRequest.Create(URL);
            Request.Proxy = null;


            if (Headers != null)
            {
                WebHeaderCollection WebHeaders = new WebHeaderCollection();
                foreach (DictionaryEntry Item in Headers)
                {
                    WebHeaders.Add(Item.Key.ToString(), Item.Value.ToString());
                }
                Request.Headers = WebHeaders;
            }
            //WebResponse webResp = Request.GetResponse();
            if (!String.IsNullOrEmpty(PostData))
            {
                Request.Method = "POST";

                //Write post data
                Stream Content = Request.GetRequestStream();
                Content.Write(System.Text.Encoding.UTF8.GetBytes(PostData), 0, PostData.Length);
                Content.Close();
            }
            
            IAsyncResult AsyncResult = Request.BeginGetResponse(new AsyncCallback(FinishWebRequest), Request);
        }

        public void FinishWebRequest(IAsyncResult Result)
        {
            //webRequest.EndGetResponse(result);
            WebRequest Request = (WebRequest)Result.AsyncState;
            HttpWebResponse Response = (HttpWebResponse)Request.EndGetResponse(Result);

            if (Response.StatusCode == HttpStatusCode.OK)
            {
                Owner.PushEvent("http_success", Request.RequestUri.AbsoluteUri, new HttpResponse(Response));
            }
            else
            {
                Owner.PushEvent("http_failure", Request.RequestUri.AbsoluteUri);
            }

            
        }

        #region ILuaAPI Members
        public override string[] GetNames()
        {
            return _Names;
        }

        public override string[] GetMethodNames()
        {
            return _MethodNames;
        }
        #endregion
    }

    public class HttpResponse : ILuaObject
    {
        protected HttpWebResponse Response;
        protected StreamReader ResponseStream;

        public static readonly String[] _MethodNames = new String[] { "readLine", "readAll", "close", "getResponseCode" };

        public HttpResponse(HttpWebResponse Response)
        {
            this.Response = Response;
            ResponseStream = new StreamReader(Response.GetResponseStream());
        }

        public string readLine()
        {
            return ResponseStream.ReadLine();
        }
        public string readAll()
        {
            string Result = ResponseStream.ReadToEnd();
            ResponseStream.Close();
            Response.Close();
            return Result;
        }
        public void close()
        {
            ResponseStream.Close();
        }

        public int getResponseCode()
        {
            return (int)Response.StatusCode;
        }

        public override string[] GetMethodNames()
        {
            return _MethodNames;
        }

    }
}
