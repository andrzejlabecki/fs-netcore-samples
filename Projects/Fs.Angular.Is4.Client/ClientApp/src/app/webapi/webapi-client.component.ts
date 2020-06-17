import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, tap } from 'rxjs/operators';

@Component({
  selector: 'app-webapi-client',
  templateUrl: './webapi-client.component.html'
})
export class WebApiClientComponent {
  public orders: Order[];
  urlOrderService = '';

  constructor(private http: HttpClient, @Inject('API_BASE_URL') private baseUrl: string) {
    this.urlOrderService = this.baseUrl + 'order';
  }

  ngOnInit() {
    this.http.get<Order[]>(this.urlOrderService + '/orders').subscribe(result => {
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
