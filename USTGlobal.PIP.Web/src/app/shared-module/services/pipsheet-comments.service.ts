import { Observable } from 'rxjs';
import { Constants } from '@shared/infrastructure/constants';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IPIPSheetComment } from '@shared/domain/IPIPSheetComment';

@Injectable({
  providedIn: 'root'
})
export class PipsheetCommentsService {

  constructor(private httpClient: HttpClient) { }

  GetPIPSheetComments(pipSheetId: any) {
    return this.httpClient.get<IPIPSheetComment[]>(Constants.webApis.getPIPSheetComments.replace('{pipSheetId}', pipSheetId));
  }

  SavePIPSheetComment(comment: IPIPSheetComment): Observable<number> {
    return this.httpClient.post<number>(Constants.webApis.savePIPSheetComment, comment);
  }

  DeletePIPSheetComment(pipSheetCommentId: any, projectId: any) {
    return this.httpClient.delete(Constants.webApis.deletePIPSheetComment.replace('{pipSheetCommentId}', pipSheetCommentId)
      .replace('{projectId}', projectId));
  }
}
