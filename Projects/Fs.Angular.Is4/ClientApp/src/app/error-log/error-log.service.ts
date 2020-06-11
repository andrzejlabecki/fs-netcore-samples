import { Injectable } from '@angular/core';
import { Inject } from '@angular/core';
import { HttpErrorResponse, HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable()
export class ErrorLogService {
  private name = 'ErrorLogService';

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
  }

  logError(error) {
    let err = null;
    if (error instanceof HttpErrorResponse) {
      console.error('There was an HTTP error.', (error as HttpErrorResponse).message, 'Custom Error Message:', (error as HttpErrorResponse).error.message);
      alert('There was an HTTP error.\n' + (error as HttpErrorResponse).message + '\nCustom Error Message= ' + (error as HttpErrorResponse).error.message);
      err = {
        type: 'HttpErrorResponse',
        message: (error as HttpErrorResponse).message,
        customMessage: (error as HttpErrorResponse).error.message,
        page: (error as HttpErrorResponse).url,
        stack: ''
      };
    } else if (error instanceof TypeError) {
      console.error('There was a Type error.', error.message);
      alert('There was a Type error.\n' + error.message);
      err = {
        type: 'TypeError',
        message: error.name + ' ' + error.message,
        customMessage: '',
        page: window.location.href,
        stack: error.stack
      };
      this.addErrorLog(err);
    } else if (error instanceof Error) {
      console.error('There was a general error.', error.message);
      alert('There was a general error.\n' + error.message);
      err = {
        type: 'Error',
        message: error.name + ' ' + error.message,
        customMessage: '',
        page: window.location.href,
        stack: error.stack
      };
      this.addErrorLog(err);
    } else {
      console.error('Nobody threw an error but something happened!', error);
      alert('Nobody threw an error but something happened!\n ' + error);
      err = {
        type: 'Unknown',
        message: error.name + ' ' + error.message,
        customMessage: '',
        page: window.location.href,
        stack: error.stack
      };
      this.addErrorLog(err);
    }
  }

  addErrorLog(error) {
    if (error.type.includes('HttpErrorResponse'))
      return;

    const headers = new HttpHeaders({
      'Content-Type': 'application/json; charset=utf-8'
    });
    console.log('add err: ' + JSON.stringify(error));
    this.http.post(this.baseUrl + 'errorlog', JSON.stringify(error), { headers: headers }).subscribe(complete => { },
      error => { console.error(error); window.alert(error.message + '\nCustom Error Message= ' + error.error.message); });
  }
}

interface ErrorLog {
  type: string;
  message: string;
  customMessage: string;
  page: string;
  stack: string;
}
