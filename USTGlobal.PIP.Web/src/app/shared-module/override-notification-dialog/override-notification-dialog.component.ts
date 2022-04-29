import { Component, OnInit } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/api';
import { UserWorkflowService } from '@shared/services/user-workflow.service';
import { OverrideNotificationStatus } from '@shared/domain/override-notification-status';

@Component({
  selector: 'app-override-notification-dialog',
  templateUrl: './override-notification-dialog.component.html',
})
export class OverrideNotificationDialogComponent implements OnInit {
  dialogData: any;
  pipSheetId: number;
  overrideStatus: OverrideNotificationStatus;
  vacationAbsence: any;
  riskManagement: any;
  clientPrice: any;
  ebitdaStdOverhead: any;
  constructor(private userWorkflowService: UserWorkflowService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
  ) { }

  ngOnInit() {
    this.dialogData = this.config.data;
    this.userWorkflowService.getOverrideNotificationStatus(+this.dialogData.pipSheetId).subscribe(item => {
      this.overrideStatus = item;
      this.vacationAbsence = this.overrideStatus.vacationAbsence ? 'Vacation Absences' : '';
      this.clientPrice = this.overrideStatus.clientPrice ? 'Invoicing Schedule' : '';
      this.riskManagement = this.overrideStatus.riskManagement ? 'Risk Management' : '';
      this.ebitdaStdOverhead = this.overrideStatus.ebitdaStdOverhead ? 'Ebitda and Std. Overhead' : '';
    });
  }
  closeNotificaltionDialog(): void {
    this.ref.close();
  }

}
