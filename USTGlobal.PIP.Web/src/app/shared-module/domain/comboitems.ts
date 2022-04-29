import { SelectItem } from 'primeng/api';

// this class is to adapt the values in required multiselect dropdown format.

export class ComboItems implements SelectItem {
    label?: any;
    value: any;
    styleClass?: string;
    icon?: string;
    title?: string;
    disabled?: boolean;
}
