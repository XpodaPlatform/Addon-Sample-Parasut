using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace XpodaParasutAddon
{
    public class ParasutClass
    {

        internal class CustomerAttributes
        {
            public string name { get; set; }
            public string email { get; set; }
            public string city { get; set; }
            public string district { get; set; }
        }

        internal class CustomerInfo
        {
            public string id { get; set; }

            public CustomerAttributes attributes { get; set; }
        }

        internal class CustomerList
        {
            public List<CustomerInfo> Data { get; set; }

        }


        internal class AuthResponse
        {
            public string access_token { get; set; }

            public string token_type { get; set; }

            public int expires_in { get; set; }

            public string refresh_token { get; set; }

        }


        // Aşağıdaki test bilgilerini destek@parasut.com adresine mail atarak edinebilirsiniz.
        private static string _clientId = "xZ3B4YftwAzh7uhdEKF2oJGYzmirGfo465Rcgo";
        private static string _clientSecret = "kZyMByTQrDNBGsXH9tzsnnMzyfajKL1aPzR5OHw";
        private static string _apiUrl = "https://api.heroku-staging.parasut.com";
        private static string _customerId = "31662"; // FİRMA ID si , değiştirmeyi unutmayın

        // Aşağıdaki parametreler XPODA Client tarafından gelecek.
        //private static string _email = "test@test.com.tr";
        //private static string _pass = "Şifre"; 



        public ParasutClass()
        {
        }


        public static Dictionary<string, object> GetToken(List<Dictionary<string, object>> parameters)
        {

            var result = new Dictionary<string, object>();

            var List = new List<Dictionary<string, object>>();

            result["Error"] = "";

            HttpClient _http = new HttpClient();

            string username = parameters[0]["username"].ToString();
            string password = parameters[1]["password"].ToString();

            var formContent = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("client_id", _clientId),
                    new KeyValuePair<string, string>("client_secret", _clientSecret),
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("redirect_uri", "urn:ietf:wg:oauth:2.0:oob"),
                    new KeyValuePair<string, string>("username", username)
             });

            var response = _http.PostAsync(_apiUrl + "/oauth/token", formContent).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var success = JsonConvert.DeserializeObject<AuthResponse>(response.Content.ReadAsStringAsync().Result);

                List.Add(new Dictionary<string, object> { { "Result", success.access_token } });

                result["List"] = List;
            }
            else
            {
                throw new Exception("Paraşüt bilgileri hatalı.");
            }

            return result;
        }



        public static Dictionary<string, object> GetCustomerList(List<Dictionary<string, object>> parameters)
        {

            var result = new Dictionary<string, object>();

            var List = new List<Dictionary<string, object>>();

            result["Error"] = "";

            if (String.IsNullOrEmpty(parameters[0]["Token"].ToString()))
            {
                result["Error"] = "Token gönderilmelidir.";
                return result;
            }


            HttpClient _http = new HttpClient();

            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", parameters[0]["Token"].ToString());
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

            try
            {
                var response = _http.GetAsync(_apiUrl + "/v4/" + _customerId + "/contacts").Result;
                var success = JsonConvert.DeserializeObject<CustomerList>(response.Content.ReadAsStringAsync().Result);

                if (success != null)
                {
                    foreach (var customer in success.Data)
                    {
                        List.Add(new Dictionary<string, object> { { "ID", customer.id } });
                        List.Add(new Dictionary<string, object> { { "CUSTOMER NAME", customer.attributes.name } });
                    }
                }

                result["List"] = List;

            }
            catch (Exception ex)
            {
                result["Error"] = ex.Message;
            }

            return result;

        }

    }
}
