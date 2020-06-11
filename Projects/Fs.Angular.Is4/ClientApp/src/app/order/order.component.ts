import { Component, Inject } from '@angular/core';
import { ViewChild, TemplateRef } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { OrderService } from '../services//order.service';
import { Order } from '../shared/models/order';
import { Report } from '../shared/models/report';
import { LinkRendererComponent } from '../shared/components/link-renderer.component';
import { LinkImgRendererComponent } from '../shared/components/linkimg-renderer.component';
//import 'ag-grid-enterprise';

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styles: ['.modal-size {width: 200px;height: 200px;}',
    '.cell-center {text-align: center;}']
})
export class OrderComponent {
  @ViewChild('templateReport', { static: false }) templateRef;

  public formOrder: FormGroup;
  public formReport: FormGroup;
  public formSearch: FormGroup;
  public list: boolean;

  public orders: Order[];
  public order: Order;
  public report: Report;
  private dialog;
  public gridReportsApi;


  columnDefs = [
    {
      headerName: 'Order#',
      field: 'orderId',
      cellRenderer: 'agGroupCellRenderer',
      width: 150,
      headerCheckboxSelection: true,
      checkboxSelection: true,
    },
    {
      headerName: 'Order Name', field: 'name', sortable: true, filter: true, flex: 1,
      cellRendererFramework: LinkRendererComponent,
      //cellRenderer: LinkRendererComponent,
      cellRendererParams: {
        onClick: (params) => this.viewOrder(params),
      }
      //cellRenderer: (val) =>
      //  '<a href="#" (click)="viewOrder(\'${val.value}\)" >${val.value}</a>'
    },
  ];


  gridOptions = {
    defaultColDef: {
      resizable: true
    },
    pagination: true,
    paginationPageSize: 5,
    rowSelection: 'single',
    suppressCellSelection: true,
    domLayout: 'autoHeight',
    onFirstDataRendered: this.onFirstDataRendered,
    context: {
      componentParent: this
    }
    /*masterDetail: true,
    detailCellRendererParams: {
      detailGridOptions: {
        columnDefs: [
          { headerName: 'Report#', field: 'reportId', sortable: false, filter: false, width: 150 },
          { headerName: 'Report Name', field: 'name', sortable: false, filter: false, flex: 1 },
        ],
      },
      getDetailRowData: this.getDetailRowData
    }*/
  };

  columnReportsDefs = [
    { headerName: 'Report#', field: 'reportId', width: 150 },
    {
      headerName: 'Report Name', field: 'name', sortable: true, filter: true, flex: 1,
      cellRendererFramework: LinkRendererComponent,
      //cellRenderer: LinkRendererComponent,
      cellRendererParams: {
        onClick: (params) => this.editReport(params),
      }
    },
    {
      headerName: '', sortable: false, filter: false, width: 70, resizable: false,
      cellStyle: { 'text-align': 'center' },
      cellRendererFramework: LinkImgRendererComponent,
      //cellRenderer: LinkRendererComponent,
      cellRendererParams: {
        value: "<img src='/assets/images/remove_grey.png'>",
        title: "Delete this report",
        onClick: (params) => this.deleteReport(params),
      }
    },
  ];

  gridReportsOptions = {
    defaultColDef: {
      resizable: true
    },
    pagination: false,
    onFirstDataRendered: this.onFirstDataRendered,
    context: {
      componentParent: this
    }
  };

  constructor(fb: FormBuilder, private modalService: NgbModal, private orderService: OrderService) {
    this.formOrder = fb.group({
      "orderName": ["", Validators.required]
    });

    this.formReport = fb.group({
      "reportName": ["", Validators.required]
    });

    this.formSearch = fb.group({
      "searchOrder": [""]
    });

    this.list = true;
  }

  ngOnInit() {
    this.orderService.getAllOrders().subscribe(result => {
      this.orders = result;
    });
  }

  /* grid events */
  public onFirstDataRendered(params) {
    params.api.sizeColumnsToFit();
  }

  public onReportsGridReady(params) {
    this.gridReportsApi = params.api;
  }

  public getDetailRowData (params) {
    params.successCallback(params.data.reports);
  }

  closeDialog() {
    this.report = null;
    this.dialog.close();
  }

  public addOrder(templateRef) {

    this.dialog = this.modalService.open(templateRef, {
      windowClass: 'modal-size'
    });
  }

  public searchOrder() {
    const search = this.formSearch.value.searchOrder;
    this.orders = null;
    this.orderService.getOrders(search).subscribe(result => {
      this.orders = result;
    });
  }

  public editOrder(templateRef, order: Order) {
    this.dialog = this.modalService.open(templateRef, {
      windowClass: 'modal-size'
    });
  }

  public viewOrder(params: any) {
    //alert(params.rowData.value);
    //alert(params.rowData.data.orderId);
    this.list = false;
    this.order = params.rowData.data;
  }

  public backList() {
    this.list = true;
    this.order = null;
  }

  public deleteOrder(order: Order) {
  }

  public onOrderSubmit() {
    const order = new Order();
    order.name = this.formOrder.value.orderName;

    this.orderService.createOrder(order).subscribe(
      () => {
        this.orderService.getAllOrders().subscribe(result => {
          this.orders = result;
        });
        this.formOrder.reset();
        this.closeDialog();
      }
    );
  }

  /* report events */
  public addReport(templateRef) {
    this.report = null;
    this.dialog = this.modalService.open(templateRef, {
      windowClass: 'modal-size'
    });
  }

  public editReport(params: any) {
    this.report = params.rowData.data;
    this.formReport.patchValue({ reportName: this.report.name });
    this.dialog = this.modalService.open(this.templateRef, {
      windowClass: 'modal-size'
    });
  }

  public deleteReport(params: any) {
  }

  public onReportSubmit() {
    if (!this.report) {
      const report = new Report();
      report.name = this.formReport.value.reportName;
      report.order = this.order;
      report.reportId = 0;
      this.orderService.createReport(report).subscribe(
        (updatedReport) => {
          //this.gridReports.refreshCells();
          /*this.orderService.getAllOrders().subscribe(result => {
            this.orders = result;
          });*/
          this.formReport.reset();
          this.closeDialog();
          //this.order.reports.push(updatedReport);
          this.gridReportsApi.updateRowData({ add: [updatedReport] });
        }
      );
    }
    else {
      const report = new Report();
      report.name = this.formReport.value.reportName;
      report.order = this.order;
      report.reportId = this.report.reportId;
      report.addedDate = this.report.addedDate;
      this.orderService.updateReport(report).subscribe(
        (updatedReport) => {
          //this.gridReports.refreshCells();
          /*this.orderService.getAllOrders().subscribe(result => {
            this.orders = result;
          });*/
          this.report.name = updatedReport.name;
          this.formReport.reset();
          this.closeDialog();
          //this.order.reports.push(updatedReport);
          this.gridReportsApi.updateRowData({ update: [report] });
        }
      );
    }
  }


}


