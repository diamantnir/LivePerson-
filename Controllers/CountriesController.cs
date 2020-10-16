using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Liveperson.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;

namespace Liveperson.Controllers
{
    [Route("api/[controller]")]
    public class CountriesController : Controller
    {
        private readonly CountriesContext _contex;

        public CountriesController(CountriesContext context)
        {

            _contex = context;


        }

      


        /// <summary>
        /// This method get name of country is parameter (for example: india) and return a short sammery from wikipedia about that country.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>text</returns>
        [HttpGet("[action]")]
        public ActionResult GetWiki(string name)
        {
            var client = new RestClient("https://en.wikipedia.org/wiki/"+ name);
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "restcountries-v1.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "a75567071fmsh1da754f34ee180dp16140cjsnd0ce561437aa");
            IRestResponse response = client.Execute(request);

            string res = "<p><b>"+ name + "</b>"+response.Content.Split(new string[] { "<p><b>"+ name.Trim()+ "</b>" }, StringSplitOptions.None)[1].Split(new string[] { "</p>" }, StringSplitOptions.None)[0];



            return Content(res);
        }

        /// <summary>
        /// Ask method basicly created for the Liveperson bot. it get as parameter a question (for example: "what is the capital of india") and do very simple analysis and return the result
        /// this is not an AI yet :)
        /// </summary>
        /// <param name="userMessage"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public ActionResult Ask(string userMessage)
        {
            string res = "Sorry i can't answer thas question. Please go to home tab for instructions";
            try
            {

                Country country = new Country();
                string s = userMessage;
                s = s.Trim();
                s.Replace("show", "").Replace("the", "").Replace("me", "").Replace("how", "").Replace("what", "").Replace("which", "");


                List<Country> countries = _contex.Countries;


                foreach (string word in s.Split(' '))
                {
                    foreach (Country c in countries)
                    {
                        if (word.Trim().ToLower() == c.name.Trim().ToLower())
                        {
                            country = c;
                            break;
                        }
                    }
                }

                s = s.ToLower().Trim();

                if (s.Contains("how many") || s.Contains("how mutch") || s.Contains("number"))
                {

                    if (s.Contains("border"))
                        res = country.borders.Count().ToString();
                    else if (s.Contains("currencies") || s.Contains("currency"))
                        res = country.currencies.Count().ToString();
                    else if (s.Contains("time"))
                        res = country.timezones.Count().ToString();
                    else if (s.Contains("population") || s.Contains("city") || s.Contains("citizen") || s.Contains("people"))
                        res = country.population.ToString();
                    else if (s.Contains("language"))
                        res = country.languages.Count().ToString();
                }
                else if (s.Contains("where is"))
                {
                    if(country.name!=null)
                    {
                        res = country.region;
                    }
                    else
                    {
                        foreach (string word in s.Split(' '))
                        {
                            foreach (Country c in countries)
                            {
                                if (word.Trim().ToLower() == c.capital.Trim().ToLower())
                                {
                                    country = c;
                                    break;
                                }
                            }
                        }

                        res = country.name;
                    }
                }
                else
                {
                    if (s.Contains("capital") || s.Contains("city"))
                        res = country.capital;
                    else if (s.Contains("border"))
                        res = String.Join(", ", country.borders.ToArray());
                    else if (s.Contains("currencies") || s.Contains("currency"))
                        res = String.Join(", ", country.currencies.ToArray());
                    else if (s.Contains("region") || s.Contains("continent"))
                        res = country.region;
                    else if (s.Contains("time"))
                        res = String.Join(", ", country.timezones.ToArray());
                    else if (s.Contains("population") || s.Contains("city"))
                        res = country.population.ToString();
                    else if (s.Contains("nativeName") || s.Contains("native name"))
                        res = country.nativeName;
                    else if (s.Contains("language"))
                        res = String.Join(", ", country.languages.ToArray());
                }
            }
            catch
            {

            }


            return Json(new { title = res });
        }

        /// <summary>
        /// GetCountries just return the list of all countries with some propertires based on the resource: https://restcountries-v1.p.rapidapi.com/all
        /// </summary>
        /// <returns>List of countries</returns>
        [HttpGet("[action]")]
        public IEnumerable<Country> GetCountries()
        {
            List<Country> countries = _contex.Countries;
            var dictionary = new Dictionary<string, string>();


            foreach (Country c in countries)
            {
                dictionary.Add(c.alpha3Code, c.name);
            }
            


            return Enumerable.Range(0, countries.Count - 1).Select(index => new Country
            {
                name = countries[index].name,
                capital = countries[index].capital,
                languages = countries[index].languages,
                timezones = countries[index].timezones,
                borders = countries[index].borders.Select(s => dictionary[s]).ToList(),
                flag = "https://www.countryflags.io/" + countries[index].alpha2Code + "/flat/64.png",
                population = countries[index].population,
                nativeName = countries[index].nativeName,
                currencies = countries[index].currencies,
                region = countries[index].region,
            });
        }

    }
}
