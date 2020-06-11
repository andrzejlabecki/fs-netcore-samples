import { Component, Inject } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { OrderService } from '../services//order.service';
import { Order } from '../shared/models/order';
import { Report } from '../shared/models/report';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styles: ['.modal-size {width: 200px;height: 200px;}']
})
export class ReportComponent {

  form: FormGroup;

  public reports: Report[];
  public report: Report;
  private dialog; 

  columnDefs = [
    { headerName: 'Order#', field: 'order.orderId', sortable: true, filter: true },
    { headerName: 'Order Name', field: 'order.name', sortable: true, filter: true },
    { headerName: 'Report#', field: 'reportId', sortable: false, filter: false },
    { headerName: 'Report Name', field: 'name', sortable: true, filter: true }
  ];

  gridOptions = {
    defaultColDef: {
      resizable: true,
    },
    pagination: true,
    paginationPageSize: 5,
    onFirstDataRendered: this.onFirstDataRendered,
  };


  constructor(fb: FormBuilder, private modalService: NgbModal, private orderService: OrderService) {
    this.form = fb.group({
      "orderName": ["", Validators.required],
      "reportName": ["", Validators.required]
    });
  }

  ngOnInit() {
    this.orderService.getAllReports().subscribe(result => {
      this.reports = result;
    });
  }

  public onFirstDataRendered(params) {
    params.api.sizeColumnsToFit();
  }

  public addReport(templateRef) {

   this.dialog = this.modalService.open(templateRef, { 
     windowClass: 'modal-size'
    });

  }

  public editReport(report: Report) {
  }

  public deleteReport(report: Report) {
  }

  public onFormSubmit() {
    let order = new Order();
    order.name = this.form.value.orderName;
    order.orderId = 0;
    let report = new Report();
    report.name = this.form.value.reportName;
    report.reportId = 0;
    order.reports = [report];

    this.orderService.createOrder(order).subscribe(
      () => {
        this.orderService.getAllReports().subscribe(result => {
          this.reports = result;
        });
        this.form.reset();
        this.closeDialog();
      }
    );
  }

  closeDialog() {
    this.dialog.close();
  }


}


