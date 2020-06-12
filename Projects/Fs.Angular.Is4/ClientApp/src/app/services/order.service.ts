import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Order } from '../shared/models/order';
import { Report } from '../shared/models/report';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  urlOrderService = '';

  constructor(private http: HttpClient) {
    this.urlOrderService = 'https://fsapi.netpoc.com/order';
  }

  getAllReports(): Observable<Report[]> {
    return this.http.get<Report[]>(this.urlOrderService + '/reports');
  }

  getAllOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(this.urlOrderService + '/orders');
  }

  getOrders(order: string): Observable<Order[]> {
    const httpParams = {
      params: new HttpParams()
        .set('order', order)
    };
    return this.http.get<Order[]>(this.urlOrderService + '/search', httpParams);
  }

  getOrderById(orderId: string): Observable<Order> {
    return this.http.get<Order>(this.urlOrderService + '/' + orderId);
  }

  createOrder(order: Order): Observable<Order> {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this.http.post<Order>(this.urlOrderService,
      order, httpOptions);
  }

  updateOrder(order: Order): Observable<Order> {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this.http.put<Order>(this.urlOrderService,
      order, httpOptions);
  }

  deleteOrderById(orderId: string): Observable<number> {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this.http.delete<number>(this.urlOrderService + '/' + orderId,
      httpOptions);
  }

  createReport(report: Report): Observable<Report> {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this.http.post<Report>(this.urlOrderService + '/reports',
      report, httpOptions);
  }

  updateReport(report: Report): Observable<Report> {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this.http.put<Report>(this.urlOrderService + '/reports',
      report, httpOptions);
  }

}

