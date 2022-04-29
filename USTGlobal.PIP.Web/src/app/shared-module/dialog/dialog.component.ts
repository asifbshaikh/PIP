import { Component, OnInit } from '@angular/core';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { NotificationService } from '@global';

@Component({
  selector: 'confirm-dialog',
  templateUrl: './dialog.component.html'
})
export class DialogComponent implements OnInit {

  isVisible: boolean;
  isDirty: boolean;

  constructor(private confirmationService: ConfirmationService,
    private notificationService: NotificationService) { }

  ngOnInit() {

    // set property to display the dialog or not
    this.notificationService.isFormDirty.subscribe(isDirty => {
      this.isVisible = isDirty;
    });

    // invoke the dialog
    this.notificationService.displayDialog.subscribe(() => {
      this.confirmationService.confirm({
        message: 'You have unsaved changes. Please click ‘SAVE’ to proceed.',
        accept: () => {
          // Actual logic to perform a confirmation
        },
        acceptLabel: 'Ok',
        rejectVisible: false
      });
    });
  }
}
