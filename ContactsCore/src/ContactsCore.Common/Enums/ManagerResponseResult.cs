namespace ContactsCore.Common.Enums
{
    public enum ManagerResponseResult
    {        
        UniqueKeyViolation = -3,
        ForeignKeyViolation = -2,
        UnknownError = -1,
        Success = 0, 
        Created = 1, 
        Updated = 2, 
        NotFound = 3
    }
}
