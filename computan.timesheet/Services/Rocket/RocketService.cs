using computan.timesheet.Contexts;
using computan.timesheet.core.common;
using computan.timesheet.Models;
using computan.timesheet.Models.Rocket;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;

namespace computan.timesheet.Services.Rocket
{
    public static class RocketService
    {
        private static string userId = string.Empty;
        public static string CreateUser(RocketSetting setting, RegisterViewModel userViewModel)
        {
            using (var httpClient = new HttpClient())
            {
                string username = userViewModel.Email.Split('@')[0];
                RocketCreateViewModel rocketChat = new RocketCreateViewModel()
                {
                    name = userViewModel.FirstName.ToLower() + " " + userViewModel.LastName.ToLower(),
                    email = userViewModel.Email.ToLower(),
                    pass = userViewModel.Password,
                    username = username.ToLower(),
                    verified =true,
                    secretURL = setting.SecretURL
                };
                var client = new RestClient(setting.Baseurl + "users.register");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("X-Auth-Token", setting.AuthToken);
                var body = JsonConvert.SerializeObject(rocketChat);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                return response.StatusCode.ToString();
            }
        }

        public static string ChangeUserStatus(RocketSetting setting, EditUserViewModel userViewModel)
        {
            string username = userViewModel.OldEmail.Split('@')[0].ToLower();
            var isExist = GetRocketUserId(setting, username);
            if (isExist == APIResponse.OK.ToString())
            {
                //RocketStatusViewModel statusModel = new RocketStatusViewModel()
                //{
                //    activeStatus = userViewModel.isactive,
                //    userId = userId,
                //};
                RocketUpdateViewModel user = new RocketUpdateViewModel()
                {
                    data = new RocketStatusViewModel()
                    {
                        active = userViewModel.isactive,
                        email = userViewModel.Email.ToLower(),
                        name = userViewModel.FirstName.ToLower() + "" + userViewModel.LastName.ToLower(),
                        username = userViewModel.Email.Split('@')[0].ToLower(),
                        verified = true

                    },
                    userId = userId,
                };
                var client = new RestClient(setting.Baseurl + "users.update");
                //var client = new RestClient(setting.Baseurl + "users.setActiveStatus"); //edit
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("X-Auth-Token", setting.AuthToken);
                request.AddHeader("X-User-Id", setting.UserId);
                request.AddHeader("Content-Type", "application/json");
                var body = JsonConvert.SerializeObject(user);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                return response.StatusCode.ToString();
            }
            return isExist;
              
        } 
        
        public static string GetRocketUserId(RocketSetting setting, string userName= "shussain")
        {
            //userName = "shussain";
            var client = new RestClient(setting.Baseurl + "users.info?username=" + userName);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-Auth-Token", setting.AuthToken);
            request.AddHeader("X-User-Id", setting.UserId);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Root res = JsonConvert.DeserializeObject<Root>(response.Content);
                userId= res.user._id;
            }
            return response.StatusCode.ToString();
        }
        
       
    }
}