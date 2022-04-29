import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DirtyCheckService {

  isDirty: boolean;

  notifyFormDirty() {}

}
