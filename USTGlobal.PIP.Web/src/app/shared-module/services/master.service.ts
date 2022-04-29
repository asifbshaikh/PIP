import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IMasterDetails } from '@shared/domain/masterdetails';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Mastermapper } from '../mapper/master/mastermapper';
import { SelectedMaster } from '@shared/mapper/master/selectedmaster';
import { Constants } from '@shared/infrastructure';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

const url = 'https://localhost:44323/api/Master';

@Injectable({
  providedIn: 'root'
})
export class MasterService {

  constructor(private httpClient: HttpClient) { }

  public getAllMasters(): Observable<any> {
    return;
  }

  // this method will have selected master list.
  public getSelectedMasters(): Observable<IMasterDetails[]> {
    return;
  }

  /**
   * saveMaster
   */
  public saveMaster(master: IMasterDetails): boolean {
    // Master Save Call
    return true;
  }

  //  From API calls

  getMasters(): Observable<any> {
    return this.httpClient.get<Mastermapper[]>(url + '/GetMasterList', httpOptions)
      .map(mastersList => new Mastermapper().getMasterComboItems(mastersList));
  }

  getMasterList(): Observable<any> {
    return this.httpClient.get<any>(Constants.webApis.getMasterList);
  }

  public GetSelectedMasters(): Observable<any> {
    return this.httpClient.get<Mastermapper[]>(url + '/GetMasters', httpOptions)
      .map(mastersList => new SelectedMaster().mapper(mastersList));
  }
}
