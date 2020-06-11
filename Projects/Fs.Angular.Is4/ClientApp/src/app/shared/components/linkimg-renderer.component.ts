import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  template: '<a [routerLink]="" [title]="params.title" queryParamsHandling="preserve" (click)="clicked($event);"><span [innerHtml]="innerHtml"></span></a>'
})
export class LinkImgRendererComponent implements ICellRendererAngularComp  {
  params: any;
  label: string;
  innerHtml: any;

  constructor(private sanitized: DomSanitizer) {
  }

  agInit(params: any): void {
    this.params = params;
    this.innerHtml = this.sanitized.bypassSecurityTrustHtml(params.value);
    //(click)="viewOrder({{params.data.value}})"
    //this.params.context.componentParent
  }

  refresh(params: any): boolean {
    return false;
  }

  clicked($event) {
    if (this.params.onClick instanceof Function) {
      const params = {
        event: $event,
        rowData: this.params
      }
      this.params.onClick(params);

    }
  }
}
