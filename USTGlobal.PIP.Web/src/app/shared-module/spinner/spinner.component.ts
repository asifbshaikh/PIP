import { Component } from '@angular/core';
import { SpinnerService } from '@core/spinner/spinner.service';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-spinner',
  templateUrl: './spinner.component.html',
  styleUrls: ['./spinner.component.scss']
})
export class SpinnerComponent {
  isLoading: Subject<boolean> = this.spinnerService.isLoading;
  constructor(private spinnerService: SpinnerService) { }
}
