import { Component } from '@angular/core';



@Component({
  selector: 'app-root', //The most important. It tells angular to instantiate this component when the <app-root> tag is found in the HTML 
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'healthcheck.client';
}
