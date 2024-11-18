//Imports
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-health-check',
  templateUrl: './health-check.component.html',
  styleUrl: './health-check.component.css'
})

//Implement the onInit interface by adding the implements onInit instruction to add
//type safety. This way, we wont risk typing or spelling mistakes within the ngOnInit lifecycle hook
export class HealthCheckComponent implements OnInit {
  public result?: Result;
  //In the constructor of the component, we instantiate the HTTPClient service
  //using dependency injection
  constructor(private http: HttpClient) {
  }

  ngOnInit() {
    this.http.get<Result>(environment.baseUrl + 'api/health').subscribe(result => {
      this.result = result;
    }, error => console.error(error));
  }
}
//Here we define 2 interfaces to deal with the JSON request we expect to receive from the check
interface Result {
  checks: Check[];
  totalStatus: string;
  totalResponseTime: number;
}

interface Check {
  name: string;
  responseTime: number;
  status: string;
  description: string;
}
