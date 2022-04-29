import { ResourcePlanning, Resource, ResourcePeriod, ProjectPeriod, ResourcePlanMasterData } from '@shared';
import { IMapper } from '../imapper';
import { Observable } from 'rxjs';
import { Http } from '@angular/http';
import { isNullOrUndefined } from 'util';
import { DateService } from '@core/services/date.service';
import { ISummaryLocations } from '@shared/domain/ISummaryLocations';

export class ResourceMapper implements ResourcePlanning {
    resources: Resource[];
    projectPeriods: ProjectPeriod[];
    resourcePlanMasterData: ResourcePlanMasterData[];

    constructor(private dateService: DateService) {

    }

    mapper(input: any): this {
        Object.assign(this, input);
        return this;
    }

    computeDyanmicColumns(periods: any[]) {
        let index = 0;
        const periodsColumns: any[] = [];
        periods.forEach(period => {

            index = index + 1;
            if (period.month === 0) {
                periodsColumns.push({
                    'field': 'periods',
                    'header': 'Year ' + '\n' + period.year
                });
                index--;
            } else {

                periodsColumns.push({
                    'field': 'periods',
                    'header': 'P' + index + '\n' + this.dateService.getMonthName(period.month - 1) + ' ' + period.year
                });
            }
        });
        return periodsColumns;
    }
    computeDyanmicPLForecasteColumns(periods: any[]) {
        let index = 0;
        const periodsColumns: any[] = [];
        periods.forEach(period => {
            index = index + 1;
            if (period.month === 0) {
                periodsColumns.push({
                    'field': 'periods',
                    'header': 'Year ' + '\n' + period.year,
                    'isCollapse': true,
                    'year': period.year,
                    'billingPeriodId': period.billingPeriodId
                });
                index--;
            } else {
                periodsColumns.push({
                    'field': 'periods',
                    'header': 'P' + index + '\n' + this.dateService.getMonthName(period.month - 1) + ' ' + period.year,
                    'isCollapse': true,
                    'year': period.year,
                    'billingPeriodId': period.billingPeriodId
                });
            }
        });
        return periodsColumns;
    }
    computeCashFlowDyanmicColumns(periods: any[]) {
        let index = 0;
        const periodsColumns: any[] = [];
        const counter = 0;
        periods.forEach(period => {

            index = index + 1;
            if (period.month === 0) {
                periodsColumns.push({
                    'field': 'periods',
                    'header': 'Year ' + '\n' + period.year,
                    'collapse': false,
                    'counter': counter + 1

                });
                index--;
            } else {

                periodsColumns.push({
                    'field': 'periods',
                    'header': 'P' + index + '\n' + this.dateService.getMonthName(period.month - 1) + ' ' + period.year,
                    'collapse': false,
                    'counter': 0
                });
            }
        });
        return periodsColumns;
    }


    computeDyanmicLocationColumns(periods: ISummaryLocations[]) {
        let index = 0;
        const periodsColumns: any[] = [];
        periods.forEach(period => {
            index = index + 1;
            periodsColumns.push({
                'field': 'periods',
                'header': period.locationName
            });
        });
        return periodsColumns;
    }

    computeDyanmicYearColumns(periods: any[]) {
        let index = 0;
        const periodsColumns: any[] = [];
        periods.forEach(period => {
            index = index + 1;
            periodsColumns.push({
                'field': 'periods',
                'header': + period.year
            });
        });
        return periodsColumns;
    }

    formulateResourceModel(resources: Resource[], periods: ProjectPeriod[]): Resource[] {
        let index = -1;
        resources.forEach(resource => {
            index = index + 1;
            resource.uId = index;
            resource.markupId = isNullOrUndefined(resource.markupId) ? -1 : resource.markupId;
            resource.milestoneId = isNullOrUndefined(resource.milestoneId) ? -1 : resource.milestoneId;

            if (resource.projectPeriod.length > 0 && resource.projectPeriod.length === periods.length) {
                // if pipsheet already exists
                resource.projectPeriod.forEach(period => {
                    period.uId = index;
                });

            }
            else if (resource.projectPeriod.length !== periods.length) {

                const existBillingPeriodIds = resource.projectPeriod.map(p => p.billingPeriodId);
                // const nonExistPeriods = periods.filter(p => !existBillingPeriodIds.includes(p.billingPeriodId));

                periods.forEach(period => {

                    if (!existBillingPeriodIds.includes(period.billingPeriodId)) {
                        resource.projectPeriod.push({
                            billingPeriodId: period.billingPeriodId,
                            fteValue: 0,
                            revenue: 0,
                            projectResourceId: resource.projectResourceId,
                            projectResourcePeriodDetailsId: 0,
                            uId: index,
                            totalHours: 0,
                            priceAdjustment: 0,
                            costHours: 0,
                            inflation: 0,
                            inflatedCostHours: 0,
                            cappedCost: 0,
                            billRate: 0,
                            costRate: 0
                        });
                    }
                });

                // set uId of periods to rowIndex of resources
                resource.projectPeriod.forEach(resourcePeriod => {
                    resourcePeriod.uId = index;
                });
            }
            else {
                // If pipsheet is new pipsheet in progress
                periods.forEach(period => {
                    resource.projectPeriod.push({
                        billingPeriodId: period.billingPeriodId,
                        fteValue: 0,
                        revenue: 0,
                        projectResourceId: 0,
                        projectResourcePeriodDetailsId: 0,
                        uId: index,
                        totalHours: 0,
                        priceAdjustment: 0,
                        costHours: 0,
                        inflation: 0,
                        inflatedCostHours: 0,
                        cappedCost: 0,
                        billRate: 0,
                        costRate: 0
                    });
                });
            }
        });

        return resources;

    }


}
