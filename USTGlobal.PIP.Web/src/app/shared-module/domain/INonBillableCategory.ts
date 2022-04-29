export interface INonBillableCategory {
    serialId: number;
    nonBillableCategoryId?: number;
    category: string;
    isActive: number;
    startDate?: Date;
    endDate?: Date;
    comments?: string;
    status: number;
}
