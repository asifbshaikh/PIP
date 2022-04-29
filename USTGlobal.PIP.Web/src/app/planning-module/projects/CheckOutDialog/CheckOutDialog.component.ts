import { Component, OnInit } from '@angular/core';
import { DynamicDialogRef } from 'primeng/api';
import { DynamicDialogConfig } from 'primeng/api';
import { Router } from '@angular/router';
import { Constants } from '../../../shared-module/infrastructure';
import { IPipCheckIn } from '@shared/domain';
import { ProjectService } from '@shared/services/project.service';

@Component({
  selector: 'app-CheckOutDialog',
  templateUrl: './CheckOutDialog.component.html',
  styleUrls: ['./CheckOutDialog.component.scss']
})
export class CheckOutDialogComponent implements OnInit {
  dialogData: any;
  pipCheckIn: IPipCheckIn;
  dialogMessage: string;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private projectService: ProjectService,
    private router: Router,
  ) { }

  ngOnInit() {
    this.dialogData = this.config.data;
  }

  navigateToReadOnlyProject(): void {
    this.ref.close();
  }


}
