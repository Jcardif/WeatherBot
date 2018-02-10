using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Sample.LuisBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"], 
            ConfigurationManager.AppSettings["LuisAPIKey"], 
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("With blanks I cannot help");
            context.Wait(MessageReceived);
        }

        [LuisIntent("getWeather")]
        public async Task GetWeatherIntent(IDialogContext context, LuisResult result)
        {
            string weatherInfo;
            foreach (var entity in result.Entities)
            {
                var locationInfo = entity.Entity;
                var currentObservation = await GetCurrentWeatherUsingAPI(locationInfo);
                string displayLocation = currentObservation.display_location?.full;
                decimal tempC = currentObservation.temp_c;
                string weather = currentObservation.weather;
                weatherInfo = $"It is {weather} and {tempC} degrees in { displayLocation}.";
                string icon = currentObservation.icon;

                await context.PostAsync(weatherInfo);
            }
            context.Wait(MessageReceived);
        }
   
        [LuisIntent("Greeting")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hi, there how can I help?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Help")]
        public async Task HelpIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("The QnA section will be available soon");
            context.Wait(MessageReceived);
        }

        private static async Task<dynamic> GetCurrentWeatherUsingAPI(string location)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var escapedLocation = Regex.Replace(location, @"\W+", "_");
                    var jsonString = await
                    client.GetStringAsync($"http://api.wunderground.com/api/0f4f72363a56f28b/conditions/q/{escapedLocation}.json");
                    dynamic response = JObject.Parse(jsonString);
                    dynamic observation = response.current_observation;
                    dynamic results = response.response.results;
                    if (observation != null)
                    {
                        return observation;
                    }
                    else if (results != null)
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                }
                return null;
            }
        }
    }
}