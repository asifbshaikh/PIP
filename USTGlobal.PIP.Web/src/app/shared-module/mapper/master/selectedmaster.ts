import { IMasterDetails } from '@shared/domain/masterdetails';
import { MasterColumn } from '@shared/domain/mastercolumn';
import { IMapper } from '../imapper';
import { Pipmasters } from '@shared/domain/pipmasters.enum';
import { Locations } from '@shared/infrastructure';

export class SelectedMaster implements IMasterDetails, IMapper {

    masterId: number;
    masterName: string;
    cols: MasterColumn[] = [];
    data: any;
    isEdit = false;
    isView = false;
    isCreate = false;
    isSelected = false;

    private masters: SelectedMaster[] = [];

    mapper(input: any): SelectedMaster[] {
        this.DefineAdditionalProperties(input);
        Object.assign(this, input);
        this.AssociateColumns(this);
        return this.masters;
    }

    private AssociateColumns(data: this) {
        Object.assign(this.masters, data);
        if (this.masters) {
            this.masters.forEach(master => {
                switch (master.masterName) {
                    case Pipmasters.location:
                        Object.assign(master.cols, Locations);
                        break;

                    default:
                        break;
                }
            });

        }

    }

    private DefineAdditionalProperties(input: any) {

        if (input) {
            let length = 0;
            length = input.length;

            for (let index = 0; index < length; index++) {
                Object.defineProperties(input[index], {
                    cols: { value: [], writable: true, enumerable: true, configurable: true },
                    isEdit: { value: true, writable: true, enumerable: true, configurable: true },
                    isView: { value: true, writable: true, enumerable: true, configurable: true },
                    isCreate: { value: true, writable: true, enumerable: true, configurable: true },
                    isSelected: { value: true, writable: true, enumerable: true, configurable: true },
                });
            }
        }
    }
}


