export interface ISharePIP {
    versions?: number;
    readonly?: string;
    editor?: string;
    accountId: number;
    roleId: number;
    pipSheetId: number;
    projectId: number;
    versionNumber: number;
    sharedWithUserId: number;
    sharedWithUserName: string;
    shareComments: string;
    sharedWithUId: string;
    isEditClicked?: boolean;
    versionName: string;
}
