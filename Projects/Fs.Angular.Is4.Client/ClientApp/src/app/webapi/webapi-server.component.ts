import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, tap } from 'rxjs/operators';

@Component({
  selector: 'app-webapi-server',
  templateUrl: './webapi-server.component.html'
})
export class WebApiServerComponent {
  public orders: Order[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Order[]>(baseUrl + 'orderserver').subscribe(result => {
      this.orders = result;
    }, error => console.error(error));
  }
}

export class Order {
  orderId: number;
  name: string;
  addedDate: Date;
  modifiedDate: Date;
  reports: Report[];
}

export class Report {
  reportId: number;
  name: string;
  addedDate: Date;
  modifiedDate: Date;
  order: Order;
}
