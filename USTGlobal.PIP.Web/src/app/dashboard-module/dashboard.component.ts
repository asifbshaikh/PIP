import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Constants, IFile } from '@shared';
import { Observable } from 'rxjs';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  public src: string;

  constructor(private httpClient: HttpClient) { }

  ngOnInit() {
    this.getDashboardFile().subscribe(data => {
      if (!isNullOrUndefined(data.filePath)) {
        this.src = data.filePath;
      }
      else {
        this.src = '/assets/AboutToolDefault.pdf';
      }
    });
  }

  getDashboardFile(): Observable<IFile> {
    return this.httpClient.get<IFile>(Constants.webApis.getDashboardFile);
  }
}
