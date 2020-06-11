import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';

@Component({
  template: '<a [routerLink]="" queryParamsHandling="preserve" (click)="clicked($event);">{{params.value}}</a>'
})
export class LinkRendererComponent implements ICellRendererAngularComp  {
  params: any;
  label: string;

  agInit(params: any): void {
    this.params = params;
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
