import { environment } from '@env';

export let ConfigurationSettings = {
    defaultRoutePrefix: '',
    appURL: environment.appUrl,
    numberOfApiRetryAttempt: 3,
    LogLevel: 3, // 0-OFF , 1-ERROR , 2-WARN, 3-INFO, 4-DEBUG, 5-LOG
    isRethrowErrorInsideGlobalErrorHandler: false,
    isUnwrapErrorInsideGlobalErrorHandler: true,
    islogErrorToConsoleInsideGlobalErrorHandler: true,
    isSendErrorToServerInsideGlobalErrorHandler: true,
    isShowErrorDialogInsideGlobalErrorHandler: true,
    supportedBrowserLanguages: ['en', 'en-us', 'en-gb', 'fr'],
    fallbackBrowserLanguage: 'en'
};

export let ToastrOptions: any = {
    animate: 'fade',
    positionClass: 'toast-top-full-width',
    dismiss: 'click',
    maxShown: 1,
    showCloseButton: true,
    newestOnTop: true
};

export let AutoCloseToastrOptions: any = {
    toastLife: 5000, // in miliseconds
    dismiss: 'auto'
};

export let CustomToastrOptions: any = {
    toastLife: 5000,  // in miliseconds
    dismiss: 'auto',
    enableHTML: true
};

export let Holiday: any = [
    {
        'field': 'Id',
        'header': 'Id',
        'type': 'nil'
    },
    {
        'field': 'holidayName',
        'header': 'Holiday',
        'type': 'text'
    },
    {
        'field': 'date',
        'header': 'Date',
        'type': 'datepicker'
    },
    {
        'field': 'locationId',
        'header': 'LocationId',
        'type': 'nil'
    },
    {
        'field': 'locationName',
        'header': 'Location',
        'type': 'nil'
    }

];


export let Locations: any = [
    {
        'field': 'id',
        'header': 'Id',
        'type': 'nil'
    },
    {
        'field': 'LocationName',
        'header': 'Location Name',
        'type': 'dropdown'
    },
    ,
    {
        'field': 'hoursPerDay',
        'header': 'Hours / Day',
        'type': 'text'
    },
    {
        'field': 'hoursPerMonth',
        'header': 'Hours / Month',
        'type': 'text'
    }

];

export let Milestone: any = [
    {
        'field': 'id',
        'header': 'Id',
        'type': 'nil'
    },
    {
        'field': 'milestoneName',
        'header': 'Milestone',
        'type': 'text'
    },
    ,
    {
        'field': 'milestoneGroupId',
        'header': 'Hours / Day',
        'type': 'nil'
    },
    {
        'field': 'milestoneGroupName',
        'header': 'Milestone Group',
        'type': 'text'
    }

];
