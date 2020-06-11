import { Order } from './order';

export class Report {
  reportId: number;
  name: string;
  addedDate: Date;
  modifiedDate: Date;
  order: Order;
}
