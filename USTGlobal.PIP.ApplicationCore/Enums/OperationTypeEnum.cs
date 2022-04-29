public enum OperationType
{
    Resend = 0,
    Revoke = 1,
    RoleAndPermission = 2,
    AdminAdd = 3,
    AdminDelete = 4,
    FinanceApproverAdd = 5,
    FinanceApproverDelete = 6,
    AllReadOnly = 7,
    DeleteAllReadOnly = 8,
    AdminCheckin = 9,
    SharedPIP = 10,
    SharedPIPUpdate = 11,
    SharedPIPDelete = 12,
    PIPVersionDelete = 13,
    Report = 14
}

public enum EmailStatus
{
    Success = 1,
    InProgress = 2,
    Failed = 3
}
