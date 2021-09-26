using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace OTP
{
    public class SMSInfo
    {
        public string api_token;
        public string sid;
        public string msisdn;
        public string sms;
        public string csms_id;
    }
    public class ResponseModel
    {
        public string status;
        public string status_code;
        public string error_message;
    }
    class Program
    {
        private string OTP { get; set; }
        private DateTime OTPTime { get; set; }

        public static IDictionary<string, string> CreateQueryParameters(string phone)
        {
            //create query parameters
            return new Dictionary<string, string>
            {
                //api_token
                ["api_token"] = "1279-98d2bb25-3f7e-49bf-a1e2-5d1a6c6c588f",

                //sid
                ["sid"] = "Test",

                //msisdn
                ["msisdn"] = phone,

                //sms
                ["sms"] = "Message Body",

                //csms_id
                ["csms_id"] = "4473433434pZ684333392",
            };
        }

        // SendOTP
        private string SendOTP(string phone)
        {
            string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            OTP = GenerateRandomOTP(6, saAllowedCharacters);
            OTPTime = DateTime.Now;
            //_httpContext.Session["GenerateOTP"] = GenerateRandomOTP(6, saAllowedCharacters);
            //_httpContext.Session["GenerateOTPTime"] = DateTime.Now;

            var client = new HttpClient();

            var formContent = new
            {
                api_token = "1279-98d2bb25-3f7e-49bf-a1e2-5d1a6c6c588f",
                sid = "Test",
                msisdn = phone,
                sms = string.Format("Dear Sir, {0} is your OTP for Registration. This OTP will expire within 5 minutes.", OTP.ToString()),
                csms_id = "4473433434pZ684333392"
            };

            //var formContent = new SMSInfo
            //{
            //    api_token = "1279-98d2bb25-3f7e-49bf-a1e2-5d1a6c6c588f",
            //    sid = "Test",
            //    msisdn = phone,
            //    sms = "message body",
            //    csms_id = "4473433434pz684333392"
            //};

            //var queryParameters = CreateQueryParameters(phone);
            //var stringContent = new FormUrlEncodedContent(queryParameters);

            var stringContent = new StringContent(JsonConvert.SerializeObject(formContent), UnicodeEncoding.UTF8, "application/json");
            var response = client.PostAsync("https://smsplus.sslwireless.com/api/v3/send-sms", stringContent).GetAwaiter().GetResult();

            var curlRequestContent = response.Content.ReadAsStringAsync();
            curlRequestContent.Wait();
            var contentResult = curlRequestContent.Result;
            ResponseModel responseModel = JsonConvert.DeserializeObject<ResponseModel>(contentResult);

            return responseModel.status_code;
        }

        // Generate OTP
        public static string GenerateRandomOTP(int iOTPLength, string[] saAllowedCharacters)
        {
            string sOTP = String.Empty;
            string sTempChars = String.Empty;

            Random rand = new Random();

            for (int i = 0; i < iOTPLength; i++)
            {
                int p = rand.Next(0, saAllowedCharacters.Length);
                sTempChars = saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];
                sOTP += sTempChars;
            }
            return sOTP;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter your phone no: ");
            var phone = Console.ReadLine();

            Program program = new Program();
            var status_code =  program.SendOTP(phone);
            if(status_code == "200")
            {
                Console.WriteLine("OTP send successfully");
            }

            Console.WriteLine("Enter your OTP: ");
            var otp = Console.ReadLine();

            DateTime otpTime = DateTime.Now;
            TimeSpan ts = otpTime - program.OTPTime;
            var time = ((int)ts.TotalMinutes);

            if (program.OTP == otp && time < 5)
                Console.WriteLine("Successfully verified");
            else
                Console.WriteLine("Invalid OTP!!!");
        }
    }
}
