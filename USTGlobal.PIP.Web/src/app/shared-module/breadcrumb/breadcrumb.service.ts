import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Breadcrumb } from './breadcrumb.model';

@Injectable()
export class BreadcrumbService {

  public breadcrumbLabels: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);
  public newBreadcrumb: BehaviorSubject<Breadcrumb[]> = new BehaviorSubject<Breadcrumb[]>([]);

  constructor() { }

  updateBreadcrumbLabels(labels: any) {
    this.breadcrumbLabels.next(labels);
  }

  updateBreadcrumb(newBreadcrumb: Breadcrumb[]) {
    this.newBreadcrumb.next(newBreadcrumb);
  }

}
