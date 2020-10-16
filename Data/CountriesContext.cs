using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Liveperson.Data
{
    public class CountriesContext : DbContext
    {



        public CountriesContext(DbContextOptions<CountriesContext> option) :base(option)
        {
            this.Database.EnsureCreated();
            if (this.Countries == null || this.Countries.Any() == false)
            {
                try
                {
                    var client = new RestClient("https://restcountries-v1.p.rapidapi.com/all");
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("x-rapidapi-host", "restcountries-v1.p.rapidapi.com");
                    request.AddHeader("x-rapidapi-key", "a75567071fmsh1da754f34ee180dp16140cjsnd0ce561437aa");
                    IRestResponse response = client.Execute(request);

                    this.Countries = JsonConvert.DeserializeObject<List<Country>>(response.Content);
                    //    File.WriteAllText("c:\\tmp\\countries.json", response.Content);

                }
                catch
                {
                    string countriesJson = System.IO.File.ReadAllText("c:\\tmp\\countries.json");
                    this.Countries = JsonConvert.DeserializeObject<List<Country>>(countriesJson);
                }
            }
        }

  

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("Liveperson");
        }


        public static DbContextOptions<CountriesContext> GetInMemoryDbContextOptions(string dbName = "Liveperson")
        {
            var options = new DbContextOptionsBuilder<CountriesContext>()
                .UseInMemoryDatabase("Liveperson")
                .Options;

            return options;
        }


        public List<Country> Countries { get; set; }
    }
}
