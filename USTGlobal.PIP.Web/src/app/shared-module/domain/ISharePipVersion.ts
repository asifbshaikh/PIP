import { IPIPSheet } from './IPIPsheet';
import { IUser } from '@global';
import { IProject } from '@shared/domain/project';

export interface ISharePipVersion {
  projectDTO: IProject;
  pipSheetDTO: IPIPSheet[];
  userDTO: IUser[];
}
