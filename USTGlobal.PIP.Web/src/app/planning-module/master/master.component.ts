import { Component, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { SelectItem } from 'primeng/api';
import { MasterService } from '@shared/services/master.service';
import { IMasterDetails } from '@shared/domain/masterdetails';
import { IMasters } from '@shared/domain/master';
import { ComboItems } from '@shared/domain/comboitems';
import { ComboItemValue } from '@shared/domain/comboitemvalue';
import { CellType } from '@shared/infrastructure/enums';


@Component({
  selector: 'app-master',
  templateUrl: './master.component.html'
})
export class MasterComponent implements OnInit {


  masters: SelectItem[];
  mastersTest: SelectItem[];
  masterList: IMasters[];
  selectedMasters: ComboItemValue[];
  selectedMasterData: IMasterDetails[];
  locations: SelectItem[];
  isEditEnabled = false;
  editingMasterId: number;
  clonedMasterData: { [s: number]: any } = {};


  constructor(private masterService: MasterService) { }

  ngOnInit() {
    // binding all masters to the dropdown

    this.masterService.getAllMasters().subscribe(values => {
      this.masters = values;
    });

    // get all masters as per selection data
    this.masterService.getSelectedMasters().subscribe(values => {
      this.selectedMasterData = values;
    });

    // temporary binding the locations
    this.locations = [
      { label: 'India', value: 'India' },
      { label: 'Australia', value: 'Australia' },
      { label: 'New Zealand', value: 'NewZealand' },
      { label: 'China', value: 'China' },
      { label: 'US', value: 'US' },
      { label: 'South Africa', value: 'South Africa' },
      { label: 'Poland', value: 'Poland' },
      { label: 'England(UK)', value: 'England' },
      { label: 'Indonesia', value: 'Indonesia' },
      { label: 'switzerland', value: 'switzerland' }
    ];
  }


showHideMasters(items: any) {
    let data: ComboItemValue;
    const clonedMaster: IMasterDetails[] = this.clone(this.selectedMasterData);


    if (clonedMaster.length > 0) {
      clonedMaster.forEach(master => {
        if (items.length > 0) {
          data = items.find(val => val.id === master.masterId);
          if (data) {
            master.isView = true;
          } else {
            master.isView = false;
          }
        } else {
          master.isView = false;
        }
      });
      this.selectedMasterData = clonedMaster;
    }
  }

  getAllMasters() {
      this.masterService.getAllMasters().subscribe(data => {
      this.masterList = data;
    });
  }



  // Generic method to be clone the array...
  // Research on more better mechanism
  private clone(item: any): any {
    let data: IMasterDetails[];
    // data = [...this.selectedMasterData];   //  this  statement is best suited for shallow copy
    data = JSON.parse(JSON.stringify(this.selectedMasterData));
    return data;
  }

  private editingSelectedMaster(masterId: string) {

    const index = this.selectedMasterData.findIndex(data => data.masterId.toString() === masterId);
    if (index >= 0) {
      this.clonedMasterData = { ...this.selectedMasterData[index] };
    }
    // tslint:disable-next-line: radix
    this.editingMasterId = parseInt(masterId);
  }

  private getMasterDataItem(): IMasterDetails {
    const master: IMasterDetails = this.selectedMasterData.find(x => x.masterId === this.editingMasterId);
    if (master) {
      return master;
    } else {
      return null;  // revisit
    }
  }

  /*
    This method should be deprecated once we refactor the code to get editing master by index instead of there masterId
  */

  private getEditingMasterIndex(): number {
    let index = -1;
    if (this.editingMasterId > 0) {
      index = this.selectedMasterData.findIndex(master => master.masterId === this.editingMasterId);
    }
    return index;
  }





  // Component Events

  // This method should be refactored such that - it after some time elapsed DB call should be made by sending the list of selected masters.
  onMasterSelected(item: any) {
    if (item) {
      this.showHideMasters(item);
      this.selectedMasters = item;
    }
  }

  onMasterEditClicked(event: Event) {
    this.isEditEnabled = !this.isEditEnabled;
    const id = (event.currentTarget as Element).id;
    const data = this.clone(this.selectedMasterData);

    if (data) {
      data.find(val => val.masterId.toString() === id).isEdit = true;
    }

    // tslint:disable-next-line: radix
    this.editingMasterId = parseInt(id);
    this.selectedMasterData = data;
  }


  onMasterEditComplete(event: Event) {
    if (this.masterService.saveMaster(this.selectedMasterData[this.editingMasterId])) {
      this.isEditEnabled = !this.isEditEnabled;
      const id = (event.currentTarget as Element).id;
      const data = this.clone(this.selectedMasterData);

      if (data) {
        data.find(val => val.masterId.toString() === id).isEdit = false;
      }
      this.selectedMasterData = data;
    } else {
      // show save error message here.
    }
  }


  onRowEditInit(editedData: any) {
    this.clonedMasterData[editedData.Id] = { ...editedData };
  }

  onRowEditSave(editedData: any) {
    if (editedData) {
      delete this.clonedMasterData[editedData.Id];
    }
  }

  onRowEditCancel(editedData: any, rowIndex: number) {
    const masterIndex = this.getEditingMasterIndex();
    if (masterIndex >= 0) {
      this.selectedMasterData[masterIndex].data[rowIndex] = this.clonedMasterData[editedData.Id];
      delete this.clonedMasterData[editedData.Id];
    } else {
      // pop up editing error message here
    }
  }
}



