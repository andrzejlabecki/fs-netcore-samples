import { Component } from '@angular/core';

@Component({
  selector: 'app-counter-component',
  templateUrl: './counter.component.html'
})
export class CounterComponent {
  public currentCount = 0;
  public errorMessage = '';

  public incrementCounter() {
    this.currentCount++;

    //this.errorMessage = eval('var ggg = g / h');
    //throw 'Test error';
  }


}
