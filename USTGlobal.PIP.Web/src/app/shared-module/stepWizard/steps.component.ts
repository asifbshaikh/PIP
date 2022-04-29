import {
  Component,
  ContentChildren,
  QueryList,
  AfterContentInit,
  OnChanges,
  SimpleChanges,
  EventEmitter,
  Input,
  Output,
  ViewChild,
  ElementRef,
  OnInit
} from '@angular/core';
import { MenuItem } from 'primeng/components/common/api';
import { StepComponent } from './step.component';
import { ActivatedRoute, Router } from '@angular/router';
import { Constants } from '@shared';
import { NotificationService, SharedDataService } from '@global';

declare var $: any;

let SCROLL_BY_PIXEL = 300;

@Component({
  selector: 'pe-steps',
  templateUrl: 'steps.component.html',
})
export class StepsComponent implements AfterContentInit, OnChanges, OnInit {
  currentIndex: number;
  projectId: number;
  pipSheetId: number;
  accountId: number;
  dashboardId: number;
  isDirty: boolean;
  displayModal = false;
  isDummy: boolean;
  isLastStep: boolean;
  hasAccountLevelAccess: boolean;
  @Input() activeIndex = 0;
  @Input() styleClass: string;
  @Input() stepClass: string;
  @Output() activeIndexChange: EventEmitter<any> = new EventEmitter();
  @Output() change = new EventEmitter();

  @ViewChild('arrowL') arrowL: ElementRef;
  @ViewChild('arrowR') arrowR: ElementRef;
  @ViewChild('stepsContainer') stepsContainer: ElementRef;

  items: MenuItem[] = [];

