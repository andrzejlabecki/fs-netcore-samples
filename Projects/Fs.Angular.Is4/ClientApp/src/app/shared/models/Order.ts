import { Report } from './report';

export class Order {
  orderId: number;
  name: string;
  addedDate: Date;
  modifiedDate: Date;
  reports: Report[];
}
