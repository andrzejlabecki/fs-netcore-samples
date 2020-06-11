import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';  
import { Order } from '../shared/models/order';
import { Report } from '../shared/models/report';
import { AuthorizeService } from '../../api-authorization/authorize.service';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  urlOrderService = '';
  private token: string;
  
  constructor(private http: HttpClient, private authorizeService: AuthorizeService) {
    this.urlOrderService = 'https://fsapi.netpoc.com/order';

    this.token = '';

    authorizeService.getAccessToken().subscribe(result => {
    this.token = result;
    });
  }

    getAllReports(): Observable<Report[]> {
      this.authorizeService.getAccessToken().subscribe(result => {
        this.token = result;
      });

      const httpOptions = { headers: new HttpHeaders({ 'Authorization': 'Bearer ' + this.token }) };

      return this.http.get<Report[]>(this.urlOrderService + '/reports', httpOptions);
    }

  getAllOrders(): Observable<Order[]> {
    this.authorizeService.getAccessToken().subscribe(result => {
      this.token = result;
    });

    const httpOptions = { headers: new HttpHeaders({ 'Authorization': 'Bearer ' + this.token }) };

    return this.http.get<Order[]>(this.urlOrderService + '/orders', httpOptions);
  }

  getOrders(order: string): Observable<Order[]> {
    this.authorizeService.getAccessToken().subscribe(result => {
      this.token = result;
    });

    const httpOptions = {
      headers: new HttpHeaders({ 'Authorization': 'Bearer ' + this.token }),
      params: new HttpParams().set('order', order)};

    return this.http.get<Order[]>(this.urlOrderService + '/search', httpOptions);
  }

  getOrderById(orderId: string): Observable<Order> {
    this.authorizeService.getAccessToken().subscribe(result => {
      this.token = result;
    });

    const httpOptions = { headers: new HttpHeaders({ 'Authorization': 'Bearer ' + this.token }) };

    return this.http.get<Order>(this.urlOrderService + '/' + orderId, httpOptions);
  }

  createOrder(order: Order): Observable<Order> {
    const httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this.token })};

    this.authorizeService.getAccessToken().subscribe(result => {
      this.token = result;
    });

    return this.http.post<Order>(this.urlOrderService, order, httpOptions);
  }

  updateOrder(order: Order): Observable<Order> {
    const httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this.token })
    };

    this.authorizeService.getAccessToken().subscribe(result => {
      this.token = result;
    });

    return this.http.put<Order>(this.urlOrderService,
      order, httpOptions);
  }

  deleteOrderById(orderId: string): Observable<number> {
    const httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this.token })
    };

    this.authorizeService.getAccessToken().subscribe(result => {
      this.token = result;
    });

    return this.http.delete<number>(this.urlOrderService + '/' + orderId,
      httpOptions);
  }

  createReport(report: Report): Observable<Report> {
    const httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this.token })
    };

    this.authorizeService.getAccessToken().subscribe(result => {
      this.token = result;
    });

    return this.http.post<Report>(this.urlOrderService + '/reports',
      report, httpOptions);
  }

  updateReport(report: Report): Observable<Report> {
    const httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this.token })
    };

    this.authorizeService.getAccessToken().subscribe(result => {
      this.token = result;
    });

    return this.http.put<Report>(this.urlOrderService + '/reports',
      report, httpOptions);
  }

}