  @ContentChildren(StepComponent) steps: QueryList<StepComponent>;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private notificationService: NotificationService,
    private sharedData: SharedDataService,
  ) {
    this.route.paramMap.subscribe(
      params => {
        this.projectId = parseInt(params.get(Constants.uiRoutes.routeParams.projectId), 10);
        this.pipSheetId = parseInt(params.get(Constants.uiRoutes.routeParams.pipSheetId), 10);
        this.accountId = parseInt(params.get(Constants.uiRoutes.routeParams.accountId), 10);
        this.dashboardId = parseInt(params.get(Constants.uiRoutes.routeParams.dashboardId), 10);
      });
  }

  ngOnInit() {
    this.notificationService.isFormDirty.subscribe(isDirty => {
      this.isDirty = isDirty;
      this.currentIndex = this.activeIndex;
    });
    this.isDummy = this.router.url.includes('samples') ? true : false;
    this.hasAccountLevelAccess = this.sharedData.sharedData.hasAccountLevelAccess;
  }

  ngAfterContentInit() {

    this.steps.toArray().forEach((step: StepComponent, index: number) => {
      if (!step.styleClass) {
        // set style class if it was not set on step component directly
        step.styleClass = this.stepClass;
      }

      if (index === this.activeIndex) {
        // show this step on init
        step.active = true;
      }

      this.items[index] = {
        label: step.label,
        command: (event: any) => {

          if (!this.isDirty) {

            // hide all steps
            this.steps.toArray().forEach((s: StepComponent) => s.active = false);

            // show the step the user has clicked on.
            step.active = true;
            this.activeIndex = index;

            // emit currently selected index (two-way binding)
            this.activeIndexChange.emit(index);
            // emit currently selected label
            this.change.next(step.label);
          } else {
            this.activeIndex = this.currentIndex;
            this.notificationService.showDialog();
          }
        }
      };
    });
  }

  ngOnChanges(changes: SimpleChanges) {

    if (!this.steps) {
      // we could also check changes['activeIndex'].isFirstChange()
      return;
    }

    if (!this.isDirty) {
      for (const prop in changes) {
        if (prop === 'activeIndex') {
          const curIndex = changes[prop].currentValue;
          this.steps.toArray().forEach((step: StepComponent, index: number) => {
            // show / hide the step
            const selected = index === curIndex;
            step.active = selected;

            if (selected) {
              // emit currently selected label
              this.change.next(step.label);
            }
          });
        }
      }
    }
  }

  public setElementTab(event) {
    setTimeout(() => {
      event.preventDefault();
      $('.jsFormContainer :input').first().focus();
    }, 500);
  }

  scrollToLeft() {
    this.stepsContainer.nativeElement.scrollLeft = 0;
  }
  scrollToRight() {
    this.stepsContainer.nativeElement.scrollLeft = SCROLL_BY_PIXEL += 300;
  }

  public previous(event) {
    if (!this.isDirty) {
      const root = this.router.url.includes('samples') ? 'samples' : 'projects';
      if (this.activeIndex < 6) {
        this.stepsContainer.nativeElement.scrollLeft = 0;
      }
      if (this.activeIndex === 0 && this.items[0].label === 'Std OH & Seat Cost') {
        this.router.navigate([`${root}/${this.projectId}/${this.pipSheetId}/${this.accountId}/${this.dashboardId}/Staff`]);
      }
      if (this.activeIndex === 0 && this.items[0].label === 'Project Header') {
        if (!this.hasAccountLevelAccess) {
          this.router.navigate([`/approver`]);
        }
        else {
          this.router.navigate([`${root}`]);
        }
      }
      if (this.activeIndex === 0 && this.items[0].label === 'Revised Summary') {
        this.router.navigate([`${root}/${this.projectId}/${this.pipSheetId}/${this.accountId}/${this.dashboardId}/Margin`]);
      }
      this.activeIndex--;
      // emit currently selected index (two-way binding)
      this.activeIndexChange.emit(this.activeIndex);
      // show / hide steps and emit selected label
      this.ngOnChanges({
        activeIndex: {
          currentValue: this.activeIndex,
          previousValue: this.activeIndex + 1,
          firstChange: false,
          isFirstChange: () => false
        }
      });
      setTimeout(() => {
        event.preventDefault();
        $('.jsFormContainer :input').first().focus();
      }, 500);
    } else {
      this.notificationService.showDialog();
    }
  }

  public next(event) {
    if (!this.isDirty) {
      const root = this.router.url.includes('samples') ? 'samples' : 'projects';
      if (this.activeIndex > 6) {
        this.stepsContainer.nativeElement.scrollLeft = SCROLL_BY_PIXEL += 300;
      }
      if (this.activeIndex === 2 && this.items[2].label === 'Resource Planning') {
        this.isLastStep = true;
        this.router.navigate([`${root}/${this.projectId}/${this.pipSheetId}/${this.accountId}/${this.dashboardId}/Margin`]);
      }
      if (this.activeIndex === 11 && this.items[11].label === 'Fixed Bid & Margin Calc') {
        this.isLastStep = true;
        this.router.navigate([`${root}/${this.projectId}/${this.pipSheetId}/${this.accountId}/${this.dashboardId}/Summary`]);
      }
      if (this.activeIndex === 0 && this.items[0].label === 'Revised Summary') {
        this.isLastStep = true;
        this.router.navigate([`${root}`]);
      }
      this.activeIndex++;
      // emit currently selected index (two-way binding)
      this.activeIndexChange.emit(this.activeIndex);
      // show / hide steps and emit selected label
      this.ngOnChanges({
        activeIndex: {
          currentValue: this.activeIndex,
          previousValue: this.activeIndex - 1,
          firstChange: false,
          isFirstChange: () => false
        }
      });
      setTimeout(() => {
        event.preventDefault();
        $('.jsFormContainer :input').first().focus();
      }, 500);
    } else {
      this.notificationService.showDialog();
    }

  }

  onExitClick(): void {
    this.notificationService.notifyFormDirty(false);
    if (this.dashboardId === 2) {
      this.router.navigate([
        Constants.uiRoutes.approver,
      ]);
    }
    else if (this.dashboardId === 1 || this.dashboardId === 3) {
      if (this.isDummy) {
        this.router.navigate([
          Constants.uiRoutes.sample
        ]);
      }
      else {
        this.router.navigate([
          Constants.uiRoutes.projects
        ]);
      }
    }
  }

  checkConfirmation() {
    if (this.isDirty) {
      this.displayModal = true;
    } else {
      this.onExitClick();
    }
  }
}
