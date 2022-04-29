import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {StepComponent} from './step.component';
import {StepsComponent} from './steps.component';

import {ButtonModule} from 'primeng/components/button/button';
import {StepsModule} from 'primeng/components/steps/steps';
import {ScrollPanelModule} from 'primeng/scrollpanel';
import {CarouselModule} from 'primeng/carousel';
import { DialogModule } from 'primeng/dialog';

@NgModule({
    imports: [CommonModule, ButtonModule, StepsModule, ScrollPanelModule, CarouselModule, DialogModule],
    exports: [StepComponent, StepsComponent],
    declarations: [StepComponent, StepsComponent]
})
export class StepWizardModule {
}
