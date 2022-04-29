import { environment } from '@env';

export class Constants {

    static uiRoutes = {
        login: '',
        shop: 'shop',
        cart: 'cart'
    };

    static businessExceptions = {
        SessionExpired: 'SessionExpired',
        SessionKilled: 'SessionKilled',
        ErrorCode: 'ErrorCode',
        MessageCode: 'MessageCode'
    };

    static webApis = {
        getSharedData: environment.apiUrl + 'pipSheet/{pipSheetId}/sharedData/',
        getWorkflowStatusAccountRole: environment.apiUrl +
        'projectHeader/pipSheet/{pipSheetId}/account/{accountId}/project/{projectId}/workflowStatusAccountRole/',
    };

    static queryString = {
        SessionExpired: 'SessionExpired=true'
    };

    static localStorageKeys = {
        sessionId: 'sessionId',
        userId: 'userId',
        accessToken: 'accessToken',
    };

    static cookies =
    {
        sessionId: 'SessionId',
        apiContext: 'apiContext'
    };

}
