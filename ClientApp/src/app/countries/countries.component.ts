import { Component, Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as $ from "jquery";



@Component({
  selector: 'app-countries',
  templateUrl: './countries.component.html'
})
@Injectable()
export class CountriesComponent {
  public countreis: Country[];
  public filterCountries: Country[];
  public selectedCountry: Country;
  config: any;
  SearchText: string;
  http2: HttpClient;
  baseUrl2: string;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Country[]>(baseUrl + 'api/Countries/GetCountries').subscribe(result => {
      this.countreis = result;
      this.filterCountries = this.countreis;


        this.config = {
        itemsPerPage: 10,
        currentPage: 1,
          totalItems: result.length
        };
      


    }, error => console.error(error));


    this.http2 = http;
    this.baseUrl2 = baseUrl;


  }





  selectCountry(country: Country, ) {
    this.selectedCountry = country;
    $('.panel-body').show();


    this.http2.get(this.baseUrl2 + 'api/Countries/GetWiki?name=' + country.name, { responseType: "text" }).subscribe(result => {
      result;
      this.selectedCountry.wiki = result;

    },

      error => console.error(error));

  }
  


  SearchFn(searchText: string) {
    this.filterCountries = new Array<Country>();

    if (this.countreis && this.countreis.length > 0) {

      this.countreis.forEach(country => {
        if (country.capital.toLowerCase().includes(searchText.toLowerCase()) || country.name.toLowerCase().includes(searchText.toLowerCase()))
          this.filterCountries.push(country);
      });
    }

  }

  pageChanged(event) {
    this.config.currentPage = event;
  }



}

interface Country {
  name: string;
  capital: string;
  languages: string;
  timezones: string;
  borders: string;
  flag: string;
  population: string;
  nativeName: string;
  currencies: string;
  region: string;
  wiki: string;
}
