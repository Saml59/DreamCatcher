using System;
using RestSharp;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using RestSharp.Serializers.SystemTextJson;

namespace DC.Models
{
    //This class exists to simplify the process of making api calls and reduce code clutter in other files
    public class APICall
    {
        public RestClient client { get; set; }
        public IRestRequest request { get; set; }
        public string json_body { get; set; }
        public string method { get; set; }

        //Constructor with a body
        public APICall(string url_extension, string method, Object body)
        {
            this.client = new RestClient(Constant.BaseURL);
            this.client.UseSystemTextJson();
            this.json_body = JsonSerializer.Serialize(body);
            this.request = new RestRequest(url_extension).AddJsonBody(this.json_body);
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            this.method = method;

        }

        //Constructor without a body
        public APICall(string url_extension, string method)
        {
            this.client = new RestClient(Constant.BaseURL);
            this.client.UseSystemTextJson();
            this.request = new RestRequest(url_extension);
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            this.method = method;
        }

        public void addHeader(string key, string value)
        {
            this.request.AddHeader(key, value);
        }


        public async Task<Generic_Response<T>> make_call<T>()
        {
            if (this.method == "GET")
            {
                var response = await this.client.GetAsync<Generic_Response<T>>(this.request);
                return response;
            }
            if (this.method == "POST")
            {
                var response = await this.client.PostAsync<Generic_Response<T>>(this.request);
                return response;
            }
            if (this.method == "DELETE")
            {
                var response = await this.client.DeleteAsync<Generic_Response<T>>(this.request);
                return response;
            }
            if (this.method == "PUT")
            {
                var response = await this.client.PutAsync<Generic_Response<T>>(this.request);
                return response;
            }
            else return null;
        }

    }
}
